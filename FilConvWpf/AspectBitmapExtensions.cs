using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageLib;

namespace FilConvWpf
{
    public static class AspectBitmapExtensions
    {
        public static AspectBitmapSource ToAspectBitmapSource(this AspectBitmap bmp)
        {
            var writeableBitmap =
                new WriteableBitmap(bmp.Bitmap.Width, bmp.Bitmap.Height, 96, 96, PixelFormats.Bgr32, null);

            writeableBitmap.WritePixels(
                new Int32Rect(0, 0, bmp.Bitmap.Width, bmp.Bitmap.Height),
                bmp.Bitmap.Pixels,
                bmp.Bitmap.Width * 4,
                0);

            return new AspectBitmapSource(writeableBitmap, bmp.PixelAspect);
        }
    }
}
