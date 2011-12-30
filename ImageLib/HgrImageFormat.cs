using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageLib
{
    public class HgrImageFormat : BwImageFormatAbstr
    {
        public override string Name
        {
            get { return "HGR"; }
        }

        public override int Width
        {
            get { return 256; }
        }

        public override int Height
        {
            get { return 256; }
        }
    }
}
