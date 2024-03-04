using ImageLib.ColorManagement;
using ImageLib.Util;

namespace ImageLib.Quantization
{
    public class FloydSteinbergDithering : IQuantizer
    {
        public void Quantize(IReadOnlyPixels src, IWriteablePixels<int> dst, Palette palette)
        {
            var currentLineErrors = new XyzColor[dst.Width + 2];

            for (var y = 0; y < dst.Height; y++)
            {
                var nextLineErrors = new XyzColor[dst.Width + 2];

                for (var x = 0; x < dst.Width; x++)
                {
                    var srcPixel = x < src.Width && y < src.Height ? src.GetPixel(x, y) : default;

                    var xyzPixel = srcPixel.ToXyz();
                    xyzPixel = xyzPixel.Add(currentLineErrors[x + 1]).Clamp();
                    var colorIndex = palette.Match(xyzPixel.ToLab());
                    var actualPixel = palette[colorIndex].Value.ToXyz();
                    var error = xyzPixel.Sub(actualPixel);

                    dst.SetPixel(x, y, colorIndex);

                    currentLineErrors[x + 2] = currentLineErrors[x + 2].Add(error.Mul(7 / 16.0));
                    nextLineErrors[x + 0] = nextLineErrors[x + 0].Add(error.Mul(3 / 16.0));
                    nextLineErrors[x + 1] = nextLineErrors[x + 1].Add(error.Mul(5 / 16.0));
                    nextLineErrors[x + 2] = nextLineErrors[x + 2].Add(error.Mul(1 / 16.0));
                }

                currentLineErrors = nextLineErrors;
            }
        }
    }
}
