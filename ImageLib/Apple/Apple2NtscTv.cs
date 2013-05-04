using System;
using System.Windows.Media;

namespace ImageLib.Apple
{
    public class Apple2NtscTv : Apple2TvSetAbstr
    {
        /// <summary>
        /// Colors corresponding to each of the Apple2SimpleColor values.
        /// </summary>
        private Color[] palette;

        public Apple2NtscTv(Color[] palette)
        {
            this.palette = palette;
        }

        public override Color GetMiddleColor(Apple2SimpleColor left, Apple2SimpleColor middle, Apple2SimpleColor right)
        {
            throw new NotImplementedException();
        }

        protected override Color GetPixel(Apple2SimpleColor[][] simpleColors, int x, int y)
        {
            // Lines are shifted relative to each other by one pixel
            int shift = (y & 1) * 2 - 1;

            Apple2SimpleColor center = Apple2TvSetUtils.GetSimplePixel(simpleColors, x, y);
            Apple2SimpleColor left = Apple2TvSetUtils.GetSimplePixel(simpleColors, x - 1, y);
            Apple2SimpleColor right = Apple2TvSetUtils.GetSimplePixel(simpleColors, x + 1, y);
            Apple2SimpleColor above = Apple2TvSetUtils.GetSimplePixel(simpleColors, x + shift, y - 1);
            Apple2SimpleColor below = Apple2TvSetUtils.GetSimplePixel(simpleColors, x + shift, y + 1);

            Color c = CombineColors(center, left, right, above, below);
            return c;
        }

        // http://www.applefritter.com/node/23880
        private Color CombineColors(
            Apple2SimpleColor center,
            Apple2SimpleColor left,
            Apple2SimpleColor right,
            Apple2SimpleColor above,
            Apple2SimpleColor below)
        {
            if (center == Apple2SimpleColor.Black)
            {
                // Black is replaced by whatever non-black above or below.
                // If both are non-black then replace with an average of
                // the two.
                return Apple2TvSetUtils.GetAverageColor(palette[(int)above], palette[(int)below]);
            }

            // If any of the surrounding colors is a complement color
            // then return white.  Otherwise return the color itself.
            if (IsComplement(center, left) || IsComplement(center, right) ||
                IsComplement(center, above) || IsComplement(center, below))
            {
                return Colors.White;
            }

            return palette[(int)center];
        }

        private static bool IsComplement(Apple2SimpleColor a, Apple2SimpleColor b)
        {
            return a == Apple2SimpleColor.Green && b == Apple2SimpleColor.Violet ||
                a == Apple2SimpleColor.Violet && b == Apple2SimpleColor.Green ||
                a == Apple2SimpleColor.Orange && b == Apple2SimpleColor.Blue ||
                a == Apple2SimpleColor.Blue && b == Apple2SimpleColor.Orange;
        }
    }
}
