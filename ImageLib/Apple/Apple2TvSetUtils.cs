using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ImageLib.Apple
{
    static class Apple2TvSetUtils
    {
        /// <summary>
        /// Get a pixel from the raster safely.
        /// </summary>
        /// <param name="raster">pixels</param>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>
        /// A SimpleColor at the specified pixel, or SimpleColor.Black if
        /// x or y is out of bounds.
        /// </returns>
        public static Apple2SimpleColor GetSimplePixel(Apple2SimpleColor[][] raster, int x, int y)
        {
            if (y < 0 || y >= raster.Length || x < 0 || x >= raster[y].Length)
            {
                return Apple2SimpleColor.Black;
            }
            return raster[y][x];
        }

        public static Color GetAverageColor(Color c1, Color c2)
        {
            return Color.FromRgb((byte)((c1.R + c2.R) / 2), (byte)((c1.G + c2.G) / 2), (byte)((c1.B + c2.B) / 2));
        }
    }
}
