using System;
using ImageLib.ColorManagement;

namespace ImageLib.Apple.HiRes
{
    public class HiResDitheringQuantizer : IHiResQuantizer
    {
        private readonly IHiResPalette _palette;
        private readonly IHiResFragmentRenderer _renderer;

        public HiResDitheringQuantizer(IHiResPalette palette, IHiResFragmentRenderer renderer)
        {
            _palette = palette;
            _renderer = renderer;
        }

        public byte[] Quantize(IReadOnlyPixels src)
        {
            var width = src.Width;
            var height = src.Height;
            var result = new byte[8 << 10];
            var errors = new XyzColor[282];
            var renderBuf = new byte[280 * 4];
            var want = new XyzColor[280];
            var got = new XyzColor[280];
            var s2 = Math.Sqrt(2);

            for (var y = 0; y < 192; y++)
            {
                var lineOffset = Apple2Utils.GetHiResLineOffset(y);
                byte previousByte = 0;

                for (var c = 0; c < 40; c++)
                {
                    var x = c * 7;

                    var septet = new LinearSeptet
                    {
                        C1 = GetPixelSafe(x + 0, y).Add(errors[x + 1]).Clamp(),
                        C2 = GetPixelSafe(x + 1, y).Add(errors[x + 2]).Clamp(),
                        C3 = GetPixelSafe(x + 2, y).Add(errors[x + 3]).Clamp(),
                        C4 = GetPixelSafe(x + 3, y).Add(errors[x + 4]).Clamp(),
                        C5 = GetPixelSafe(x + 4, y).Add(errors[x + 5]).Clamp(),
                        C6 = GetPixelSafe(x + 5, y).Add(errors[x + 6]).Clamp(),
                        C7 = GetPixelSafe(x + 6, y).Add(errors[x + 7]).Clamp(),
                    };

                    want[x + 0] = septet.C1;
                    want[x + 1] = septet.C2;
                    want[x + 2] = septet.C3;
                    want[x + 3] = septet.C4;
                    want[x + 4] = septet.C5;
                    want[x + 5] = septet.C6;
                    want[x + 6] = septet.C7;

                    var match = _palette.Match(septet, (c & 1) != 0, previousByte);

                    got[x + 0] = match.Septet.C1;
                    got[x + 1] = match.Septet.C2;
                    got[x + 2] = match.Septet.C3;
                    got[x + 3] = match.Septet.C4;
                    got[x + 4] = match.Septet.C5;
                    got[x + 5] = match.Septet.C6;
                    got[x + 6] = match.Septet.C7;

                    result[lineOffset + c] = match.Native;
                    previousByte = match.Native;
                }

                _renderer.RenderLine(new ArraySegment<byte>(result, lineOffset, 40), new ArraySegment<byte>(renderBuf));

                Array.Clear(errors, 0, errors.Length);
                for (var x = 0; x < 280; x++)
                {
                    var e = want[x].Sub(got[x]);
                    errors[x + 0] = errors[x + 0].Add(e.Mul(.2));
                    errors[x + 1] = errors[x + 1].Add(e.Mul(.4));
                    errors[x + 2] = errors[x + 2].Add(e.Mul(.3));
                }
            }

            return result;

            XyzColor GetPixelSafe(int x, int y)
            {
                if (x < 0 || x >= width || y < 0 || y >= height)
                    return default;
                return src.GetPixel(x, y).ToXyz();
            }
        }
    }
}
