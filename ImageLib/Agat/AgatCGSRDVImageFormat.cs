using System;
using FilLib;

namespace ImageLib.Agat
{
    /// Double Height Medium Resolution Color Graphics.
    /// <remarks>
    /// 128x256, 16 colors per pixel. This is a non-standard mode only available on modified hardware.
    /// </remarks>
    public class AgatCGSRDVImageFormat : C16ImageFormatAbstr
    {
        protected override int Width => 128;
        protected override int Height => 256;
        protected override ImageMeta.Mode MetaMode => ImageMeta.Mode.Agat_128_256_Pal16;

        protected override int GetLineOffset(int y)
        {
            int line = Math.DivRem(y, 2, out int bank);
            return (line + Height / 2 * bank) * BytesPerScanline;
        }
    }
}
