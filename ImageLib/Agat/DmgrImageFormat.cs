using System;

namespace ImageLib.Agat
{
    public class DmgrImageFormat : C16ImageFormatAbstr
    {
        protected override int Width => 128;

        protected override int Height => 256;

        public override double Aspect => 8.0 / 3.0;

        protected override int GetLineOffset(int y)
        {
            int line = Math.DivRem(y, 2, out int bank);
            return (line + Height / 2 * bank) * BytesPerScanline;
        }
    }
}
