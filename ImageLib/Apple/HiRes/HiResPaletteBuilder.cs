using System;
using System.Linq;
using System.Threading.Tasks;
using ImageLib.ColorManagement;

namespace ImageLib.Apple.HiRes
{
    public class HiResPaletteBuilder
    {
        private const int _srcPixelsPerByte = 7;
        private const int _tgtBytesPerPixel = 4;

        private readonly IHiResFragmentRenderer _renderer;

        public HiResPaletteBuilder(IHiResFragmentRenderer renderer)
        {
            _renderer = renderer;
        }

        public HiResPalette Build()
        {
            const int entryCount = 1 << 13;

            var entries = new Septet[entryCount];

            Parallel.ForEach(
                Enumerable.Range(0, entryCount),
                () => new Buffers(),
                (i, _, buffers) =>
                {
                    var lineBuffer = buffers.LineBuffer;
                    var colorBuffer = buffers.ColorBuffer;

                    // i is OPpNnCccccccc, see HiResPalette
                    //      2109876543210
                    var odd = (i & (1 << 12)) != 0;
                    lineBuffer[0] = (byte)((i >> 4) & 0xC0);
                    lineBuffer[1] = (byte)(i & 0xFF);
                    lineBuffer[2] = (byte)(((i >> 2) & 0x80) | ((i >> 8) & 1));

                    _renderer.RenderLine(new ArraySegment<byte>(lineBuffer), new ArraySegment<byte>(colorBuffer), !odd);

                    // take the middle 7 pixels
                    entries[i] = new Septet
                    {
                        C1 = Color(0),
                        C2 = Color(1),
                        C3 = Color(2),
                        C4 = Color(3),
                        C5 = Color(4),
                        C6 = Color(5),
                        C7 = Color(6),
                    };

                    return buffers;

                    LabColor Color(int k)
                    {
                        var o = (_srcPixelsPerByte + k) * _tgtBytesPerPixel;
                        var rgb = Rgb.FromRgb(colorBuffer[o + 2], colorBuffer[o + 1], colorBuffer[o + 0]);
                        return rgb.ToLab();
                    }
                },
                _ => {});

            return new HiResPalette(entries);
        }

        private class Buffers
        {
            public byte[] LineBuffer = new byte[3];
            public byte[] ColorBuffer = new byte[3 * _srcPixelsPerByte * _tgtBytesPerPixel];
        }
    }
}
