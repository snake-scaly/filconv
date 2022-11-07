using ImageLib.Util;

namespace ImageLib.Apple
{
    public static class Apple2Palettes
    {
        public static Rgb[] American { get { return americanPalette; } }
        public static Rgb[] European { get { return europeanPalette; } }
        public static Rgb[] LoRes16 { get { return loRes16Palette; } }
        public static Rgb[] DoubleHiRes16 { get { return doubleHiRes16Palette; } }

        /// <summary>
        /// Construct all Apple's NTSC colors.
        /// </summary>
        /// <returns></returns>
        private static Rgb[] BuildAmerican16Palette(int phase)
        {
            var result = new Rgb[16];
            for (int i = 0; i < 16; i++)
            {
                result[i] = YIQColor.From4BitsStrict(i, phase).ToColor();
            }
            return result;
        }

        /// <summary>
        /// RGB colors corresponding to simple colors for American Apples
        /// </summary>
        private static readonly Rgb[] americanPalette =
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
        private static readonly Rgb[] europeanPalette =
        {
            Rgb.FromRgb(0, 0, 0),
            Rgb.FromRgb(0, 255, 0),
            Rgb.FromRgb(255, 0, 255),
            Rgb.FromRgb(0, 0, 255),
            Rgb.FromRgb(255, 0, 0),
            Rgb.FromRgb(255, 255, 255),
        };

        private static readonly Rgb[] loRes16Palette = BuildAmerican16Palette(0);
        private static readonly Rgb[] doubleHiRes16Palette = BuildAmerican16Palette(1);
    }
}
