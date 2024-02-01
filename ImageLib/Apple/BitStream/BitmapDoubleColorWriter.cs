using ImageLib.ColorManagement;

namespace ImageLib.Apple.BitStream
{
    internal sealed class BitmapDoubleColorWriter : IColorWriter
    {
        private readonly Bgr32BitmapData _dst;
        private int _offset1;
        private int _offset2;

        public BitmapDoubleColorWriter(Bgr32BitmapData dst, int startLine)
        {
            _dst = dst;

            const int bytesPerBgr32Pixel = 4;
            var stride = bytesPerBgr32Pixel * dst.Width;
            _offset1 = stride * startLine;
            _offset2 = _offset1 + stride;
        }

        public void Write(Rgb c)
        {
            if (_offset2 + 4 > _dst.Pixels.Length)
                return;

            _dst.Pixels[_offset1++] = c.B;
            _dst.Pixels[_offset1++] = c.G;
            _dst.Pixels[_offset1++] = c.R;
            ++_offset1;

            _dst.Pixels[_offset2++] = c.B;
            _dst.Pixels[_offset2++] = c.G;
            _dst.Pixels[_offset2++] = c.R;
            ++_offset2;
        }

        public void Dispose()
        {
        }
    }
}
