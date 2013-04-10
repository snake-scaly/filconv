using System.Windows.Media;

namespace ImageLib.Gamut
{
    public class SrgbGamut : IGamut
    {
        public Color FromSrgb(Color c)
        {
            return c;
        }
    }
}
