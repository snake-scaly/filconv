using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Data.Core;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilConv.Encode;
using FilConv.I18n;
using FilConv.Native;
using FilConv.Presenter;
using FilLib;
using ImageLib;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace FilConv;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly Dictionary<string, object> _originalPreviewSettings = new();
    private readonly Dictionary<string, object> _encodedPreviewSettings = new();

    [NotifyPropertyChangedFor(nameof(Title))]
    [ObservableProperty] private string _appTitle = string.Empty;
    [NotifyPropertyChangedFor(nameof(Title))]
    [ObservableProperty] private string? _fileName;
    [ObservableProperty] private IImagePresenter? _originalPresenter;
    [ObservableProperty] private IImagePresenter? _encodingPresenter;
    public string Title => FileName == null ? AppTitle : $"{Path.GetFileName(FileName)} - {AppTitle}";

    public MainWindowViewModel()
    {
        L10n.AddLocalizedProperty(
            this,
            new ClrPropertyInfo(nameof(AppTitle), _ => AppTitle, (_, v) => AppTitle = (string)v!, typeof(string)),
            "MainWindowTitle")
            .Update();
    }

    [RelayCommand]
    private async Task FileOpen(IStorageProvider storageProvider)
    {
        var options = new FilePickerOpenOptions { FileTypeFilter = GetFileFilterList() };

        var suggestedDir = Path.GetDirectoryName(FileName);
        if (suggestedDir != null)
            options.SuggestedStartLocation = await storageProvider.TryGetFolderFromPathAsync(suggestedDir);

        var picked = await storageProvider.OpenFilePickerAsync(options);
        if (picked.Count == 0)
            return;

        var fileName = picked[0].Path.LocalPath;

        try
        {
            IImagePresenter newLeftPresenter;

            if (fileName.EndsWith(".fil", StringComparison.InvariantCultureIgnoreCase))
            {
                var fil = Fil.FromFile(fileName);
                ImageMeta.TryParse(fil, out var metadata);
                var ni = new NativeImage
                {
                    Data = fil.GetData(),
                    Metadata = metadata,
                    FormatHint = new FormatHint(fileName),
                };
                newLeftPresenter = new NativeImagePresenter(ni);
            }
            else if (fileName.EndsWith(".scr", StringComparison.InvariantCultureIgnoreCase) ||
                     fileName.EndsWith(".bol", StringComparison.InvariantCultureIgnoreCase))
            {
                NativeImage ni = new NativeImage
                {
                    Data = File.ReadAllBytes(fileName),
                    FormatHint = new FormatHint(fileName)
                };
                newLeftPresenter = new NativeImagePresenter(ni);
            }
            else
            {
                var bmp = new Bitmap(fileName);
                _ = bmp.Format ?? throw new NotSupportedException("Unsupported image format");
                newLeftPresenter = new BitmapPresenter(bmp);
            }

            OriginalPresenter = newLeftPresenter;
            EncodingPresenter = new EncodingImagePresenter((IOriginal)newLeftPresenter);
            FileName = fileName;
        }
        catch (NotSupportedException e)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                (string)L10n.GetObject("ErrorMessageBoxTitle"),
                string.Format((string)L10n.GetObject("UnableToLoadFileError"), fileName, e.Message),
                icon: Icon.Error);
            await messageBox.ShowAsync();
        }
    }

    private bool FileSaveAsCanExecute(IStorageProvider storageProvider) =>
        EncodingPresenter != null;

    [RelayCommand(CanExecute = nameof(FileSaveAsCanExecute))]
    private async Task FileSaveAs(IStorageProvider storageProvider)
    {
        var eip = (EncodingImagePresenter)EncodingPresenter!;
        var delegates = eip.SaveDelegates.ToList();
        var defaultDelegate = delegates[0];

        var choices = delegates
            .Select(
                x => new FilePickerFileType(L10n.GetObject(x.FormatNameL10nKey) as string)
                {
                    Patterns = x.FileNameMasks.ToList()
                })
            .ToList();

        var options = new FilePickerSaveOptions
        {
            FileTypeChoices = choices,
            DefaultExtension = defaultDelegate.FileNameMasks.First(),
            SuggestedFileName = Path.GetFileNameWithoutExtension(FileName),
        };

        var suggestedDir = Path.GetDirectoryName(FileName);
        if (suggestedDir != null)
            options.SuggestedStartLocation = await storageProvider.TryGetFolderFromPathAsync(suggestedDir);

        var picked = await storageProvider.SaveFilePickerAsync(options);

        if (picked == null)
            return;

        var pickedExt = Path.GetExtension(picked.Name);
        ISaveDelegate? pickedDelegate = null;

        if (!string.IsNullOrEmpty(pickedExt))
            pickedDelegate = delegates.FirstOrDefault(x => x.FileNameMasks.Any(y => y.EndsWith(pickedExt)));

        if (pickedDelegate == null)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                (string)L10n.GetObject("ErrorMessageBoxTitle"),
                "The file must have a supported extension",
                icon: Icon.Error);
            await messageBox.ShowAsync();
            return;
        }

        try
        {
            pickedDelegate.SaveAs(picked.Path.LocalPath);
        }
        catch (Exception ex)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                (string)L10n.GetObject("ErrorMessageBoxTitle"),
                ex.Message,
                icon: Icon.Error);
            await messageBox.ShowAsync();
        }
    }

    [RelayCommand]
    private void ChangeLanguage(object? param)
    {
        L10n.Culture = param is string s ? new CultureInfo(s) : null;
    }

    partial void OnOriginalPresenterChanged(IImagePresenter? oldValue, IImagePresenter? newValue)
    {
        oldValue?.StoreSettings(_originalPreviewSettings);
        newValue?.AdoptSettings(_originalPreviewSettings);
        oldValue?.Dispose();
    }

    partial void OnEncodingPresenterChanged(IImagePresenter? oldValue, IImagePresenter? newValue)
    {
        oldValue?.StoreSettings(_encodedPreviewSettings);
        newValue?.AdoptSettings(_encodedPreviewSettings);
        oldValue?.Dispose();
    }

    private static IReadOnlyList<FilePickerFileType> GetFileFilterList()
    {
        var all = new FilePickerFileType("All files (*.*)") { Patterns = ["*.*"] };
        var allSupported = new FilePickerFileType("Supported formats")
        {
            Patterns = _supportedFiles.SelectMany(x => x.Extensions).ToList(),
        };
        var individual = _supportedFiles.Select(x => new FilePickerFileType(FormatName(x)) { Patterns = x.Extensions });

        return Enumerable.Empty<FilePickerFileType>()
            .Append(allSupported)
            .Concat(individual)
            .Append(all)
            .ToList();

        string FormatName(SupportedFile sf) =>
            $"{L10n.GetObject(sf.Name) as string} ({string.Join(", ", sf.Extensions)})";
    }

    private record struct SupportedFile(string Name, string[] Extensions);

    private static readonly SupportedFile[] _supportedFiles = [
        new SupportedFile("FileFormatNameFil", ["*.fil", "*.FIL"]),
        new SupportedFile("FileFormatNameScr", ["*.scr"]),
        new SupportedFile("FileFormatNameBmp", ["*.bmp"]),
        new SupportedFile("FileFormatNameJpeg", ["*.jpg", "*,jpeg"]),
        new SupportedFile("FileFormatNamePng", ["*.png"]),
        new SupportedFile("FileFormatNameGif", ["*.gif"]),
        new SupportedFile("FileFormatNameTiff", ["*.tif", "*.tiff"])
    ];
}
