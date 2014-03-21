using ImageLib.Util;
using System.Windows.Media;

namespace ImageLib.Apple
{
    public static class Apple2Palettes
    {
        public static Color[] American { get { return americanPalette; } }
        public static Color[] European { get { return europeanPalette; } }
        public static Color[] LoRes16 { get { return loRes16Palette; } }
        public static Color[] DoubleHiRes16 { get { return doubleHiRes16Palette; } }

        /// <summary>
        /// Construct all Apple's NTSC colors.
        /// </summary>
        /// <returns></returns>
        private static Color[] BuildAmerican16Palette(int phase)
        {
            var result = new Color[16];
            for (int i = 0; i < 16; i++)
            {
                result[i] = YIQColor.From4BitsStrict(i, phase).ToColor();
            }
            return result;
        }

        /// <summary>
        /// RGB colors corresponding to simple colors for American Apples
        /// </summary>
        private static readonly Color[] americanPalette =
        {
            YIQColor.From4BitsStrict(0x0, 0).ToColor(),
            YIQColor.From4BitsStrict(0xC, 0).ToColor(),
            YIQColor.From4BitsStrict(0x3, 0).ToColor(),
            YIQColor.From4BitsStrict(0x6, 0).ToColor(),
            YIQColor.From4BitsStrict(0x9, 0).ToColor(),
            YIQColor.From4BitsStrict(0xF, 0).ToColor(),
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

        private static readonly Color[] loRes16Palette = BuildAmerican16Palette(0);
        private static readonly Color[] doubleHiRes16Palette = BuildAmerican16Palette(1);
    }
}
