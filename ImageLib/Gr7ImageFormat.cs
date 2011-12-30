using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageLib
{
    public class Gr7ImageFormat : C16ImageFormatAbstr
    {
        public override string Name
        {
            get { return "GR7"; }
        }

        public override int Width
        {
            get { return 64; }
        }

        public override int Height
        {
            get { return 64; }
        }
    }
}
