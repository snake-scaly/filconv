using ImageLib;
using ImageLib.ColorManagement;
using SkiaSharp;

namespace filconv
{
    public class BitmapPixels : IReadOnlyPixels
    {
        private readonly SKBitmap _bitmap;

        public int Width => _bitmap.Width;
        public int Height => _bitmap.Height;

        public BitmapPixels(SKBitmap bitmap)
        {
            _bitmap = bitmap;
        }

        public Rgb GetPixel(int x, int y)
        {
            var pixel = _bitmap.GetPixel(x, y);
            return Rgb.FromRgb(pixel.Red, pixel.Green, pixel.Blue);
        }
    }
}
