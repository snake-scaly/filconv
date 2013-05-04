using System.Collections.Generic;
using System.Windows.Media;
using System;

namespace ImageLib.Util
{
    static class ColorUtils
    {
        // YCbCr coefficients
        private const double Kr = 0.299;
        private const double Kb = 0.114;
        private const double Kg = 1 - Kr - Kb;

        public static double GetDistanceSq(Color c1, Color c2)
        {
            double dx = (c1.R - c2.R) * Kr;
            double dy = (c1.G - c2.G) * Kg;
            double dz = (c1.B - c2.B) * Kb;

            return dx * dx + dy * dy + dz * dz;
        }

        public static int BestMatch(Color color, IEnumerable<Color> palette)
        {
            double closestMetric = double.PositiveInfinity;
            int closest = -1;

            int index = 0;
            foreach (var entry in palette)
            {
                double d = GetDistanceSq(entry, color);

                if (d < closestMetric)
                {
                    closestMetric = d;
                    closest = index;
                }

                ++index;
            }

            return closest;
        }

        public static Color Desaturate(Color color)
        {
            byte gray = (byte)Math.Round(Kr * color.R + Kg * color.G + Kb * color.B);
            return Color.FromRgb(gray, gray, gray);
        }

        public static Color Pow(Color color, double gamma)
        {
            return Color.FromArgb(
                color.A,
                Pow(color.R, gamma),
                Pow(color.G, gamma),
                Pow(color.B, gamma));
        }

        private static byte Pow(byte c, double gamma)
        {
            return (byte)Math.Round(Math.Pow((c / 255.0), gamma) * 255);
        }
    }
}
