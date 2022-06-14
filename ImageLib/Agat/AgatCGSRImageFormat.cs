using FilLib;

namespace ImageLib.Agat
{
    /// Medium Resolution Color Graphics (ЦГСР).
    /// <remarks>128x128, 16 colors per pixel.</remarks>
    public class AgatCGSRImageFormat : C16ImageFormatAbstr
    {
        protected override int Width => 128;
        protected override int Height => 128;
        protected override ImageMeta.Mode MetaMode => ImageMeta.Mode.Agat_128_128_Pal16;
    }
}
