using ImageLib.Util;

namespace ImageLib.Gamut
{
    public class SrgbGamut : IGamut
    {
        public Rgb FromSrgb(Rgb c)
        {
            return c;
        }
    }
}
