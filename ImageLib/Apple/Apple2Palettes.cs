using ImageLib.Util;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageLib.Apple
{
    public static class Apple2Palettes
    {
        private const double yiqHueShift = -15;

        public static Color[] American { get { return americanPalette; } }
        public static Color[] European { get { return europeanPalette; } }
        public static Color[] American16 { get { return american16Palette; } }

        /// <summary>
        /// Construct all Apple's NTSC colors.
        /// </summary>
        /// <returns></returns>
        private static Color[] BuildAmerican16Palette()
        {
            Color[] result = new Color[16];
            result[0] = Color.FromRgb(0, 0, 0);

            for (int i = 1; i < 16; i++)
            {
                int bitCount = (i & 5) + ((i >> 1) & 5);
                bitCount = (bitCount & 3) + ((bitCount >> 2) & 3);
                double x = ((i >> 0) & 1) - ((i >> 2) & 1); // [-1, 1]
                double y = ((i >> 3) & 1) - ((i >> 1) & 1); // [-1, 1]

                double h = Math.Atan2(y, x) / (2 * Math.PI) + yiqHueShift / 360;
                h -= Math.Floor(h); // Wrap to [0..1)
                double s = Math.Sqrt(x*x + y*y) / Math.Sqrt(2);
                double l = bitCount / 4.0; // [0, 1]

                HSLColor c = new HSLColor(h * 240, s * 240, l * 240);
                System.Drawing.Color dc = c;
                result[i] = Color.FromArgb(dc.A, dc.R, dc.G, dc.B);
            }

            return result;
        }

        /// <summary>
        /// RGB colors corresponding to simple colors for American Apples
        /// </summary>
        private static readonly Color[] americanPalette =
        {
            Color.FromRgb(0, 0, 0),
            Color.FromRgb(0, 255, 0),
            Color.FromRgb(255, 0, 255),
            Color.FromRgb(0, 85, 255),
            Color.FromRgb(255, 85, 0),
            Color.FromRgb(255, 255, 255),
        };

        /// <summary>
        /// RGB colors corresponding to simple colors for European Apples
        /// </summary>
        private static readonly Color[] europeanPalette =
        {
            Color.FromRgb(0, 0, 0),
            Color.FromRgb(0, 255, 0),
            Color.FromRgb(255, 0, 255),
            Color.FromRgb(0, 0, 255),
            Color.FromRgb(255, 0, 0),
            Color.FromRgb(255, 255, 255),
        };

        private static readonly Color[] american16Palette = BuildAmerican16Palette();
    }
}
