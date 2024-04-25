using System.Reflection;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FilConv;

/// <summary>
/// Interaction logic for AboutBox.axaml
/// </summary>
public partial class AboutBox : Window
{
    public AboutBox()
    {
        InitializeComponent();
        var appVersion = Assembly.GetExecutingAssembly().GetName().Version;
        version.Text = appVersion == null
            ? "???"
            : $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}";
    }

    private void Button_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
