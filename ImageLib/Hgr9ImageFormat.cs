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
    }
}
