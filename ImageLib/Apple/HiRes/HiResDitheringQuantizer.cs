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

            for (var y = 0; y < 192; y++)
            {
                var lineOffset = Apple2Utils.GetHiResLineOffset(y);
                byte previousByte = 0;

                for (var c = 0; c < 40; c++)
                {
                    var x = c * 7;

                    var c1 = GetPixelSafe(x + 0, y).Add(errors[x + 1]).Clamp();
                    var c2 = GetPixelSafe(x + 1, y).Add(errors[x + 2]).Clamp();
                    var c3 = GetPixelSafe(x + 2, y).Add(errors[x + 3]).Clamp();
                    var c4 = GetPixelSafe(x + 3, y).Add(errors[x + 4]).Clamp();
                    var c5 = GetPixelSafe(x + 4, y).Add(errors[x + 5]).Clamp();
                    var c6 = GetPixelSafe(x + 5, y).Add(errors[x + 6]).Clamp();
                    var c7 = GetPixelSafe(x + 6, y).Add(errors[x + 7]).Clamp();

                    want[x + 0] = c1;
                    want[x + 1] = c2;
                    want[x + 2] = c3;
                    want[x + 3] = c4;
                    want[x + 4] = c5;
                    want[x + 5] = c6;
                    want[x + 6] = c7;

                    var septet = new Septet
                    {
                        C1 = c1.ToLab(),
                        C2 = c2.ToLab(),
                        C3 = c3.ToLab(),
                        C4 = c4.ToLab(),
                        C5 = c5.ToLab(),
                        C6 = c6.ToLab(),
                        C7 = c7.ToLab(),
                    };

                    var match = _palette.Match(septet, (c & 1) != 0, previousByte);

                    result[lineOffset + c] = match;
                    previousByte = match;
                }

                _renderer.RenderLine(new ArraySegment<byte>(result, lineOffset, 40), new ArraySegment<byte>(renderBuf));
                for (var i = 0; i < 280; i++)
                {
                    var o = i * 4;
                    var rgb = Rgb.FromRgb(renderBuf[o + 2], renderBuf[o + 1], renderBuf[o + 0]);
                    got[i] = rgb.ToXyz();
                }

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
