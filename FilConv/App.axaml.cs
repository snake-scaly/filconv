using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FilConv.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FilConv;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        services
            .AddSingleton<IImageFileService, ImageFileService>()
            .AddSingleton<IFileChooserService, FileChooserService>()
            .AddSingleton<IStorageProviderAccessor, StorageProviderAccessor>()
            .AddSingleton<MainWindowViewModel>();

        var serviceProvider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
            };

            serviceProvider.GetRequiredService<IStorageProviderAccessor>().StorageProvider =
                desktop.MainWindow.StorageProvider;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
