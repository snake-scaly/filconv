using System.Linq;
using Xunit;

namespace FilLib.Tests
{
    public class ImageMetaTests
    {
        private readonly byte[] _validMeta =
        {
            0x00, 0x00, 0x00, 0x00, 0xD6, 0xD2, 0x6D, 0x30, 0x0F, 0x8A, 0xC0, 0x0E, 0xD6, 0xF3, 0x7A, 0x0B,
            0xE1, 0xE7, 0xE1, 0xF4, 0xB7, 0xA0, 0xA0, 0xA0, 0x0F, 0x0F, 0x4C, 0x0E, 0x84, 0x73, 0x7F, 0x8B,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0F, 0x0E, 0xC5, 0xA7, 0x50, 0x73, 0x76, 0xFB,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xC3, 0x4F, 0x4D, 0x4D, 0x45, 0x4E, 0x54, 0xAE, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
            0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0, 0xA0,
        };

        private readonly uint[] _expectedColors =
        {
            0xFF000000,
            0xFFFFFFFF,
            0xFF880000,
            0xFFAAFFEE,
            0xFFCC44CC,
            0xFF00CC55,
            0xFF0000AA,
            0xFFEEEE77,
            0xFFDD8855,
            0xFF664400,
            0xFFFF7777,
            0xFF333333,
            0xFF777777,
            0xFFAAFF66,
            0xFF0088FF,
            0xFFBBBBBB,
        };

        [Fact]
        public void TryParse_AcceptsValidData()
        {
            var fil = new Fil { Type = new FilType(0x84), Sectors = _validMeta };

            var success = ImageMeta.TryParse(fil, out var meta);

            Assert.True(success);
            Assert.NotNull(meta);
            Assert.Equal(ImageMeta.Mode.Agat_256_256_Pal4, meta.DisplayMode);
            Assert.True(meta.IsGigaScreen);
            Assert.Equal(ImageMeta.Palette.Agat_4, meta.PaletteType);
            Assert.Equal(_expectedColors, meta.CustomPalette);
            Assert.Equal("АГАТ7", meta.Charset);
            Assert.Equal("Comment.", meta.Comment);
        }

        [Theory]
        [InlineData(0x10, ImageMeta.Mode.Agat_256_256_Mono)]
        [InlineData(0x40, ImageMeta.Mode.Agat_64_64_Pal16)]
        [InlineData(0x50, ImageMeta.Mode.Agat_128_128_Pal16)]
        [InlineData(0x60, ImageMeta.Mode.Agat_256_256_Pal4)]
        [InlineData(0x70, ImageMeta.Mode.Agat_512_256_Mono)]
        [InlineData(0x80, ImageMeta.Mode.Agat_128_256_Pal16)]
        [InlineData(0x90, ImageMeta.Mode.Agat_280_192_AppleHiRes)]
        [InlineData(0x21, ImageMeta.Mode.Agat_T32_Pal16)]
        [InlineData(0x31, ImageMeta.Mode.Agat_T64_Mono)]
        [InlineData(0x41, ImageMeta.Mode.Agat_T64_Pal16)]
        [InlineData(0xA1, ImageMeta.Mode.Agat_T32_Pal16_FgBg)]
        [InlineData(0xC1, ImageMeta.Mode.Agat_T64_Pal16_FgBg)]
        [InlineData(0x9A, ImageMeta.Mode.Apple_280_192_HiRes)]
        [InlineData(0xAA, ImageMeta.Mode.Apple_T40)]
        [InlineData(0xBA, ImageMeta.Mode.Apple_T80)]
        [InlineData(0xCA, ImageMeta.Mode.Apple_40_48_LoRes)]
        [InlineData(0xDA, ImageMeta.Mode.Apple_80_48_DoubleLoRes)]
        [InlineData(0xEA, ImageMeta.Mode.Apple_140_192_DoubleHiResColor)]
        [InlineData(0xFA, ImageMeta.Mode.Apple_560_192_DoubleHiResMono)]
        [InlineData(0x1D, ImageMeta.Mode.Agat_256_256_Mono)]
        [InlineData(0x4D, ImageMeta.Mode.Agat_64_64_Pal16)]
        [InlineData(0x5D, ImageMeta.Mode.Agat_128_128_Pal16)]
        [InlineData(0x6D, ImageMeta.Mode.Agat_256_256_Pal4)]
        [InlineData(0x7D, ImageMeta.Mode.Agat_512_256_Mono)]
        [InlineData(0x8D, ImageMeta.Mode.Agat_128_256_Pal16)]
        [InlineData(0x9D, ImageMeta.Mode.Agat_280_192_AppleHiRes)]
        [InlineData(0xA0, ImageMeta.Mode.Unknown)]
        [InlineData(0x23, ImageMeta.Mode.Unknown)]
        public void TryParse_AllValidDisplayModes(byte code, ImageMeta.Mode mode)
        {
            _validMeta[6] = code;
            var fil = new Fil { Type = new FilType(0x84), Sectors = _validMeta };

            var success = ImageMeta.TryParse(fil, out var meta);

            Assert.True(success);
            Assert.NotNull(meta);
            Assert.Equal(mode, meta.DisplayMode);
        }

