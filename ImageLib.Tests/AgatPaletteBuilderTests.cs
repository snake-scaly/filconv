using System.Linq;
using ImageLib.Agat;
using ImageLib.Util;
using Xunit;

namespace ImageLib.Tests
{
    public class AgatPaletteBuilderTests
    {
        [Fact]
        public void Build_Reduce()
        {
            var pixels = new[] { Rgb.FromRgb(0, 0, 0), Rgb.FromRgb(128, 128, 128), Rgb.FromRgb(255, 255, 255) };
            var palette = new AgatPaletteBuilder().Build(pixels, 2);
            Assert.Equal(2, palette.Count());
        }

        [Fact]
        public void Build_FillUnpopulatedWithBlack()
        {
            var pixels = new[] { Rgb.FromRgb(128, 128, 128), Rgb.FromRgb(255, 255, 255) };
            var palette = new AgatPaletteBuilder().Build(pixels, 3).ToList();
            Assert.Equal(3, palette.Count);
            Assert.Equal(Rgb.FromRgb(0, 0, 0), palette[2]);
        }

        [Theory]
        [InlineData(17 * 3 - 1, 17 * 3)]
        [InlineData(17 * 5 + 1, 17 * 5)]
        [InlineData(17 * 9 + 8, 17 * 9)]
        [InlineData(17 * 13 + 9, 17 * 14)]
        public void Build_ChoseClosestQuantizedColor(byte given, byte expected)
        {
            var palette = new AgatPaletteBuilder().Build(new[] { Rgb.FromRgb(given, given, given) }, 1);
            var c = Assert.Single(palette);
            Assert.Equal(Rgb.FromRgb(expected, expected, expected), c);
        }
    }
}
