using System;
using ImageLib.Util;

namespace ImageLib.Apple
{
    public abstract class Apple2TvSetAbstr : Apple2TvSet
    {
        public double Aspect { get { return (4.0 / 280) / (3.0 / 192); } }

        public Rgb[][] ProcessColors(Apple2SimpleColor[][] simpleColors)
        {
            int height = simpleColors.Length;
            int width = simpleColors[0].Length;

            Rgb[][] result = new Rgb[height][];

            for (int y = 0; y < height; ++y)
            {
                result[y] = new Rgb[width];
                for (int x = 0; x < width; ++x)
                {
                    result[y][x] = GetPixel(simpleColors, x, y);
                }
            }

            return result;
        }

        public abstract Rgb GetMiddleColor(Apple2SimpleColor left, Apple2SimpleColor middle, Apple2SimpleColor right);

        public virtual Apple2SimpleColor GetBestMatch(Rgb color, bool isOdd)
        {
            throw new NotImplementedException();
        }

        protected abstract Rgb GetPixel(Apple2SimpleColor[][] simpleColors, int x, int y);
    }
}
