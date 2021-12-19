using System;
using System.Windows.Media;
using FilLib;
using ImageLib.Gamut;

namespace ImageLib.Agat
{
    public class Mgr9ImageFormat : AgatImageFormatAbstr
    {
        public override int ComputeMatchScore(NativeImage native)
        {
            if (native.Metadata?.DisplayMode == ImageMeta.Mode.Agat_256_256_Pal4)
                return NativeImageFormatUtils.MetaMatchScore;
            return base.ComputeMatchScore(native);
        }

        protected override int Width => 256;
        protected override int Height => 256;
        protected override int BitsPerPixel => 2;
        protected override IGamut Gamut { get; } = new Mgr9BlackGamut();

        protected override int GetLineOffset(int y)
        {
            int line = Math.DivRem(y, 2, out var bank);
            return (line + Height / 2 * bank) * BytesPerScanline;
        }

        protected override Color[] GetStandardPalette(ImageMeta.Palette variant) => _colorPalette;

        private static readonly Color[] _colorPalette =
        {
            Color.FromRgb(0, 0, 0),
            Color.FromRgb(255, 0, 0),
            Color.FromRgb(0, 255, 0),
            Color.FromRgb(0, 0, 255),
        };
    }
}
