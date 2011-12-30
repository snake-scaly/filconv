using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageLib
{
    public abstract class BwImageFormatAbstr : AgatImageFormatAbstr
    {
        protected override int BitsPerPixel
        {
            get { return 1; }
        }

        protected override System.Drawing.Color[] Palette
        {
            get { return new Color[] { Color.FromArgb(0, 0, 0), Color.FromArgb(255, 255, 255) }; }
        }
    }
}
