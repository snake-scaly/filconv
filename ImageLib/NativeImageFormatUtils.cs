using System;

namespace ImageLib
{
    static class NativeImageFormatUtils
    {
        public const int BestSizeMatchScore = 0x10000;
        public const int NameMatchScore = 0x100000;
        public const int MetaMatchScore = 0x1000000;

        public static int ComputeMatch(NativeImage native, int expectedSize, string nameFragment = null)
        {
            int score = BestSizeMatchScore - Math.Abs(native.Data.Length - expectedSize);

            if (nameFragment != null && native.FormatHint.FileName?.ContainsIgnoreCase(nameFragment) == true)
                score += NameMatchScore;

            return score;
        }
    }
}
