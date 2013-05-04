﻿using System;
using System.Windows.Media;

namespace ImageLib.Apple
{
    public abstract class Apple2TvSetAbstr : Apple2TvSet
    {
        public double Aspect { get { return (4.0 / 280) / (3.0 / 192); } }

        public Color[][] ProcessColors(Apple2SimpleColor[][] simpleColors)
        {
            int height = simpleColors.Length;
            int width = simpleColors[0].Length;

            Color[][] result = new Color[height][];

            for (int y = 0; y < height; ++y)
            {
                result[y] = new Color[width];
                for (int x = 0; x < width; ++x)
                {
                    result[y][x] = GetPixel(simpleColors, x, y);
                }
            }

            return result;
        }

        public abstract Color GetMiddleColor(Apple2SimpleColor left, Apple2SimpleColor middle, Apple2SimpleColor right);

        public virtual Apple2SimpleColor GetBestMatch(Color color, bool isOdd)
        {
            throw new NotImplementedException();
        }

        protected abstract Color GetPixel(Apple2SimpleColor[][] simpleColors, int x, int y);
    }
}
