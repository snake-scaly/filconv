using ImageLib.Util;

namespace ImageLib.Gamut
{
    public class Mgr9BlackGamut : IGamut
    {
        private const double srgbGamma = 2.2;

        public Rgb FromSrgb(Rgb c)
        {
            return ColorUtils.Gamma(c, srgbGamma);
        }
    }
}
