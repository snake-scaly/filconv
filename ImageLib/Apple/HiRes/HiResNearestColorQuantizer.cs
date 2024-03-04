using System.Linq;
using System.Threading.Tasks;
using ImageLib.ColorManagement;

namespace ImageLib.Apple.HiRes
{
    public class HiResNearestColorQuantizer : IHiResQuantizer
    {
        private readonly IHiResPalette _palette;

        public HiResNearestColorQuantizer(IHiResPalette palette)
        {
            _palette = palette;
        }

        public byte[] Quantize(IReadOnlyPixels src)
        {
            var width = src.Width;
            var height = src.Height;
            var data = new byte[8 << 10];

            Parallel.ForEach(
                Enumerable.Range(0, 192),
                (y, _) =>
                {
                    var lineOffset = Apple2Utils.GetHiResLineOffset(y);
                    byte previousByte = 0;

                    for (var c = 0; c < 40; c++)
                    {
                        var x = c * 7;
                        var septet = new Septet
                        {
                            C1 = GetPixelSafe(x + 0, y),
                            C2 = GetPixelSafe(x + 1, y),
                            C3 = GetPixelSafe(x + 2, y),
                            C4 = GetPixelSafe(x + 3, y),
                            C5 = GetPixelSafe(x + 4, y),
                            C6 = GetPixelSafe(x + 5, y),
                            C7 = GetPixelSafe(x + 6, y),
                        };
                        var match = _palette.Match(septet, (c & 1) != 0, previousByte);
                        data[lineOffset + c] = match;
                        previousByte = match;
                    }
                });

            return data;

            LabColor GetPixelSafe(int x, int y)
            {
                if (x < 0 || x >= width || y < 0 || y >= height)
                    return default;
                return src.GetPixel(x, y).ToLab();
            }
        }
    }
}
