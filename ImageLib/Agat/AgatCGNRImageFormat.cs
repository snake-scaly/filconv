using FilLib;

namespace ImageLib.Agat
{
    /// Low Resolution Color Graphics (ЦГНР).
    /// <remarks>64x64, 16 colors per pixel. Only in Agat-7.</remarks>
    public class AgatCGNRImageFormat : C16ImageFormatAbstr
    {
        protected override int Width => 64;
        protected override int Height => 64;
        protected override ImageMeta.Mode MetaMode => ImageMeta.Mode.Agat_64_64_Pal16;
    }
}
