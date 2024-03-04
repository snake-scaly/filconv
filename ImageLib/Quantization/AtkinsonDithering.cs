using ImageLib.ColorManagement;
using ImageLib.Util;

namespace ImageLib.Quantization
{
    public class AtkinsonDithering : IQuantizer
    {
        public void Quantize(IReadOnlyPixels src, IWriteablePixels<int> dst, Palette palette)
        {
            var line0Errors = new XyzColor[dst.Width + 3];
            var line1Errors = new XyzColor[dst.Width + 3];

            for (var y = 0; y < dst.Height; y++)
            {
                var line2Errors = new XyzColor[dst.Width + 3];

                for (var x = 0; x < dst.Width; x++)
                {
                    var srcPixel = x < src.Width && y < src.Height ? src.GetPixel(x, y) : default;

                    var xyzPixel = srcPixel.ToXyz();
                    xyzPixel = xyzPixel.Add(line0Errors[x + 1]).Clamp();
                    var colorIndex = palette.Match(xyzPixel.ToLab());
                    var actualPixel = palette[colorIndex].Value.ToXyz();
                    var error = xyzPixel.Sub(actualPixel);

                    dst.SetPixel(x, y, colorIndex);

                    error = error.Div(8);
                    line0Errors[x + 2] = line0Errors[x + 2].Add(error);
                    line0Errors[x + 3] = line0Errors[x + 3].Add(error);
                    line1Errors[x + 0] = line1Errors[x + 0].Add(error);
                    line1Errors[x + 1] = line1Errors[x + 1].Add(error);
                    line1Errors[x + 2] = line1Errors[x + 2].Add(error);
                    line2Errors[x + 1] = line2Errors[x + 1].Add(error);
                }

                line0Errors = line1Errors;
                line1Errors = line2Errors;
            }
        }
    }
}
