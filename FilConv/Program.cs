using System;
using System.IO;
using Avalonia;

namespace FilConv;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            var errorLogName = Path.Join(AppContext.BaseDirectory, "error.log");
            File.AppendAllText(errorLogName, e.ToString());
            throw;
        }
    }

    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
    }
}
