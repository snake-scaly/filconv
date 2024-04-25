using System;
using ImageLib.ColorManagement;

namespace ImageLib.Apple.BitStream
{
    public sealed class MonoPictureBuilder : IBitStreamPictureBuilder
    {
        private const int _width = 560;
        private const int _height = 192;

        private readonly Bgr32BitmapData _bitmap;

        public MonoPictureBuilder()
        {
            var pixels = new byte[_width * _height * 4];
            _bitmap = new Bgr32BitmapData(pixels, _width, _height);
        }

        public IScanlineWriter GetScanlineWriter(int index)
        {
            return new ScanlineWriter(_bitmap, index);
        }

        public Bgr32BitmapData Build()
        {
            return _bitmap;
        }

        private class ScanlineWriter : IScanlineWriter
        {
            private static readonly Rgb[] _palette = { Rgb.FromRgb(0, 0, 0), Rgb.FromRgb(255, 255, 255) };

            private readonly Bgr32BitmapData _bitmap;
            private readonly int _end;
            private int _pos;

            public ScanlineWriter(Bgr32BitmapData bitmap, int scanline)
            {
                _bitmap = bitmap;
                _pos = scanline * _bitmap.Width * 4;
                _end = _pos + _bitmap.Width * 4;
            }

            public void Write(int bit)
            {
                if (_pos >= _end)
                    return;
                var c = _palette[bit & 1];
                _bitmap.Pixels[_pos++] = c.B;
                _bitmap.Pixels[_pos++] = c.G;
                _bitmap.Pixels[_pos++] = c.R;
                _bitmap.Pixels[_pos++] = byte.MaxValue;
            }

            public void Dispose()
            {
            }
        }
    }
}
