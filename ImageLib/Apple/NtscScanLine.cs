using ImageLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ImageLib.Apple
{
    /// <summary>
    /// Converts a stream of bits as supplied by Apple hardware into
    /// a stream of colors.
    /// </summary>
    public class NtscScanLine : IDisposable
    {
        private ColorWriter _colorWriter;
        private int _bits;
        private int _phase;

        public NtscScanLine(ColorWriter colorWriter, int initialPhase = 0)
        {
            if (initialPhase < 0 || initialPhase > 3)
                throw new ArgumentOutOfRangeException("initialPhase", initialPhase, "Must be in the range [0, 3]");

            _colorWriter = colorWriter;
            _phase = initialPhase - 1;
        }

        public void Write(int bit)
        {
            _bits = (_bits >> 1) | ((bit & 1) << 5);
            ++_phase;
            _colorWriter.Write(YIQColor.From6BitsPerceptual(_bits, _phase).ToColor());
        }

        public void Flush()
        {
            while (_bits != 0)
            {
                Write(0);
            }
        }

        public void Close()
        {
            Flush();
            _colorWriter.Close();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
                _colorWriter.Dispose();
            }
        }

        private static readonly int[] _bitCount = new int[] { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4 };
    }
}
