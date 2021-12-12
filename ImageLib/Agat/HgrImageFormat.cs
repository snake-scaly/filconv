using FilLib;

namespace ImageLib.Agat
{
    public class HgrImageFormat : BwImageFormatAbstr
    {
        public override int ComputeMatchScore(NativeImage native)
        {
            if (native.Metadata?.DisplayMode == ImageMeta.Mode.Agat_256_256_Mono)
                return NativeImageFormatUtils.MetaMatchScore;
            return base.ComputeMatchScore(native);
        }

        protected override int Width => 256;
        protected override int Height => 256;
    }
}
