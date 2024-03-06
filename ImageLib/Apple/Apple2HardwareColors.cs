using System.Collections.Generic;
using System.Linq;
using ImageLib.ColorManagement;
using ImageLib.Util;

namespace ImageLib.Apple
{
    public static class Apple2HardwareColors
    {
        // taken from AppleWin
        private static readonly Rgb[] _rgbCardColors =
        {
            Rgb.FromRgb(0x00, 0x00, 0x00),
            Rgb.FromRgb(0x99, 0x03, 0x5F),
            Rgb.FromRgb(0x42, 0x04, 0xE1),
            Rgb.FromRgb(0xCA, 0x13, 0xFE),
            Rgb.FromRgb(0x00, 0x73, 0x10),
            Rgb.FromRgb(0x7F, 0x7F, 0x7F),
            Rgb.FromRgb(0x24, 0x97, 0xFF),
            Rgb.FromRgb(0xAA, 0xA2, 0xFF),
            Rgb.FromRgb(0x4F, 0x51, 0x01),
            Rgb.FromRgb(0xF0, 0x5C, 0x00),
            Rgb.FromRgb(0xBE, 0xBE, 0xBE),
            Rgb.FromRgb(0xFF, 0x85, 0xE1),
            Rgb.FromRgb(0x12, 0xCA, 0x07),
            Rgb.FromRgb(0xCE, 0xD4, 0x13),
            Rgb.FromRgb(0x51, 0xF5, 0x95),
            Rgb.FromRgb(0xFF, 0xFF, 0xFE),
        };

        public static Rgb[] HiRes { get; } = {
            _rgbCardColors[0],
            _rgbCardColors[3],
            _rgbCardColors[6],
            _rgbCardColors[12],
            _rgbCardColors[9],
            _rgbCardColors[15],
        };

        public static Palette LoRes16 { get; } = new Palette(_rgbCardColors);
        public static Palette DoubleHiRes16 { get; } = new Palette(DoubleHiResColors());
        public static Palette Monochrome { get; } = new Palette(new[] { default(Rgb), Rgb.FromRgb(255, 255, 255) });

        private static IEnumerable<Rgb> DoubleHiResColors() =>
            Enumerable.Range(0, 16).Select(i => _rgbCardColors[((i & 7) << 1) | ((i & 8) >> 3)]);
    }
}
