using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using FilConv.I18n;

namespace FilConv.Services;

public class FileChooserService : IFileChooserService
{
    private readonly IStorageProviderAccessor _storageProviderAccessor;

    public FileChooserService(IStorageProviderAccessor storageProviderAccessor)
    {
        _storageProviderAccessor = storageProviderAccessor;
    }

    private IStorageProvider StorageProvider =>
        _storageProviderAccessor.StorageProvider
        ?? throw new InvalidOperationException($"{nameof(IStorageProvider)} is not configured");

    public async Task<string?> OpenChooserAsync(
        IEnumerable<SupportedFile> supportedFiles,
        string? currentFileName)
    {
        var options = new FilePickerOpenOptions { FileTypeFilter = SupportedFilesToFilter(supportedFiles).ToList() };

        if (currentFileName != null)
        {
            string suggestedDir = Path.GetDirectoryName(currentFileName)!;
            options.SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(suggestedDir);
        }

        var picked = await StorageProvider.OpenFilePickerAsync(options);

        return picked.Count == 0 ? null : picked[0].Path.LocalPath;
    }

    public async Task<string?> SaveChooserAsync(
        IEnumerable<SupportedFile> supportedFiles,
        string? currentFileName)
    {
        var options = new FilePickerSaveOptions
        {
            FileTypeChoices = SupportedFilesToFilter(supportedFiles).ToList(),
        };

        if (currentFileName != null)
        {
            options.SuggestedFileName = Path.GetFileNameWithoutExtension(currentFileName);
            string suggestedDir = Path.GetDirectoryName(currentFileName)!;
            options.SuggestedStartLocation = await StorageProvider.TryGetFolderFromPathAsync(suggestedDir);
        }

        var picked = await StorageProvider.SaveFilePickerAsync(options);

        return picked?.Path.LocalPath;
    }

    private static IEnumerable<FilePickerFileType> SupportedFilesToFilter(IEnumerable<SupportedFile> supportedFiles)
    {
        return supportedFiles.Select(x =>
            new FilePickerFileType(SupportedFileName(x)) { Patterns = SupportedFilePatterns(x).ToList() });
    }

    private static string SupportedFileName(SupportedFile supportedFile)
    {
        var patterns = string.Join(", ", SupportedFilePatterns(supportedFile));
        return $"{L10n.GetObject(supportedFile.Name)} ({patterns})";
    }

    private static IEnumerable<string> SupportedFilePatterns(SupportedFile supportedFile)
    {
        return supportedFile.Suffixes.Select(x => $"*{x}");
    }
}
