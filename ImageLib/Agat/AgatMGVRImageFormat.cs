using FilLib;

namespace ImageLib.Agat
{
    /// High Resolution Monochrome Graphics (МГВР).
    /// <remarks>256x256, 2 colors per pixel.</remarks>
    public class AgatMGVRImageFormat : BwImageFormatAbstr
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
