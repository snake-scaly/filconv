using ImageLib.ColorManagement;
using ImageLib.Util;
using Xunit;

namespace ImageLib.Tests
{
    public class PaletteTests
    {
        [Fact]
        public void CreateEmpty()
        {
            var palette = new Palette();
            Assert.Empty(palette);
        }

        [Fact]
        public void CreateFromColors()
        {
            var palette = new Palette(new[] { Rgb.FromRgb(1, 1, 1), Rgb.FromRgb(2, 2, 2) }, 4);
            Assert.Equal(4, palette.Count);
            Assert.Equal(Rgb.FromRgb(1, 1, 1), palette[0].Value);
            Assert.True(palette[0].Important);
            Assert.Equal(Rgb.FromRgb(2, 2, 2), palette[1].Value);
            Assert.True(palette[1].Important);
            Assert.Equal(default, palette[2].Value);
            Assert.False(palette[2].Important);
            Assert.Equal(default, palette[3].Value);
            Assert.False(palette[3].Important);
        }

        [Theory]
        [InlineData(180, 20, 20, 0)]
        [InlineData(190, 50, 190, 2)]
        [InlineData(30, 128, 128, 1)]
        [InlineData(255, 255, 0, 3)]
        public void FindsBestMatch(byte r, byte g, byte b, int i)
        {
            var palette = new Palette(new[]
            {
                Rgb.FromRgb(200, 0, 0),
                Rgb.FromRgb(0, 128, 50),
                Rgb.FromRgb(128, 0, 255),
                Rgb.FromRgb(200, 255, 128)
            });

            var match = palette.Match(ColorSpace.Srgb.ToXyz(Rgb.FromRgb(r, g, b)));

            Assert.Equal(i, match);
        }

        [Fact]
        public void DoesNotMatchUnimportant()
        {
            var palette = new Palette(new[] { Rgb.FromRgb(255, 255, 255) }, 2);
            var match = palette.Match(default);
            Assert.Equal(0, match);
        }

        [Fact]
        public void SortAlignsSimilarColors()
        {
            var palette = new Palette(new[] { Rgb.FromRgb(255, 0, 0), Rgb.FromRgb(0, 255, 0), Rgb.FromRgb(0, 0, 255) });
            var template = new Palette(new[] { Rgb.FromRgb(7, 245, 3), Rgb.FromRgb(1, 20, 235), Rgb.FromRgb(200, 5, 9)});

            palette.Sort(template);

            Assert.Equal(Rgb.FromRgb(0, 255, 0), palette[0].Value);
            Assert.Equal(Rgb.FromRgb(0, 0, 255), palette[1].Value);
            Assert.Equal(Rgb.FromRgb(255, 0, 0), palette[2].Value);
        }

        [Fact]
        public void SortOnlyConsidersImportantColors()
        {
            var palette = new Palette
            {
                new PaletteEntry(Rgb.FromRgb(1, 1, 1)),
                new PaletteEntry(Rgb.FromRgb(128, 128, 128), false),
                new PaletteEntry(Rgb.FromRgb(128, 128, 128), false),
            };
            var template = new Palette
            {
                new PaletteEntry(Rgb.FromRgb(128, 128, 128), false),
                new PaletteEntry(Rgb.FromRgb(128, 128, 128), false),
                new PaletteEntry(Rgb.FromRgb(255, 255, 255)),
            };

            palette.Sort(template);

            Assert.False(palette[0].Important);
            Assert.False(palette[1].Important);
            Assert.True(palette[2].Important);
            Assert.Equal(Rgb.FromRgb(1, 1, 1), palette[2].Value);
        }
    }
}
