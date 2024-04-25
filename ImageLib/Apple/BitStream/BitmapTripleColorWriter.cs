using ImageLib.ColorManagement;

namespace ImageLib.Apple.BitStream
{
    internal sealed class BitmapTripleColorWriter : IColorWriter
    {
        private readonly Bgr32BitmapData _dst;
        private int _offset1;
        private int _offset2;
        private int _offset3;

        public BitmapTripleColorWriter(Bgr32BitmapData dst, int startLine)
        {
            _dst = dst;

            const int bytesPerBgr32Pixel = 4;
            var stride = bytesPerBgr32Pixel * dst.Width;
            _offset1 = stride * startLine;
            _offset2 = _offset1 + stride;
            _offset3 = _offset2 + stride;
        }

        public void Write(Rgb c)
        {
            if (_offset2 + 4 > _dst.Pixels.Length)
                return;

            _dst.Pixels[_offset1++] = c.B;
            _dst.Pixels[_offset1++] = c.G;
            _dst.Pixels[_offset1++] = c.R;
            _dst.Pixels[_offset1++] = byte.MaxValue;

            _dst.Pixels[_offset2++] = c.B;
            _dst.Pixels[_offset2++] = c.G;
            _dst.Pixels[_offset2++] = c.R;
            _dst.Pixels[_offset2++] = byte.MaxValue;

            _dst.Pixels[_offset3++] = 0;
            _dst.Pixels[_offset3++] = 0;
            _dst.Pixels[_offset3++] = 0;
            _dst.Pixels[_offset3++] = byte.MaxValue;
        }

        public void Dispose()
        {
        }
    }
}
