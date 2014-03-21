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
            return ToByte(Math.Pow((c / 255.0), gamma));
        }

        /// <summary>
        /// Convert a color component from double to byte.
        /// </summary>
        /// <param name="v">color component. The value is clamped to the range [0, 1] before processing</param>
        /// <returns>A</returns>
        public static byte ToByte(double v)
        {
            return (byte)Math.Round(Clamp(v) * 255);
        }

        /// <summary>
        /// Convert an YIQ color to RGB.
        /// </summary>
        /// <param name="y">the Y (luma) component</param>
        /// <param name="i">the I (in-phase) component</param>
        /// <param name="q">the Q (quadrature) component</param>
        /// <returns>An RGB <see cref="Color"/>.</returns>
        public static Color ColorFromYiq(double y, double i, double q)
        {
            double r = y + 0.9563 * i + 0.6210 * q;
            double g = y - 0.2721 * i - 0.6474 * q;
            double b = y - 1.1070 * i + 1.7046 * q;
            return Color.FromRgb(ToByte(r), ToByte(g), ToByte(b));
        }

        /// <summary>
        /// Clamp a value to the range [0, 1].
        /// </summary>
        /// <param name="v">value to clamp</param>
        /// <returns><code>v</code> if <code>0≤v≤1</code>, 0 if <code>v&lt;0</code>,
        /// or 1 if <code>v&gt;1</code>.</returns>
        public static double Clamp(double v)
        {
            return Math.Max(0, Math.Min(1, v));
        }
    }
}
