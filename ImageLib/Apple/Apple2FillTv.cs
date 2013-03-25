﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

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

        protected override Color GetPixel(Apple2SimpleColor[][] simpleColors, int x, int y)
        {
            Apple2SimpleColor center = Apple2TvSetUtils.GetSimplePixel(simpleColors, x, y);
            Apple2SimpleColor left = Apple2TvSetUtils.GetSimplePixel(simpleColors, x - 1, y);
            Apple2SimpleColor right = Apple2TvSetUtils.GetSimplePixel(simpleColors, x + 1, y);

            if (center == Apple2SimpleColor.Black)
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

            return palette[(int)center];
        }
    }
}