namespace ImageLib.Apple.HiRes
{
    interface AppleScreen
    {
        /// <summary>
        /// Get screen width, in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Get screen height, in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Get image pixels in native format.
        /// </summary>
        byte[] Pixels { get; }

        /// <summary>
        /// Get byte offset of the given line.
        /// </summary>
        /// <param name="lineIndex">line index counting from the top of the screen</param>
        /// <returns>Offset of the line in the Pixels array.</returns>
        int GetLineOffset(int lineIndex);
    }
}
