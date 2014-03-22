using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageLib.Apple
{
    class BitmapDoubleColorWriter : ColorWriter
    {
        private const int _numberOfLines = 2;

        private WriteableBitmap _dst;
        private int _startLine;
        private byte[] _pixels;
        private int _stride;
        private int _offset1;
        private int _offset2;

        public BitmapDoubleColorWriter(WriteableBitmap dst, int startLine)
        {
            _dst = dst;
            _startLine = startLine;

            const int bytesPerBgr32Pixel = 4;
            _stride = bytesPerBgr32Pixel * dst.PixelWidth;
            _pixels = new byte[_stride * _numberOfLines];
            _offset1 = 0;
            _offset2 = _stride;
        }

        public void Write(System.Windows.Media.Color c)
        {
            if (_offset2 + 4 > _pixels.Length)
                return;

            _pixels[_offset1++] = c.B;
            _pixels[_offset1++] = c.G;
            _pixels[_offset1++] = c.R;
            ++_offset1;

            _pixels[_offset2++] = c.B;
            _pixels[_offset2++] = c.G;
            _pixels[_offset2++] = c.R;
            ++_offset2;
        }

        public void Close()
        {
            var r = new Int32Rect(0, _startLine, _dst.PixelWidth, _numberOfLines);
            _dst.WritePixels(r, _pixels, _stride, 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }
    }
}
