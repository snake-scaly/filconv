using System;
using System.Windows.Media;
using ImageLib.Util;

namespace ImageLib.Gamut
{
    public class BlackAndWhiteGamut : IGamut
    {
        private const double srgbGamma = 2.2;

        public Color FromSrgb(Color c)
        {
            c = ColorUtils.Desaturate(ColorUtils.Pow(c, srgbGamma));
            return Color.FromArgb(c.A, c.G, c.G, c.G);
        }
    }
}
