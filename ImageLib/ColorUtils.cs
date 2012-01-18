using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace ImageLib
{
    static class ColorUtils
    {
        // YCbCr coefficients
        private const double Kr = 0.299;
        private const double Kb = 0.114;
        private const double Kg = 1 - Kr - Kb;

        public static int BestMatch(Color color, IList<Color> palette)
        {
            double closestMetric = double.PositiveInfinity;
            int closest = -1;

            int index = 0;
            foreach (var entry in palette)
            {
                double dx = (entry.R - color.R) * Kr;
                double dy = (entry.G - color.G) * Kg;
                double dz = (entry.B - color.B) * Kb;

                double d = dx * dx + dy * dy + dz * dz;

                if (d < closestMetric)
                {
                    closestMetric = d;
                    closest = index;
                }

                ++index;
            }

            return closest;
        }
    }
}
