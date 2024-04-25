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
        /// <summary>
        /// Output displays supported by the format.
        /// </summary>
        IEnumerable<NativeDisplay>? SupportedDisplays { get; }

        /// <summary>
        /// Displays supported for encoding.
        /// </summary>
        IEnumerable<NativeDisplay>? SupportedEncodingDisplays { get; }

        AspectBitmap FromNative(NativeImage native, DecodingOptions options);
        NativeImage ToNative(IReadOnlyPixels bitmap, EncodingOptions options);

        /// <summary>
        /// Compute probability of an image to be in this format.
        /// </summary>
        /// <param name="native">the image to test</param>
        /// <returns>Matching score. Higher value means better probability.</returns>
        int ComputeMatchScore(NativeImage native);

        DecodingOptions GetDefaultDecodingOptions(NativeImage native);

        /// <summary>
        /// Color palettes supported by the format on a given display. Null for none.
        /// </summary>
        IEnumerable<NativePalette>? GetSupportedPalettes(NativeDisplay display);
    }
}
