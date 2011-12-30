using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageLib
{
    public class Hgr9ImageFormat : BwImageFormatAbstr
    {
        public override string Name
        {
            get { return "HGR9"; }
        }

        public override int Width
        {
            get { return 512; }
        }

        public override int Height
        {
            get { return 256; }
        }

        public override double Aspect
        {
            get { return 2.0 / 3.0; }
        }

        protected override int GetLineOffset(int y)
        {
            int bank;
            int line = Math.DivRem(y, 2, out bank);
            return (line + Height / 2 * bank) * BytesPerScanline;
        }
    }
}
