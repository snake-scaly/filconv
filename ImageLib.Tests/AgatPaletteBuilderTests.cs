using System.Linq;
using ImageLib.Agat;
using ImageLib.Util;
using Xunit;

namespace ImageLib.Tests
{
    public class AgatPaletteBuilderTests
    {
        [Fact]
        public void Build_Basic()
        {
            var pixels = new[] { Rgb.FromRgb(0, 0, 0), Rgb.FromRgb(1, 1, 1), Rgb.FromRgb(2, 2, 2) };
            var builder = new AgatPaletteBuilder();
            var palette = builder.Build(pixels, 2);
            Assert.Equal(2, palette.Count());
        }

        [Fact]
        public void Build_FillUnpopulatedWithBlack()
        {
            var pixels = new[] { Rgb.FromRgb(0, 0, 0), Rgb.FromRgb(1, 1, 1) };
            var builder = new AgatPaletteBuilder();
            var palette = builder.Build(pixels, 3).ToList();
            Assert.Equal(3, palette.Count());
            Assert.Equal(0, palette[2].R);
            Assert.Equal(0, palette[2].G);
            Assert.Equal(0, palette[2].B);
        }
    }
}
