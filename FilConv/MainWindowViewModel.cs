using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Data.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilConv.Encode;
using FilConv.I18n;
using FilConv.Presenter;
using FilConv.Services;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace FilConv;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IImageFileService _imageFileService;
    private readonly IFileChooserService _fileChooserService;
    private readonly Dictionary<string, object> _originalPreviewSettings = new();
    private readonly Dictionary<string, object> _encodedPreviewSettings = new();

    [NotifyPropertyChangedFor(nameof(Title))]
    [ObservableProperty] private string _appTitle = string.Empty;
    [NotifyPropertyChangedFor(nameof(Title))]
    [ObservableProperty] private string? _fileName;
    [ObservableProperty] private IImagePresenter? _originalPresenter;
    [ObservableProperty] private IImagePresenter? _encodingPresenter;
    public string Title => FileName == null ? AppTitle : $"{Path.GetFileName(FileName)} - {AppTitle}";

    public MainWindowViewModel(IFileChooserService fileChooserService, IImageFileService imageFileService)
    {
        _fileChooserService = fileChooserService;
        _imageFileService = imageFileService;

        L10n.AddLocalizedProperty(
            this,
            new ClrPropertyInfo(nameof(AppTitle), _ => AppTitle, (_, v) => AppTitle = (string)v!, typeof(string)),
            "MainWindowTitle")
            .Update();
    }

    [RelayCommand]
    private async Task FileOpen()
    {
        var allSuffixes = _supportedFiles.SelectMany(x => x.Suffixes).Distinct().ToList();
        var allSupported = new SupportedFile("FileFormatNameAllSupported", allSuffixes);

        var all = Enumerable.Empty<SupportedFile>()
            .Append(allSupported)
            .Concat(_supportedFiles)
            .Append(_allFiles);

        var picked = await _fileChooserService.OpenChooserAsync(all, FileName);
        if (picked == null)
            return;

        try
        {
            var newLeftPresenter = await _imageFileService.LoadAsync(picked);

            OriginalPresenter = newLeftPresenter;
            EncodingPresenter = new EncodingImagePresenter((IOriginal)newLeftPresenter);
            FileName = picked;
        }
        catch (Exception e)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                (string)L10n.GetObject("ErrorMessageBoxTitle"),
                string.Format((string)L10n.GetObject("UnableToLoadFileError"), picked, e.Message),
                icon: Icon.Error);
            await messageBox.ShowAsync();
        }
    }

    [RelayCommand]
    private async Task FileOpenRaw()
    {
        var picked = await _fileChooserService.OpenChooserAsync([_allFiles], FileName);
        if (picked == null)
            return;

        try
        {
            var newLeftPresenter = await _imageFileService.LoadRawAsync(picked);

            OriginalPresenter = newLeftPresenter;
            EncodingPresenter = new EncodingImagePresenter((IOriginal)newLeftPresenter);
            FileName = picked;
        }
        catch (Exception e)
        {
            var messageBox = MessageBoxManager.GetMessageBoxStandard(
                (string)L10n.GetObject("ErrorMessageBoxTitle"),
                string.Format((string)L10n.GetObject("UnableToLoadFileError"), picked, e.Message),
                icon: Icon.Error);
            await messageBox.ShowAsync();
        }
    }

    private bool FileSaveAsCanExecute() => EncodingPresenter is EncodingImagePresenter;

    [RelayCommand(CanExecute = nameof(FileSaveAsCanExecute))]
    private async Task FileSaveAs()
    {
        var eip = (EncodingImagePresenter)EncodingPresenter!;
        var delegates = eip.SaveDelegates.ToList();
        var choices = delegates.Select(x => new SupportedFile(x.FormatNameL10nKey, x.FileNameSuffixes.ToList()));

        var picked = await _fileChooserService.SaveChooserAsync(choices, FileName);
        if (picked == null)
            return;

        var pickedExt = Path.GetExtension(picked);
        ISaveDelegate? pickedDelegate = null;

        if (!string.IsNullOrEmpty(pickedExt))
            pickedDelegate = delegates.FirstOrDefault(x => x.FileNameSuffixes.Any(y => y.EndsWith(pickedExt)));

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
            pickedDelegate.SaveAs(picked);
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

    private bool FileRawSaveAsCanExecute() =>
        EncodingPresenter is EncodingImagePresenter { RawSaveDelegate: not null };

    [RelayCommand(CanExecute = nameof(FileRawSaveAsCanExecute))]
    private async Task FileSaveRawAs()
    {
        var eip = (EncodingImagePresenter)EncodingPresenter!;

        var picked = await _fileChooserService.SaveChooserAsync([_allFiles], FileName);
        if (picked == null)
            return;

        try
        {
            eip.RawSaveDelegate!.SaveAs(picked);
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

    private static readonly SupportedFile[] _supportedFiles =
    [
        new SupportedFile("FileFormatNameFil", [".fil", ".FIL"]),
        new SupportedFile("FileFormatNameScr", [".scr"]),
        new SupportedFile("FileFormatNameBmp", [".bmp"]),
        new SupportedFile("FileFormatNameJpeg", [".jpg", ".jpeg"]),
        new SupportedFile("FileFormatNamePng", [".png"]),
        new SupportedFile("FileFormatNameGif", [".gif"]),
        new SupportedFile("FileFormatNameTiff", [".tif", ".tiff"])
    ];

    private static readonly SupportedFile _allFiles = new("FileFormatNameAll", [string.Empty]);
}
