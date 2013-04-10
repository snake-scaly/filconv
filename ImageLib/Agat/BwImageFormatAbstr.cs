using System.Windows.Media;
using ImageLib.Gamut;

namespace ImageLib.Agat
{
    public abstract class BwImageFormatAbstr : AgatImageFormatAbstr
    {
        protected override int BitsPerPixel
        {
            get { return 1; }
        }

        protected override Color[] Palette
        {
            get { return _palette; }
        }

        protected override IGamut Gamut
        {
            get { return _gamut; }
        }

        private static readonly Color[] _palette = new Color[]
        {
            Color.FromRgb(0, 0, 0),
            Color.FromRgb(255, 255, 255)
        };

        private static readonly IGamut _gamut = new BlackAndWhiteGamut();
    }
}
