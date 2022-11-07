using FilLib;

namespace ImageLib.Agat
{
    /// High Resolution Monochrome Graphics (МГВР).
    /// <remarks>256x256, 2 colors per pixel.</remarks>
    public class AgatMGVRImageFormat : BwImageFormatAbstr
    {
        protected override int Width => 256;
        protected override int Height => 256;
        protected override ImageMeta.Mode MetaMode => ImageMeta.Mode.Agat_256_256_Mono;
    }
}
