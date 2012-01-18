using System.Windows.Media;

namespace ImageLib
{
    public abstract class C16ImageFormatAbstr : AgatImageFormatAbstr
    {
        protected override int BitsPerPixel
        {
            get { return 4; }
        }

        protected override Color[] Palette
        {
            get { return _colorPalette; }
        }

        static readonly Color[] _colorPalette =
        {
            Color.FromRgb(0, 0, 0),
            Color.FromRgb(217, 0, 0),
            Color.FromRgb(0, 217, 0),
            Color.FromRgb(217, 217, 0),
            Color.FromRgb(0, 0, 217),
            Color.FromRgb(217, 0, 217),
            Color.FromRgb(0, 217, 217),
            Color.FromRgb(217, 217, 217),
            Color.FromRgb(38, 38, 38),
            Color.FromRgb(255, 0, 0),
            Color.FromRgb(0, 255, 0),
            Color.FromRgb(255, 255, 0),
            Color.FromRgb(0, 0, 255),
            Color.FromRgb(255, 0, 255),
            Color.FromRgb(0, 255, 255),
            Color.FromRgb(255, 255, 255),
        };

        static readonly Color[] _bwPalette =
        {
            Color.FromRgb(0, 0, 0),
            Color.FromRgb(102, 102, 102),
            Color.FromRgb(68, 68, 68),
            Color.FromRgb(171, 171, 171),
            Color.FromRgb(47, 47, 47),
            Color.FromRgb(149, 149, 149),
            Color.FromRgb(115, 115, 115),
            Color.FromRgb(217, 217, 217),
            Color.FromRgb(38, 38, 38),
            Color.FromRgb(140, 140, 140),
            Color.FromRgb(106, 106, 106),
            Color.FromRgb(208, 208, 208),
            Color.FromRgb(84, 84, 84),
            Color.FromRgb(187, 187, 187),
            Color.FromRgb(153, 153, 153),
            Color.FromRgb(255, 255, 255),
        };
    }
}
