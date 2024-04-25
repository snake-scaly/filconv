using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace FilConv.UI;

public class ScaledBitmap : Control
{
    public static readonly DirectProperty<ScaledBitmap, Bitmap?> SourceProperty =
        AvaloniaProperty.RegisterDirect<ScaledBitmap, Bitmap?>(nameof(Source), o => o.Source, (o, v) => o.Source = v);

    public static readonly DirectProperty<ScaledBitmap, double?> ScaleProperty =
        AvaloniaProperty.RegisterDirect<ScaledBitmap, double?>(nameof(Scale), o => o.Scale, (o, v) => o.Scale = v);

    public static readonly DirectProperty<ScaledBitmap, double> AspectProperty =
        AvaloniaProperty.RegisterDirect<ScaledBitmap, double>(nameof(Aspect), o => o.Aspect, (o, v) => o.Aspect = v);

    public static readonly DirectProperty<ScaledBitmap, Size> FitSizeProperty =
        AvaloniaProperty.RegisterDirect<ScaledBitmap, Size>(nameof(FitSize), o => o.FitSize, (o, v) => o.FitSize = v);

    private Bitmap? _source;
    private double? _scale;
    private double _aspect;
    private Size _fitSize;

    public Bitmap? Source
    {
        get => _source;
        set
        {
            if (SetAndRaise(SourceProperty, ref _source, value))
            {
                InvalidateMeasure();
                InvalidateVisual();
            }
        }
    }

    public double? Scale
    {
        get => _scale;
        set
        {
            if (SetAndRaise(ScaleProperty, ref _scale, value))
                InvalidateMeasure();
        }
    }

    public double Aspect
    {
        get => _aspect;
        set
        {
            if (SetAndRaise(AspectProperty, ref _aspect, value))
                InvalidateMeasure();
        }
    }

    public Size FitSize
    {
        get => _fitSize;
        set
        {
            if (SetAndRaise(FitSizeProperty, ref _fitSize, value))
                InvalidateMeasure();
        }
    }

    public override void Render(DrawingContext drawingContext)
    {
        if (_source == null)
            return;
        drawingContext.DrawImage(_source, new Rect(GetScaledSourceSize()));
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return _source == null ? default : GetScaledSourceSize();
    }

    private Size GetScaledSourceSize()
    {
        if (_source == null)
            return default;
        var size = new Size(_source.Size.Width * _aspect, _source.Size.Height);
        var scale = _scale ?? Math.Min(_fitSize.Width / size.Width, _fitSize.Height / size.Height);
        return size * scale;
    }
}
