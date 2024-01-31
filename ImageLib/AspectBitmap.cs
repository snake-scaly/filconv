namespace ImageLib
{
    /// <summary>
    /// A raw image with an associated aspect ratio.
    /// </summary>
    public class AspectBitmap
    {
        public AspectBitmap(Bgr32BitmapData bitmap, double pixelAspect)
        {
            Bitmap = bitmap;
            PixelAspect = pixelAspect;
        }

        /// <summary>
        /// Gets Image data.
        /// </summary>
        public Bgr32BitmapData Bitmap { get; }

        /// <summary>
        /// Gets aspect ratio of the image pixels.
        /// </summary>
        /// <remarks>
        /// Aspect value is calculated as physical pixel width divided by physical
        /// pixel height as displayed by the original hardware.  For instance,
        /// aspect of 1.0 corresponds to square original pixels.
        /// </remarks>
        public double PixelAspect { get; }

        public static AspectBitmap FromImageAspect(Bgr32BitmapData bitmap, double imageAspect)
        {
            return new AspectBitmap(bitmap, imageAspect * bitmap.Height / bitmap.Width);
        }
    }
}
