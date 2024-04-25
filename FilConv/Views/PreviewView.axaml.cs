using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using FilConv.ViewModels;

namespace FilConv.Views;

public partial class PreviewView
{
    public static readonly StyledProperty<AspectBitmapSource> AspectBitmapProperty =
        AvaloniaProperty.Register<PreviewView, AspectBitmapSource>(nameof(AspectBitmap));

    public static readonly StyledProperty<double?> ScaleProperty =
        AvaloniaProperty.Register<PreviewView, double?>(nameof(Scale));

    public static readonly StyledProperty<bool> UseNativeAspectProperty =
        AvaloniaProperty.Register<PreviewView, bool>(nameof(UseNativeAspect));


    public AspectBitmapSource AspectBitmap
    {
        get => GetValue(AspectBitmapProperty);
        set => SetValue(AspectBitmapProperty, value);
    }

    public double? Scale
    {
        get => GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, value);
    }

    public bool UseNativeAspect
    {
        get => GetValue(UseNativeAspectProperty);
        set => SetValue(UseNativeAspectProperty, value);
    }
}

public partial class PreviewView : UserControl
{
    private bool _drag;
    private Vector _dragAnchor;

    public PreviewView()
    {
        InitializeComponent();

        (Content as StyledElement)!.DataContext = new PreviewViewModel(
            this.GetObservable(AspectBitmapProperty),
            this.GetObservable(ScaleProperty).Prepend(Scale),
            this.GetObservable(UseNativeAspectProperty));
    }

    private void Bitmap_OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        var pp = e.GetCurrentPoint(this);
        if (pp.Properties.IsLeftButtonPressed)
        {
            e.Pointer.Capture(sender as InputElement);
            _drag = true;
            _dragAnchor = Scroller.Offset + pp.Position;
        }
    }

    private void Bitmap_OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        e.Pointer.Capture(null);
        _drag = false;
    }

    private void Bitmap_OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (!_drag) return;
        var p = e.GetPosition(this);
        Scroller.Offset = _dragAnchor - p;
    }
}
