using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace ImageLib
{
    /// <summary>
    /// Image converter interface.
    /// </summary>
    /// <remarks>
    /// This interface provides routines to convert between
    /// standard raster image representation and bytes in a particular
    /// native format.
    /// </remarks>
    public interface INativeImageFormat
    {
        /// Color palettes supported by the format. Null for none.
        IEnumerable<NativePalette> SupportedPalettes { get; }

        AspectBitmap FromNative(NativeImage native, DecodingOptions options);
        NativeImage ToNative(BitmapSource bitmap, EncodingOptions options);

        /// Compute probability of an image to be in this format.
        /// <param name="native">the image to test</param>
        /// <returns>Matching score. Higher value means better probability.</returns>
        int ComputeMatchScore(NativeImage native);

        DecodingOptions GetDefaultDecodingOptions(NativeImage native);
    }
}
