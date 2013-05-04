using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageLib.Util
{
    public class BitmapPixels
    {
        private byte[] _pixels;
        private int _stride;

        /// <summary>
        /// Extract pixel data from a bitmap.
        /// </summary>
        /// <param name="bitmap">bitmap to get pixels from</param>
        public BitmapPixels(BitmapSource bitmap)
        {
            var bgr32 = new FormatConvertedBitmap(bitmap, PixelFormats.Bgr32, null, 0);

            Width = bgr32.PixelWidth;
            Height = bgr32.PixelHeight;
            _stride = Width * 4;

            _pixels = new byte[Height * _stride];
            bgr32.CopyPixels(_pixels, _stride, 0);
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Color GetPixel(int x, int y)
        {
            ValidateCoordinates(x, y);
            int offset = 4 * x + _stride * y;
            return Color.FromRgb(_pixels[offset + 2], _pixels[offset + 1], _pixels[offset]);
        }

        private void ValidateCoordinates(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x", x, "X must be between 0 and " + Width);
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y", y, "Y must be between 0 and " + Height);
        }
    }
}
