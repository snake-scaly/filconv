using Avalonia;
using Avalonia.Controls;

namespace FilConv.UI;

public partial class ScaleComboBox : UserControl
{
    public static readonly StyledProperty<double?> ScaleProperty =
        AvaloniaProperty.Register<ScaleComboBox, double?>(nameof(Scale));

    public double? Scale
    {
        get => GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, value);
    }

    public ScaleComboBox()
    {
        InitializeComponent();
    }

    private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Scale = (((sender as ComboBox)?.SelectedItem as Control)?.Tag as int?) / 100.0;
    }
}
