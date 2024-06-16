using Avalonia.Platform.Storage;

namespace FilConv.Services;

public interface IStorageProviderAccessor
{
    IStorageProvider? StorageProvider { get; set; }
}
