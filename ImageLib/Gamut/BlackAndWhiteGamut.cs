using ImageLib.Util;

namespace ImageLib.Gamut
{
    public class BlackAndWhiteGamut : IGamut
    {
        private const double srgbGamma = 2.2;

        public Rgb FromSrgb(Rgb c)
        {
            c = ColorUtils.Desaturate(ColorUtils.Gamma(c, srgbGamma));
            return Rgb.FromRgb(c.G, c.G, c.G);
        }
    }
}
