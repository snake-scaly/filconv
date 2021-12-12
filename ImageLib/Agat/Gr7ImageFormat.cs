using FilLib;

namespace ImageLib.Agat
{
    public class Gr7ImageFormat : C16ImageFormatAbstr
    {
        public override int ComputeMatchScore(NativeImage native)
        {
            if (native.Metadata?.DisplayMode == ImageMeta.Mode.Agat_64_64_Pal16)
                return NativeImageFormatUtils.MetaMatchScore;
            return base.ComputeMatchScore(native);
        }

        protected override int Width => 64;
        protected override int Height => 64;
    }
}
