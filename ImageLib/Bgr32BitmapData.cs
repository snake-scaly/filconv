namespace ImageLib
{
    public class Bgr32BitmapData
    {
        /// <summary>
        /// Pixel matrix in BGR32 format.
        /// </summary>
        /// <remarks>
        /// This is a rectangular matrix of <see cref="Height"/> lines, <see cref="Width"/>*4 bytes each.
        /// Order of bytes in each line is <c>B0, G0, R0, 0, B1, G1, R1, 0, ...</c>
        /// </remarks>
        public byte[] Pixels { get; }

        /// <summary>
        /// Bitmap width in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Bitmap height in pixels.
        /// </summary>
        public int Height { get; }

        public Bgr32BitmapData(byte[] pixels, int width, int height)
        {
            Pixels = pixels;
            Width = width;
            Height = height;
        }
    }
}
