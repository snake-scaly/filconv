using System.Windows.Media;
using ImageLib.Util;

namespace ImageLib.Apple
{
    public class Apple2FillTv : Apple2TvSetAbstr
    {
        /// <summary>
        /// Colors corresponding to each of the Apple2SimpleColor values.
        /// </summary>
        private Color[] palette;

        public Apple2FillTv(Color[] palette)
        {
            this.palette = palette;
        }

        public override Color GetMiddleColor(Apple2SimpleColor left, Apple2SimpleColor middle, Apple2SimpleColor right)
        {
            if (middle == Apple2SimpleColor.Black)
            {
                if (left != Apple2SimpleColor.Black && right != Apple2SimpleColor.Black)
                {
                    return Apple2TvSetUtils.GetAverageColor(palette[(int)left], palette[(int)right]);
                }
                return Colors.Black;
            }

            if (left != Apple2SimpleColor.Black || right != Apple2SimpleColor.Black)
            {
                return Colors.White;
            }

            return palette[(int)middle];
        }

        public override Apple2SimpleColor GetBestMatch(Color color, bool isOdd)
        {
            return (Apple2SimpleColor)ColorUtils.BestMatch(color, palette);
        }

        protected override Color GetPixel(Apple2SimpleColor[][] simpleColors, int x, int y)
        {
            Apple2SimpleColor middle = Apple2TvSetUtils.GetSimplePixel(simpleColors, x, y);
            Apple2SimpleColor left = Apple2TvSetUtils.GetSimplePixel(simpleColors, x - 1, y);
            Apple2SimpleColor right = Apple2TvSetUtils.GetSimplePixel(simpleColors, x + 1, y);
            return GetMiddleColor(left, middle, right);
        }
    }
}
