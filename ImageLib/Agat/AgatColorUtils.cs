using System;
using System.Linq;
using FilLib;
using ImageLib.ColorManagement;

namespace ImageLib.Agat
{
    internal static class AgatColorUtils
    {
        public static Rgb[] NativeDisplayToColors(NativeDisplay display, ImageMeta? meta)
        {
            switch (display)
            {
                case NativeDisplay.Color: return AgatHardwareColors.Color;
                case NativeDisplay.Mono: return AgatHardwareColors.Mono;

                case NativeDisplay.Meta:
                    if (meta == null)
                        throw new InvalidOperationException("Cannot use Meta palette without meta");
                    if (meta.CustomPalette == null)
                        throw new InvalidOperationException("Meta palette is null");
                    return meta.CustomPalette.Select(UintToRgb).ToArray();

                default:
                    throw new ArgumentException($"Unsupported display {display:G}", nameof(display));
            }
        }

        public static uint RgbToUint(Rgb c)
        {
            return ((uint)c.R << 16) | ((uint)c.G << 8) | c.B;
        }

        private static Rgb UintToRgb(uint i)
        {
            var r = (byte)(i >> 16);
            var g = (byte)(i >> 8);
            var b = (byte)(i >> 0);
            return Rgb.FromRgb(r, g, b);
        }
    }
}
