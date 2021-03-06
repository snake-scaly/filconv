using ImageLib.Util;

namespace ImageLib.Apple.HiRes
{
    public class Apple2FillTv : Apple2TvSetAbstr
    {
        /// <summary>
        /// Colors corresponding to each of the Apple2SimpleColor values.
        /// </summary>
        private Rgb[] palette;

        public Apple2FillTv(Rgb[] palette)
        {
            this.palette = palette;
        }

        public override Rgb GetMiddleColor(Apple2SimpleColor left, Apple2SimpleColor middle, Apple2SimpleColor right)
        {
            if (middle == Apple2SimpleColor.Black)
            {
                if (left != Apple2SimpleColor.Black && right != Apple2SimpleColor.Black)
                {
                    return Apple2TvSetUtils.GetAverageColor(palette[(int)left], palette[(int)right]);
                }
                return palette[0];
            }

            if (left != Apple2SimpleColor.Black || right != Apple2SimpleColor.Black)
            {
                return palette[5];
            }

            return palette[(int)middle];
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
