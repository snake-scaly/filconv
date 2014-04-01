using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageLib
{
    static class NativeImageFormatUtils
    {
        public const int BestSizeMatchScore = 0x10000;
        public const int NameMatchScore = 0x100000;

        public static int ComputeMatch(NativeImage native, int expectedSize, string nameFragment = null)
        {
            int score = BestSizeMatchScore - Math.Abs(native.Data.Length - expectedSize);
            if (nameFragment != null)
            {
                score += native.FormatHint.FileNameContains(nameFragment) ? NameMatchScore : 0;
            }
            return score;
        }
    }
}
