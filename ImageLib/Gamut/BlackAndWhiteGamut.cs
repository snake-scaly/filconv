using System;
using System.Windows.Media;

namespace ImageLib.Gamut
{
    public class BlackAndWhiteGamut : IGamut
    {
        private const double srgbGamma = 2.2;

        public Color FromSrgb(Color c)
        {
            c = ColorUtils.Desaturate(c);
            float corrected = (float)Math.Pow(c.ScG, srgbGamma);
            return Color.FromScRgb(1, corrected, corrected, corrected);
        }
    }
}
