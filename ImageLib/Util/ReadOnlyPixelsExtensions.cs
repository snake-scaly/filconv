using System.Collections.Generic;
using ImageLib.ColorManagement;

namespace ImageLib.Util
{
    public static class ReadOnlyPixelsExtensions
    {
        public static IEnumerable<Rgb> AllPixelsInRect(this IReadOnlyPixels p, int left, int top, int right, int bottom)
        {
            var maxX = p.Width;
            var maxY = p.Height;
            for (var y = top; y < bottom; y++)
                for (var x = left; x < right; x++)
                    yield return x < maxX && y < maxY ? p.GetPixel(x, y) : default;
        }
    }
}
