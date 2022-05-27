using System.Windows.Media.Imaging;

namespace ImageLib
{
    /// <summary>
    /// An image to be displayed.
    /// </summary>
    public class AspectBitmap
    {
        public AspectBitmap(BitmapSource bitmap, double pixelAspect)
        {
            Bitmap = bitmap;
            PixelAspect = pixelAspect;
        }

        public BitmapSource Bitmap { get; }

        /// <summary>
        /// Gets aspect ratio of the image pixels.
        /// </summary>
        /// <remarks>
        /// Aspect value is calculated as physical pixel width divided by physical
        /// pixel height as displayed by the original hardware.  For instance,
        /// aspect of 1.0 corresponds to square original pixels.
        /// </remarks>
        public double PixelAspect { get; }

        public static AspectBitmap FromImageAspect(BitmapSource bitmap, double imageAspect)
        {
            return new AspectBitmap(bitmap, imageAspect * bitmap.PixelHeight / bitmap.PixelWidth);
        }
    }
}
