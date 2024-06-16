using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FilConv.Presenter;
using FilConv.Services;

namespace FilConv.ViewModels;

public class DesignMainWindowViewModel : MainWindowViewModel
{
    public DesignMainWindowViewModel() : base(new FakeFileChooserService(), new FakeImageFileService())
    {
    }

    private class FakeFileChooserService : IFileChooserService
    {
        public Task<string?> OpenChooserAsync(IEnumerable<SupportedFile> supportedFiles, string? currentFileName) =>
            throw new NotSupportedException();
        public Task<string?> SaveChooserAsync(IEnumerable<SupportedFile> supportedFiles, string? currentFileName) =>
            throw new NotSupportedException();
    }

    private class FakeImageFileService : IImageFileService
    {
        public Task<IImagePresenter> LoadAsync(string fileName) => throw new NotSupportedException();
        public Task<IImagePresenter> LoadRawAsync(string fileName) => throw new NotSupportedException();
    }
}
