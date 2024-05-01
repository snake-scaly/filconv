using System;
using ImageLib.Util;

namespace ImageLib.Apple.BitStream
{
    /// <summary>
    /// Converts a stream of bits as supplied by Apple hardware into
    /// a stream of colors.
    /// </summary>
    internal sealed class NtscScanlineWriter : IScanlineWriter
    {
        private readonly IColorWriter _colorWriter;
        private int _bits;
        private int _phase;
        private bool _disposed;
        private int _column;

        public NtscScanlineWriter(IColorWriter colorWriter, int initialPhase = 0)
        {
            if (initialPhase < 0 || initialPhase > 3)
                throw new ArgumentOutOfRangeException(nameof(initialPhase), initialPhase, "Must be in the range [0, 3]");

            _colorWriter = colorWriter;
            _phase = initialPhase - 1;
            _column = 0;
        }

        public void Write(int bit)
        {
            _bits = (_bits >> 1) | ((bit & 1) << 5);
            ++_phase;
            ++_column;
            if (_column < 560)
                _colorWriter.Write(YIQColor.From6BitsPerceptual(_bits, _phase).ToColor());
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            while (_column < 560)
            {
                Write(0);
            }

            _colorWriter.Dispose();
            _disposed = true;
        }
    }
}
