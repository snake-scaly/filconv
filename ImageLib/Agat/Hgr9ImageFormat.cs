using System;
using FilLib;

namespace ImageLib.Agat
{
    public class Hgr9ImageFormat : BwImageFormatAbstr
    {
        public override double Aspect => 2.0 / 3.0;

        public override int ComputeMatchScore(NativeImage native)
        {
            if (native.Metadata?.DisplayMode == ImageMeta.Mode.Agat_512_256_Mono)
                return NativeImageFormatUtils.MetaMatchScore;
            return base.ComputeMatchScore(native);
        }

        protected override int Width => 512;
        protected override int Height => 256;

        protected override int GetLineOffset(int y)
        {
            int bank;
            int line = Math.DivRem(y, 2, out bank);
            return (line + Height / 2 * bank) * BytesPerScanline;
        }
    }
}
