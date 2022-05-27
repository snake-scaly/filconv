using System;
using ImageLib.Util;

namespace ImageLib.Apple
{
    public class Apple2DoublePixelTv : Apple2TvSet
    {
        /// <summary>
        /// Colors corresponding to each of the Apple2SimpleColor values.
        /// </summary>
        private Rgb[] palette;

        public Apple2DoublePixelTv(Rgb[] palette)
        {
            this.palette = palette;
        }

        public Rgb[][] ProcessColors(Apple2SimpleColor[][] simpleColors)
        {
            int height = simpleColors.Length;
            int width = simpleColors[0].Length;

            Rgb[][] result = new Rgb[height][];
            for (int y = 0; y < height; ++y)
            {
                result[y] = new Rgb[width / 2];
                for (int x = 0; x < width; x += 2)
                {
                    result[y][x / 2] = GetPixel(simpleColors[y][x], simpleColors[y][x + 1]);
                }
            }

            return result;
        }

        public Rgb GetMiddleColor(Apple2SimpleColor left, Apple2SimpleColor middle, Apple2SimpleColor right)
        {
            throw new NotImplementedException();
        }

        public Apple2SimpleColor GetBestMatch(Rgb color, bool isOdd)
        {
            throw new NotImplementedException();
        }

        private Rgb GetPixel(Apple2SimpleColor sc1, Apple2SimpleColor sc2)
        {
            if (sc1 == Apple2SimpleColor.Black)
            {
                return palette[(int)sc2];
            }
            if (sc2 == Apple2SimpleColor.Black)
            {
                return palette[(int)sc1];
            }
            return palette[5];
        }
    }
}
