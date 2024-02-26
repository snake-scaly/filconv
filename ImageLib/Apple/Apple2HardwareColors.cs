using ImageLib.ColorManagement;
using ImageLib.Util;

namespace ImageLib.Apple
{
    public static class Apple2HardwareColors
    {
        public static Rgb[] American { get; } = {
            YIQColor.From4BitsStrict(0x0, 0).ToColor(),
            YIQColor.From4BitsStrict(0x3, 0).ToColor(),
            YIQColor.From4BitsStrict(0x6, 0).ToColor(),
            YIQColor.From4BitsStrict(0xC, 0).ToColor(),
            YIQColor.From4BitsStrict(0x9, 0).ToColor(),
            YIQColor.From4BitsStrict(0xF, 0).ToColor(),
        };

        public static Palette LoRes16 { get; } = BuildYiq16Palette(0);
        public static Palette DoubleHiRes16 { get; } = BuildYiq16Palette(1);

        /// <summary>
        /// Construct all Apple's NTSC colors.
        /// </summary>
        /// <returns></returns>
        private static Palette BuildYiq16Palette(int phase)
        {
            var result = new Rgb[16];
            for (int i = 0; i < 16; i++)
            {
                result[i] = YIQColor.From4BitsStrict(i, phase).ToColor();
            }
            return new Palette(result);
        }
    }
}
