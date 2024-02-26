using ImageLib.ColorManagement;
using ImageLib.Util;

namespace ImageLib.Quantization
{
    public class NearestColorQuantizer : IQuantizer
    {
        public void Quantize(IReadOnlyPixels src, IWriteablePixels<int> dst, Palette palette)
        {
            for (var y = 0; y < dst.Height; y++)
            {
                for (var x = 0; x < dst.Width; x++)
                {
                    var srcPixel = x < src.Width && y < src.Height ? src.GetPixel(x, y) : default;

                    var xyzPixel = ColorSpace.Srgb.ToXyz(srcPixel);
                    var colorIndex = palette.Match(xyzPixel);

                    dst.SetPixel(x, y, colorIndex);
                }
            }
        }
    }
}
