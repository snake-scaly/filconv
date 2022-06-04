using ImageLib.Util;

namespace ImageLib.Apple.HiRes
{
    public abstract class Apple2TvSetAbstr : Apple2TvSet
    {
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

        protected abstract Rgb GetPixel(Apple2SimpleColor[][] simpleColors, int x, int y);
    }
}
