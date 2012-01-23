using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ImageLib
{
    public class Apple2DoublePixelTv : Apple2TvSet
    {
        /// <summary>
        /// Colors corresponding to each of the Apple2SimpleColor values.
        /// </summary>
        private Color[] palette;

        public double Aspect { get { return (4.0 / 140) / (3.0 / 192); } }

        public Apple2DoublePixelTv(Color[] palette)
        {
            this.palette = palette;
        }

        public Color[][] ProcessColors(Apple2SimpleColor[][] simpleColors)
        {
            int height = simpleColors.Length;
            int width = simpleColors[0].Length;

            Color[][] result = new Color[height][];
            for (int y = 0; y < height; ++y)
            {
                result[y] = new Color[width / 2];
                for (int x = 0; x < width; x += 2)
                {
                    result[y][x / 2] = GetPixel(simpleColors[y][x], simpleColors[y][x + 1]);
                }
            }

            return result;
        }

        private Color GetPixel(Apple2SimpleColor sc1, Apple2SimpleColor sc2)
        {
            if (sc1 == Apple2SimpleColor.Black)
            {
                return palette[(int)sc2];
            }
            if (sc2 == Apple2SimpleColor.Black)
            {
                return palette[(int)sc1];
            }
            return Colors.White;
        }
    }
}
