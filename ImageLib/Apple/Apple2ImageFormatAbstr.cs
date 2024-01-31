using System.Collections.Generic;

namespace ImageLib.Apple
{
    public abstract class Apple2ImageFormatAbstr : INativeImageFormat
    {
        public IEnumerable<NativeDisplay> SupportedDisplays { get; } =
            new[] { NativeDisplay.Color, NativeDisplay.Mono, NativeDisplay.Artifact };

        public IEnumerable<NativePalette> SupportedPalettes => null;

        public abstract AspectBitmap FromNative(NativeImage native, DecodingOptions options);
        public abstract NativeImage ToNative(IReadOnlyPixels bitmap, EncodingOptions options);
        public abstract int ComputeMatchScore(NativeImage native);

        public DecodingOptions GetDefaultDecodingOptions(NativeImage native) => default;
    }
}
