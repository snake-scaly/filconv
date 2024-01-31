using System.Windows.Media.Imaging;

namespace FilConvWpf
{
    public class AspectBitmapSource
    {
        public BitmapSource Bitmap { get; }
        public double PixelAspect { get; }

        public AspectBitmapSource(BitmapSource bitmap, double pixelAspect)
        {
            Bitmap = bitmap;
            PixelAspect = pixelAspect;
        }
    }
}
