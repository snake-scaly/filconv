using System.Collections.Generic;

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
        /// Output displays supported by the format.
        IEnumerable<NativeDisplay> SupportedDisplays { get; }

        /// Color palettes supported by the format. Null for none.
        IEnumerable<NativePalette> SupportedPalettes { get; }

        AspectBitmap FromNative(NativeImage native, DecodingOptions options);
        NativeImage ToNative(IReadOnlyPixels bitmap, EncodingOptions options);

        /// Compute probability of an image to be in this format.
        /// <param name="native">the image to test</param>
        /// <returns>Matching score. Higher value means better probability.</returns>
        int ComputeMatchScore(NativeImage native);

        DecodingOptions GetDefaultDecodingOptions(NativeImage native);
    }
}
