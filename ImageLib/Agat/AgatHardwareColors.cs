using ImageLib.ColorManagement;

namespace ImageLib.Agat
{
    // Colors that the standard Agat hardware is capable of producing on different standard displays.
    internal static class AgatHardwareColors
    {
        public static Rgb[] Color { get; } =
        {
            Rgb.FromRgb(0, 0, 0),
            Rgb.FromRgb(217, 0, 0),
            Rgb.FromRgb(0, 217, 0),
            Rgb.FromRgb(217, 217, 0),
            Rgb.FromRgb(0, 0, 217),
            Rgb.FromRgb(217, 0, 217),
            Rgb.FromRgb(0, 217, 217),
            Rgb.FromRgb(217, 217, 217),
            Rgb.FromRgb(38, 38, 38),
            Rgb.FromRgb(255, 38, 38),
            Rgb.FromRgb(38, 255, 38),
            Rgb.FromRgb(255, 255, 38),
            Rgb.FromRgb(38, 38, 255),
            Rgb.FromRgb(255, 38, 255),
            Rgb.FromRgb(38, 255, 255),
            Rgb.FromRgb(255, 255, 255),
        };

        public static Rgb[] Mono { get; } =
        {
            Rgb.FromRgb(0, 0, 0),
            Rgb.FromRgb(102, 102, 102),
            Rgb.FromRgb(68, 68, 68),
            Rgb.FromRgb(171, 171, 171),
            Rgb.FromRgb(47, 47, 47),
            Rgb.FromRgb(149, 149, 149),
            Rgb.FromRgb(115, 115, 115),
            Rgb.FromRgb(217, 217, 217),
            Rgb.FromRgb(38, 38, 38),
            Rgb.FromRgb(140, 140, 140),
            Rgb.FromRgb(106, 106, 106),
            Rgb.FromRgb(208, 208, 208),
            Rgb.FromRgb(84, 84, 84),
            Rgb.FromRgb(187, 187, 187),
            Rgb.FromRgb(153, 153, 153),
            Rgb.FromRgb(255, 255, 255),
        };
    }
}
