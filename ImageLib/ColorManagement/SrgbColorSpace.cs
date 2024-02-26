using System;
using ImageLib.Util;

namespace ImageLib.ColorManagement
{
    public class SrgbColorSpace
    {
        public Rgb FromXyz(XyzColor xyz)
        {
            // https://en.wikipedia.org/wiki/SRGB#From_CIE_XYZ_to_sRGB

            var rl = 3.2406 * xyz.X - 1.5372 * xyz.Y - 0.4986 * xyz.Z;
            var gl = -0.9689 * xyz.X + 1.8752 * xyz.Y + 0.0415 * xyz.Z;
            var bl = 0.0557 * xyz.X - 0.2040 * xyz.Y + 1.0570 * xyz.Z;

            byte FromLinear(double x)
            {
                var l = x <= 0.0031308 ? 12.92 * x : 1.055 * Math.Pow(x, 1 / 2.4) - 0.055;
                return (byte)Math.Round(ColorUtils.Clamp(l, 0, 1) * 255);
            }

            return new Rgb
            {
                R = FromLinear(rl),
                G = FromLinear(gl),
                B = FromLinear(bl),
            };
        }

        public XyzColor ToXyz(Rgb sRgb)
        {
            // https://en.wikipedia.org/wiki/SRGB#From_sRGB_to_CIE_XYZ

            double ToLinear(double x) => x <= 0.04045 ? x / 12.92 : Math.Pow((x + 0.055) / 1.055, 2.4);

            var rl = ToLinear(sRgb.R / 255.0);
            var gl = ToLinear(sRgb.G / 255.0);
            var bl = ToLinear(sRgb.B / 255.0);

            return new XyzColor
            {
                X = 0.4124 * rl + 0.3576 * gl + 0.1805 * bl,
                Y = 0.2126 * rl + 0.7152 * gl + 0.0722 * bl,
                Z = 0.0193 * rl + 0.1192 * gl + 0.9505 * bl,
            };
        }
    }
}
