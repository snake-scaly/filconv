using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageLib.Common;
using ImageLib.Util;

namespace ImageLib.Apple
{
    public sealed class MonoPictureBuilder : IBitStreamPictureBuilder
    {
        private const int _width = 560;
        private const int _height = 192;

        private readonly WriteableBitmap _bitmap;

        public MonoPictureBuilder()
        {
            _bitmap = new WriteableBitmap(
                _width,
                _height,
                Constants.Dpi,
                Constants.Dpi,
                PixelFormats.Bgr32,
                null);
        }

        public IScanlineWriter GetScanlineWriter(int index)
        {
            return new ScanlineWriter(_bitmap, index);
        }

        public BitmapSource Build()
        {
            return _bitmap;
        }

        private class ScanlineWriter : IScanlineWriter
        {
            private readonly WriteableBitmap _bitmap;
            private readonly int _scanline;
            private readonly byte[] _pixels;
            private readonly Rgb[] _palette;
            private int _pos;

            public ScanlineWriter(WriteableBitmap bitmap, int scanline)
            {
                _bitmap = bitmap;
                _scanline = scanline;
                _pixels = new byte[_bitmap.PixelWidth * 4];
                _palette = new[] { Rgb.FromRgb(0, 0, 0), Rgb.FromRgb(255, 255, 255) };
            }

            public void Write(int bit)
            {
                if (_pos >= _pixels.Length)
                    return;
                var c = _palette[bit & 1];
                _pixels[_pos++] = c.B;
                _pixels[_pos++] = c.G;
                _pixels[_pos++] = c.R;
                _pos++;
            }

            public void Dispose()
            {
                _bitmap.WritePixels(new Int32Rect(0, _scanline, _bitmap.PixelWidth, 1), _pixels, _pixels.Length, 0);
            }
        }
    }
}
