using ImageLib.Util;

namespace ImageLib.Apple.BitStream
{
    /// Writes only a subset of pixels into the inner writer.
    public class PartialLineColorWriter : IColorWriter
    {
        private readonly IColorWriter _inner;
        private int _skip;
        private int _width;

        public PartialLineColorWriter(IColorWriter inner, int skip, int width)
        {
            _inner = inner;
            _skip = skip;
            _width = width;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public void Write(Rgb c)
        {
            if (_skip-- > 0)
                return;

            if (_width-- <= 0)
                return;

            _inner.Write(c);
        }
    }
}
