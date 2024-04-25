using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FilConv;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void menuExit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void menuAbout_Click(object sender, RoutedEventArgs e)
    {
        AboutBox about = new AboutBox();
        about.ShowDialog(this);
    }
}
