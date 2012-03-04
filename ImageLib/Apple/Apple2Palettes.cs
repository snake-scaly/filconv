using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ImageLib.Apple
{
    public static class Apple2Palettes
    {
        public static Color[] American { get { return americanPalette; } }
        public static Color[] European { get { return europeanPalette; } }

        /// <summary>
        /// RGB colors corresponding to simple colors for American Apples
        /// </summary>
        private static readonly Color[] americanPalette =
        {
            Color.FromRgb(0, 0, 0),
            Color.FromRgb(0, 255, 0),
            Color.FromRgb(255, 0, 255),
            Color.FromRgb(0, 128, 255),
            Color.FromRgb(255, 128, 0),
        };

        /// <summary>
        /// RGB colors corresponding to simple colors for European Apples
        /// </summary>
        private static readonly Color[] europeanPalette =
        {
            Color.FromRgb(0, 0, 0),
            Color.FromRgb(0, 255, 0),
            Color.FromRgb(255, 0, 255),
            Color.FromRgb(0, 0, 255),
            Color.FromRgb(255, 0, 0),
        };
    }
}
