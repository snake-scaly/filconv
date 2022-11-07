using System;
using System.Collections.Generic;
using System.Drawing;
using ImageLib.Util;

namespace ImageLib.Debug
{
    public static class DebugUtils
    {
        public static Image PalToImage(IEnumerable<Rgb> palette)
        {
            var b = new Bitmap(64 * 4, 48 * 4);
            using (var g = Graphics.FromImage(b))
            {
                var i = 0;
                foreach (var c in palette)
                {
                    var y = Math.DivRem(i, 4, out var x);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(c.R, c.G, c.B)), x * 64, y * 48, 64, 48);
                    i++;
                }
            }
            return b;
        }

        public static Image RGScatter(IEnumerable<Rgb> pixels)
        {
            var b = new Bitmap(256, 256);
            foreach (var p in pixels)
                b.SetPixel(p.R, 255 - p.G, Color.FromArgb(p.R, p.G, 0));
            return b;
        }

        public static Image RBScatter(IEnumerable<Rgb> pixels)
        {
            var b = new Bitmap(256, 256);
            foreach (var p in pixels)
                b.SetPixel(p.R, 255 - p.B, Color.FromArgb(p.R, 0, p.B));
            return b;
        }
    }
}
