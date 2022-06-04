namespace ImageLib.Apple
{
    internal static class Apple2HiResUtils
    {
        /// <summary>
        /// Calculate direct pixel color based on its characteristics.
        /// </summary>
        /// <param name="pixelBit">state of the pixel bit itself</param>
        /// <param name="shiftBit">state of the most significant bit in the pixel's byte</param>
        /// <param name="isOdd">whether the horizontal screen position of the pixel is odd</param>
        /// <returns>One of the <see cref="Apple2SimpleColor"/> values.</returns>
        public static Apple2SimpleColor GetPixelColor(bool pixelBit, bool shiftBit, bool isOdd)
        {
            if (!pixelBit)
            {
                return Apple2SimpleColor.Black;
            }
            if (isOdd)
            {
                return shiftBit ? Apple2SimpleColor.Orange : Apple2SimpleColor.Violet;
            }
            return shiftBit ? Apple2SimpleColor.Blue : Apple2SimpleColor.Green;
        }
    }
}
