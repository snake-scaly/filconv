using System.Collections.Generic;
using System.Windows.Media;
using System;

namespace ImageLib
{
    static class ColorUtils
    {
        // YCbCr coefficients
        private const float Kr = 0.299f;
        private const float Kb = 0.114f;
        private const float Kg = 1 - Kr - Kb;

        public static int BestMatch(Color color, IList<Color> palette)
        {
            float closestMetric = float.PositiveInfinity;
            int closest = -1;

            int index = 0;
            foreach (var entry in palette)
            {
                float dx = (entry.ScR - color.ScR) * Kr;
                float dy = (entry.ScG - color.ScG) * Kg;
                float dz = (entry.ScB - color.ScB) * Kb;

                float d = dx * dx + dy * dy + dz * dz;

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
            float gray = Kr * color.ScR + Kg * color.ScG + Kb * color.ScB;
            return Color.FromScRgb(1, gray, gray, gray);
        }
    }
}
