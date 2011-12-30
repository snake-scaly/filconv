using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageLib
{
    public abstract class C16ImageFormatAbstr : AgatImageFormatAbstr
    {
        protected override int BitsPerPixel
        {
            get { return 4; }
        }

        protected override Color[] Palette
        {
            get { return _colorPalette; }
        }

        static readonly Color[] _colorPalette =
        {
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(217, 0, 0),
            Color.FromArgb(0, 217, 0),
            Color.FromArgb(217, 217, 0),
            Color.FromArgb(0, 0, 217),
            Color.FromArgb(217, 0, 217),
            Color.FromArgb(0, 217, 217),
            Color.FromArgb(217, 217, 217),
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(255, 0, 0),
            Color.FromArgb(0, 255, 0),
            Color.FromArgb(255, 255, 0),
            Color.FromArgb(0, 0, 255),
            Color.FromArgb(255, 0, 255),
            Color.FromArgb(0, 255, 255),
            Color.FromArgb(255, 255, 255),
        };

        static readonly Color[] _bwPalette =
        {
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(102, 102, 102),
            Color.FromArgb(68, 68, 68),
            Color.FromArgb(171, 171, 171),
            Color.FromArgb(47, 47, 47),
            Color.FromArgb(149, 149, 149),
            Color.FromArgb(115, 115, 115),
            Color.FromArgb(217, 217, 217),
            Color.FromArgb(38, 38, 38),
            Color.FromArgb(140, 140, 140),
            Color.FromArgb(106, 106, 106),
            Color.FromArgb(208, 208, 208),
            Color.FromArgb(84, 84, 84),
            Color.FromArgb(187, 187, 187),
            Color.FromArgb(153, 153, 153),
            Color.FromArgb(255, 255, 255),
        };
    }
}
