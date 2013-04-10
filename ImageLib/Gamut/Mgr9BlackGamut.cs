using System;
using System.Windows.Media;

namespace ImageLib.Gamut
{
    public class Mgr9BlackGamut : IGamut
    {
        private const double srgbGamma = 2.2;

        public Color FromSrgb(Color c)
        {
            return Color.FromScRgb(c.ScA, Gc(c.ScR) / 2, Gc(c.ScG) / 2, Gc(c.ScB) / 2);
        }

        private float Gc(float c)
        {
            return (float)Math.Pow(c, srgbGamma);
        }
    }
}
