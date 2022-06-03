using System;
using System.Linq;
using FilLib;
using ImageLib.Util;

namespace ImageLib.Agat
{
    internal static class AgatColorUtils
    {
        public static Rgb[] NativeDisplayToColors(NativeDisplay display, ImageMeta meta)
        {
            switch (display)
            {
                case NativeDisplay.Color: return AgatPalettes.Color;
                case NativeDisplay.Mono: return AgatPalettes.Mono;
                case NativeDisplay.MonoA7: return AgatPalettes.Mono7;

                case NativeDisplay.Meta:
                    if (meta == null)
                        throw new InvalidOperationException("Cannot use Meta palette without meta");
                    return meta.CustomPalette.Select(UintToRgb).ToArray();

                default:
                    throw new ArgumentException($"Unsupported display {display:G}", nameof(display));
            }
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
