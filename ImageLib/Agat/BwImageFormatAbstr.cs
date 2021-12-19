using System.Windows.Media;
using FilLib;
using ImageLib.Gamut;

namespace ImageLib.Agat
{
    public abstract class BwImageFormatAbstr : AgatImageFormatAbstr
    {
        protected override int BitsPerPixel => 1;

        protected override IGamut Gamut { get; } = new BlackAndWhiteGamut();

        protected override Color[] GetStandardPalette(ImageMeta.Palette variant) => _palette;

        private static readonly Color[] _palette =
        {
            Color.FromRgb(0, 0, 0),
            Color.FromRgb(255, 255, 255)
        };
    }
}
