using System;
using FilLib;
using ImageLib.Gamut;

namespace ImageLib.Agat
{
    /// High Resolution Color Graphics (ЦГВР).
    /// <remarks>256x256, 4 colors per pixel. Only in Agat-9.</remarks>
    public class AgatCGVRImageFormat : AgatImageFormatAbstr
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

        protected override int MapColorIndexNativeToStandard(int index, int palette)
        {
            return _nativeToStandardColorMap[palette][index];
        }

        static readonly int[][] _nativeToStandardColorMap =
        {
            new[] { 0, 1, 2, 4 },
            new[] { 15, 1, 2, 4 },
            new[] { 0, 0, 2, 4 },
            new[] { 0, 1, 0, 4 },
        };
    }
}
