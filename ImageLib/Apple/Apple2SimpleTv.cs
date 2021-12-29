using System.Linq;
using ImageLib.Util;

namespace ImageLib.Apple
{
    public class Apple2SimpleTv : Apple2TvSetAbstr
    {
        /// <summary>
        /// Colors corresponding to each of the Apple2SimpleColor values.
        /// </summary>
        private Rgb[] palette;

        public Apple2SimpleTv(Rgb[] palette)
        {
            this.palette = palette;
        }

        public override Rgb GetMiddleColor(Apple2SimpleColor left, Apple2SimpleColor middle, Apple2SimpleColor right)
        {
            if (middle != Apple2SimpleColor.Black && (left != Apple2SimpleColor.Black || right != Apple2SimpleColor.Black))
            {
                return palette[5];
            }
            return palette[(int)middle];
        }

        public override Apple2SimpleColor GetBestMatch(Rgb color, bool isOdd)
        {
            Apple2SimpleColor[] columnColors;
            if (isOdd)
            {
                columnColors = new Apple2SimpleColor[]
                {
                    Apple2SimpleColor.Black,
                    Apple2SimpleColor.Green,
                    Apple2SimpleColor.Blue,
                    Apple2SimpleColor.White,
                };
            }
            else
            {
                columnColors = new Apple2SimpleColor[]
                {
                    Apple2SimpleColor.Black,
                    Apple2SimpleColor.Violet,
                    Apple2SimpleColor.Orange,
                    Apple2SimpleColor.White,
                };
            }

            var columnPalette = columnColors.Select(sc => palette[(int)sc]);
            return columnColors[ColorUtils.BestMatch(color, columnPalette)];
        }

        protected override Rgb GetPixel(Apple2SimpleColor[][] simpleColors, int x, int y)
        {
            Apple2SimpleColor middle = Apple2TvSetUtils.GetSimplePixel(simpleColors, x, y);
            Apple2SimpleColor left = Apple2TvSetUtils.GetSimplePixel(simpleColors, x - 1, y);
            Apple2SimpleColor right = Apple2TvSetUtils.GetSimplePixel(simpleColors, x + 1, y);
            return GetMiddleColor(left, middle, right);
        }
    }
}
