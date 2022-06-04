using System.Collections.Generic;
using System.Windows.Media.Imaging;
using FilLib;
using ImageLib.Apple;
using ImageLib.Apple.HiRes;
using ImageLib.Util;

namespace ImageLib.Agat
{
    public class AgatAppleImageFormat : INativeImageFormat
    {
        public IEnumerable<NativeDisplay> SupportedDisplays { get; } =
            new[] { NativeDisplay.Color, NativeDisplay.Mono, NativeDisplay.Meta };
        public IEnumerable<NativePalette> SupportedPalettes => null;

        public AspectBitmap FromNative(NativeImage native, DecodingOptions options)
        {
            var colors = AgatColorUtils.NativeDisplayToColors(options.Display, native.Metadata);
            var tv = new Apple2SimpleTv(PalToApple(colors));
            return Apple2HiResSimpleRenderer.Render(native, tv);
        }

        public NativeImage ToNative(BitmapSource bitmap, EncodingOptions options)
        {
            throw new System.NotImplementedException();
        }

        public int ComputeMatchScore(NativeImage native)
        {
            if (native.Metadata?.DisplayMode == ImageMeta.Mode.Agat_280_192_AppleHiRes)
                return NativeImageFormatUtils.MetaMatchScore;
            return NativeImageFormatUtils.ComputeMatch(native, 0x2000);
        }

        public DecodingOptions GetDefaultDecodingOptions(NativeImage native)
        {
            return new DecodingOptions { Display = NativeDisplay.Color, Palette = NativePalette.Default };
        }

        private static Rgb[] PalToApple(IList<Rgb> agatColors)
        {
            return new[] { agatColors[0], agatColors[2], agatColors[5], agatColors[4], agatColors[1], agatColors[7] };
        }
    }
}
