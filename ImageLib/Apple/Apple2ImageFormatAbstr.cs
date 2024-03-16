using System.Collections.Generic;

namespace ImageLib.Apple
{
    public abstract class Apple2ImageFormatAbstr : INativeImageFormat
    {
        public virtual IEnumerable<NativeDisplay> SupportedDisplays { get; } =
            new[] { NativeDisplay.Color, NativeDisplay.Mono, NativeDisplay.Artifact };

        public virtual IEnumerable<NativeDisplay> SupportedEncodingDisplays => null;

        public abstract AspectBitmap FromNative(NativeImage native, DecodingOptions options);
        public abstract NativeImage ToNative(IReadOnlyPixels bitmap, EncodingOptions options);
        public abstract int ComputeMatchScore(NativeImage native);

        public virtual DecodingOptions GetDefaultDecodingOptions(NativeImage native) => default;
        public IEnumerable<NativePalette> GetSupportedPalettes(NativeDisplay display) => null;
    }
}
