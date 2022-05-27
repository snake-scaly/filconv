using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FilConvWpf
{
    public class ScaledBitmap : FrameworkElement
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source),
            typeof(BitmapSource),
            typeof(ScaledBitmap),
            new FrameworkPropertyMetadata
            {
                AffectsMeasure = true,
                AffectsRender = true,
                PropertyChangedCallback = SourcePropertyChangedCallback
            });

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            nameof(Scale),
            typeof(double?),
            typeof(ScaledBitmap),
            new FrameworkPropertyMetadata { DefaultValue = 1.0, AffectsMeasure = true });

        public static readonly DependencyProperty AspectProperty = DependencyProperty.Register(
            nameof(Aspect),
            typeof(double),
            typeof(ScaledBitmap),
            new FrameworkPropertyMetadata { DefaultValue = 1.0, AffectsMeasure = true });

        public static readonly DependencyProperty FitWidthProperty = DependencyProperty.Register(
            nameof(FitWidth),
            typeof(double),
            typeof(ScaledBitmap),
            new FrameworkPropertyMetadata { DefaultValue = 300.0, AffectsMeasure = true });

        public static readonly DependencyProperty FitHeightProperty = DependencyProperty.Register(
            nameof(FitHeight),
            typeof(double),
            typeof(ScaledBitmap),
            new FrameworkPropertyMetadata { DefaultValue = 300.0, AffectsMeasure = true });

        public event EventHandler<ExceptionEventArgs> BitmapFailed;

        public BitmapSource Source
        {
            get => (BitmapSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public double? Scale
        {
            get => (double?)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public double Aspect
        {
            get => (double)GetValue(AspectProperty);
            set => SetValue(AspectProperty, value);
        }

        public double FitWidth
        {
            get => (double)GetValue(FitWidthProperty);
            set => SetValue(FitWidthProperty, value);
        }

        public double FitHeight
        {
            get => (double)GetValue(FitHeightProperty);
            set => SetValue(FitHeightProperty, value);
        }

        private Size ScaledSourceSize
        {
            get
            {
                var size = new Size(Source.PixelWidth * Aspect, Source.PixelHeight);
                var scale = Scale ?? Math.Min(FitWidth / size.Width, FitHeight / size.Height);
                return new Size(Source.PixelWidth * scale * Aspect, Source.PixelHeight * scale);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return Source == null ? default : ScaledSourceSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Source == null)
                return;
            drawingContext.DrawImage(Source, new Rect(ScaledSourceSize));
        }

        private static void SourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bitmap = (ScaledBitmap)d;

            var oldValue = (BitmapSource)e.OldValue;
            var newValue = (BitmapSource)e.NewValue;

            if (oldValue != null && !oldValue.IsFrozen)
            {
                oldValue.DownloadCompleted -= bitmap.OnSourceDownloaded;
                oldValue.DownloadFailed -= bitmap.OnSourceFailed;
                oldValue.DecodeFailed -= bitmap.OnSourceFailed;
            }
            if (newValue != null && !newValue.IsFrozen)
            {
                newValue.DownloadCompleted += bitmap.OnSourceDownloaded;
                newValue.DownloadFailed += bitmap.OnSourceFailed;
                newValue.DecodeFailed += bitmap.OnSourceFailed;
            }
        }

        private void OnSourceDownloaded(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        private void OnSourceFailed(object sender, ExceptionEventArgs e)
        {
            BitmapFailed?.Invoke(this, e);
        }
    }
}
