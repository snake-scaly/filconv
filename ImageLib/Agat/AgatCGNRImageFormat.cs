using FilLib;

namespace ImageLib.Agat
{
    /// Low Resolution Color Graphics (ЦГНР).
    /// <remarks>64x64, 16 colors per pixel. Only in Agat-7.</remarks>
    public class AgatCGNRImageFormat : C16ImageFormatAbstr
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
