using Avalonia.Platform.Storage;

namespace FilConv.Services;

public class StorageProviderAccessor : IStorageProviderAccessor
{
    public IStorageProvider? StorageProvider { get; set; }
}