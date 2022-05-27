using System.Windows.Media.Imaging;

namespace ImageLib
{
    /// <summary>
    /// Image converter interface.
    /// </summary>
    /// <para>This interface provides routines to convert between
    /// standard raster image representation and bytes in a particular
    /// native format.</para>
    public interface INativeImageFormat
    {
        AspectBitmap FromNative(NativeImage native);
        NativeImage ToNative(BitmapSource bitmap, EncodingOptions options);

        /// <summary>
        /// Compute probability of an image to be in this format.
        /// </summary>
        /// <param name="native">the image to test</param>
        /// <returns>Matching score. Higher value means better probability.</returns>
        int ComputeMatchScore(NativeImage native);
    }
}
