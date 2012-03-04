using System.Windows.Media;

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
            get { return new Color[] { Color.FromRgb(0, 0, 0), Color.FromRgb(255, 255, 255) }; }
        }
    }
}
