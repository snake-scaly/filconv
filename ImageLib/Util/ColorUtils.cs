using System.Collections.Generic;
using System;
using ImageLib.ColorManagement;

namespace ImageLib.Util
{
    static class ColorUtils
    {
        // YCbCr coefficients
        private const double Kr = 0.299;
        private const double Kb = 0.114;
        private const double Kg = 1 - Kr - Kb;

        public static double GetDistanceSq(Rgb c1, Rgb c2)
        {
            double dx = (c1.R - c2.R) * Kr;
            double dy = (c1.G - c2.G) * Kg;
            double dz = (c1.B - c2.B) * Kb;

            return dx * dx + dy * dy + dz * dz;
        }

        public static int BestMatch(Rgb color, IEnumerable<Rgb> palette)
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

        /// <summary>
        /// Convert a color component from double to byte.
        /// </summary>
        /// <remarks>
        /// Double values [0,1] are mapped to byte values [0,255]. Values
        /// below 0 and above 1 are mapped to 0 and 255 respectively.
        /// </remarks>
        public static byte FromDouble(double value)
        {
            return ClampByte((int)Math.Round(value * 255));
        }

        /// Clamp an integer to the specified interval.
        public static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// Clamp a float to the specified interval.
        public static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        /// Clamp a double to the specified interval.
        public static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public static byte ClampByte(int value)
        {
            return (byte)Clamp(value, 0, 255);
        }

        /// <summary>
        /// Convert an YIQ color to RGB.
        /// </summary>
        /// <param name="y">the Y (luma) component</param>
        /// <param name="i">the I (in-phase) component</param>
        /// <param name="q">the Q (quadrature) component</param>
        /// <returns>An RGB <see cref="Rgb"/>.</returns>
        public static Rgb ColorFromYiq(double y, double i, double q)
        {
            double r = y + 0.9563 * i + 0.6210 * q;
            double g = y - 0.2721 * i - 0.6474 * q;
            double b = y - 1.1070 * i + 1.7046 * q;
            return Rgb.FromRgb(FromDouble(r), FromDouble(g), FromDouble(b));
        }
    }
}
