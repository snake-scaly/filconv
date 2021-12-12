using FilLib;

namespace ImageLib.Agat
{
    public class MgrImageFormat : C16ImageFormatAbstr
    {
        public override int ComputeMatchScore(NativeImage native)
        {
            if (native.Metadata?.DisplayMode == ImageMeta.Mode.Agat_128_128_Pal16)
                return NativeImageFormatUtils.MetaMatchScore;
            return base.ComputeMatchScore(native);
        }

        protected override int Width => 128;
        protected override int Height => 128;
    }
}
