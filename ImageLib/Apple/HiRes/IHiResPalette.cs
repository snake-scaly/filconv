namespace ImageLib.Apple.HiRes
{
    public interface IHiResPalette
    {
        /// <summary>
        /// Find a HiRes-encoded byte that best matches the given seven RGB pixels.
        /// </summary>
        /// <param name="septet">Pixels to match.</param>
        /// <param name="odd">Whether the first pixel of the septet is in an odd screen column.</param>
        /// <param name="previousByte">Value of the previous byte in the line. Pass zero for the first septet.</param>
        /// <returns>The best match.</returns>
        byte Match(Septet septet, bool odd, byte previousByte);
    }
}