        // test all palette variants
        [Theory]
        [InlineData(0x00, ImageMeta.Palette.Agat_1)]
        [InlineData(0x10, ImageMeta.Palette.Agat_2)]
        [InlineData(0x20, ImageMeta.Palette.Agat_3)]
        [InlineData(0x30, ImageMeta.Palette.Agat_4)]
        [InlineData(0x80, ImageMeta.Palette.Agat_1_Gray)]
        [InlineData(0x90, ImageMeta.Palette.Agat_2_Gray)]
        [InlineData(0xA0, ImageMeta.Palette.Agat_3_Gray)]
        [InlineData(0xB0, ImageMeta.Palette.Agat_4_Gray)]
        [InlineData(0x40, ImageMeta.Palette.Unknown)]
        [InlineData(0x11, ImageMeta.Palette.Agat_2)]
        [InlineData(0xF0, ImageMeta.Palette.Custom)]
        public void TryParse_AllValidPaletteTypes(byte code, ImageMeta.Palette palette)
        {
            _validMeta[7] = code;
            var fil = new Fil { Type = new FilType(0x84), Sectors = _validMeta };

            var success = ImageMeta.TryParse(fil, out var meta);

            Assert.True(success);
            Assert.NotNull(meta);
            Assert.Equal(palette, meta.PaletteType);
        }

        [Fact]
        public void TryParse_RejectsNonBFiles()
        {
            var fil = new Fil { Sectors = _validMeta };

            var success = ImageMeta.TryParse(fil, out var meta);

            Assert.False(success);
            Assert.Null(meta);
        }

        [Fact]
        public void TryParse_RejectsEmptySectors()
        {
            var fil = new Fil { Type = new FilType(4) };

            var success = ImageMeta.TryParse(fil, out var meta);

            Assert.False(success);
            Assert.Null(meta);
        }

        [Fact]
        public void TryParse_RejectsFilesWithIncompleteLastSector()
        {
            var fil = new Fil { Type = new FilType(4), Sectors = _validMeta.Take(_validMeta.Length - 1).ToArray() };

            var success = ImageMeta.TryParse(fil, out var meta);

            Assert.False(success);
            Assert.Null(meta);
        }

        [Fact]
        public void TryParse_RejectsFilesWithDataSizeNotDivisibleBy256()
        {
            var fil = new Fil { Type = new FilType(0x84) };
            fil.SetData(new byte[257]);

            var success = ImageMeta.TryParse(fil, out var meta);

            Assert.False(success);
            Assert.Null(meta);
        }

        [Fact]
        public void TryParse_RejectsBadSignature()
        {
            _validMeta[4] = 42;
            var fil = new Fil { Type = new FilType(0x84), Sectors = _validMeta };

            var success = ImageMeta.TryParse(fil, out var meta);

            Assert.False(success);
            Assert.Null(meta);
        }
    }
}
