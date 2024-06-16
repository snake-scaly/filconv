using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilConv.Services;

public interface IFileChooserService
{
    Task<string?> OpenChooserAsync(IEnumerable<SupportedFile> supportedFiles, string? currentFileName);
    Task<string?> SaveChooserAsync(IEnumerable<SupportedFile> supportedFiles, string? currentFileName);
}
