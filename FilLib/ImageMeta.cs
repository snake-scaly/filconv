using System;
using System.Collections.Generic;
using System.Linq;

namespace FilLib
{
    public class ImageMeta
    {
        public enum Mode
        {
            Unknown,
            Agat_256_256_Mono,
            Agat_64_64_Pal16,
            Agat_128_128_Pal16,
            Agat_256_256_Pal4,
            Agat_512_256_Mono,
            Agat_128_256_Pal16,
            Agat_280_192_AppleHiRes,
            Agat_T32_Pal16,
            Agat_T64_Mono,
            Agat_T64_Pal16,
            Agat_T32_Pal16_FgBg,
            Agat_T64_Pal16_FgBg,
            Apple_280_192_HiRes,
            Apple_T40,
            Apple_T80,
            Apple_40_48_LoRes,
            Apple_80_48_DoubleLoRes,
            Apple_140_192_DoubleHiResColor,
            Apple_560_192_DoubleHiResMono,
        }

        public enum Palette
        {
            Unknown,
            Agat_1,
            Agat_2,
            Agat_3,
            Agat_4,
            Agat_1_Gray,
            Agat_2_Gray,
            Agat_3_Gray,
            Agat_4_Gray,
            Custom,
        }

        public Mode DisplayMode { get; set; }

        public bool IsGigaScreen { get; set; }

        public Palette PaletteType { get; set; }

        public IList<uint> CustomPalette { get; set; }

        public string Charset
        {
            get => _charset;
            set => _charset = value?.Length > 8 ? value.Substring(0, 8) : value;
        }

        public string Comment
        {
            get => _comment;
            set => _comment = value?.Length > 0xC0 ? value.Substring(0, 0xC0) : value;
        }

        public static bool TryParse(Fil fil, out ImageMeta result)
        {
            result = null;

            if (!fil.Type.HasAddrSize || fil.Sectors.Length == 0 || (fil.Sectors.Length & 0xFF) != 0)
                return false;

            var metaOffset = (fil.Sectors.Length - 1) & ~255;

            if (fil.Sectors[metaOffset + 4] != 0xD6 || fil.Sectors[metaOffset + 5] != 0xD2)
                return false;

            result = new ImageMeta
            {
                DisplayMode = ParseDisplayMode(fil.Sectors, metaOffset),
                IsGigaScreen = (fil.Sectors[metaOffset + 6] & 0x0F) == 0x0D,
                PaletteType = ParsePaletteType(fil.Sectors, metaOffset),
                CustomPalette = ParseCustomPalette(fil.Sectors, metaOffset),
                Charset = ParseCharset(fil.Sectors, metaOffset),
                Comment = ParseComment(fil.Sectors, metaOffset),
            };

            return true;
        }

        public void Embed(Fil fil)
        {
            if (!fil.Type.HasAddrSize || fil.Sectors.Length < 512 || (fil.Sectors.Length & 0xFF) != 0)
                throw new InvalidOperationException("Can only embed in type B files");

            var metaOffset = (fil.Sectors.Length - 1) & ~255;

            fil.Sectors[metaOffset + 4] = 0xD6;
            fil.Sectors[metaOffset + 5] = 0xD2;

            fil.Sectors[metaOffset + 6] = _modes
                .Where(m => m.Mode == DisplayMode)
                .Select(m => m.Code)
                .FirstOrDefault();

            var paletteType = _palettes
                .Where(p => p.Palette == PaletteType)
                .Select(p => p.Code)
                .FirstOrDefault();
            fil.Sectors[metaOffset + 7] = (byte)(paletteType << 4);

            if (CustomPalette != null)
            {
                for (var i = 0; i < 8; i++)
                {
                    var rgb1 = CustomPalette[i * 2];
                    var rgb2 = CustomPalette[i * 2 + 1];
                    fil.Sectors[metaOffset + 0x08 + i] = (byte)(((rgb1 & 0xF00000) >> 16) | ((rgb2 & 0xF00000) >> 20));
                    fil.Sectors[metaOffset + 0x18 + i] = (byte)(((rgb1 & 0xF000) >> 8) | ((rgb2 & 0xF000) >> 12));
                    fil.Sectors[metaOffset + 0x28 + i] = (byte)((rgb1 & 0xF0) | ((rgb2 & 0xF0) >> 4));
                }
            }

            Fill(fil.Sectors, 0xA0, metaOffset + 0x10, 8);
            Fill(fil.Sectors, 0xA0, metaOffset + 0x40, 0xC0);
            if (Charset != null)
                AgatEncoding.Encode(Charset).CopyTo(fil.Sectors, metaOffset + 0x10);
            if (Comment != null)
                AgatEncoding.Encode(Comment).CopyTo(fil.Sectors, metaOffset + 0x40);
        }

        private static Mode ParseDisplayMode(byte[] data, int metaOffset)
        {
            byte code = data[metaOffset + 6];

            // GigaScreen is Agat
            if ((code & 0x0F) == 0x0D)
                code &= 0xF0;

            return _modes
                .Where(m => m.Code == code)
                .Select(m => m.Mode)
                .FirstOrDefault();
        }

        private static Palette ParsePaletteType(byte[] data, int metaOffset)
        {
            var code = data[metaOffset + 7] >> 4;

            return _palettes
                .Where(p => p.Code == code)
                .Select(p => p.Palette)
                .FirstOrDefault();
        }

        private static IList<uint> ParseCustomPalette(byte[] data, int metaOffset)
        {
            var result = new uint[16];

            for (var i = 0; i < 8; i++)
            {
                var or = metaOffset + 8 + i;
                var og = metaOffset + 8 + 16 + i;
                var ob = metaOffset + 8 + 32 + i;

                result[i * 2] = Rgb444ToUint(data[or] >> 4, data[og] >> 4, data[ob] >> 4);
                result[i * 2 + 1] = Rgb444ToUint(data[or] & 15, data[og] & 15, data[ob] & 15);
            }

            return result;
        }

        private static uint Rgb444ToUint(int r, int g, int b)
        {
            var ur = (uint)r;
            var ug = (uint)g;
            var ub = (uint)b;

            return 0xFF000000 | (ur << 20) | (ur << 16) | (ug << 12) | (ug << 8) | (ub << 4) | ub;
        }

        private static string ParseCharset(byte[] data, int metaOffset)
        {
            return AgatEncoding.Decode(data.Skip(metaOffset + 16).Take(8).ToArray()).Trim();
        }

        private static string ParseComment(byte[] data, int metaOffset)
        {
            return AgatEncoding.Decode(data.Skip(metaOffset + 64).Take(192).ToArray()).Trim();
        }

        private void Fill(IList<byte> a, byte v, int start, int len)
        {
            for (; len > 0; len--, start++)
                a[start] = v;
        }

        private static readonly ModeMap[] _modes =
        {
            new ModeMap { Code = 0x10, Mode = Mode.Agat_256_256_Mono },
            new ModeMap { Code = 0x40, Mode = Mode.Agat_64_64_Pal16 },
            new ModeMap { Code = 0x50, Mode = Mode.Agat_128_128_Pal16 },
            new ModeMap { Code = 0x60, Mode = Mode.Agat_256_256_Pal4 },
            new ModeMap { Code = 0x70, Mode = Mode.Agat_512_256_Mono },
            new ModeMap { Code = 0x80, Mode = Mode.Agat_128_256_Pal16 },
            new ModeMap { Code = 0x90, Mode = Mode.Agat_280_192_AppleHiRes },
            new ModeMap { Code = 0x21, Mode = Mode.Agat_T32_Pal16 },
            new ModeMap { Code = 0x31, Mode = Mode.Agat_T64_Mono },
            new ModeMap { Code = 0x41, Mode = Mode.Agat_T64_Pal16 },
            new ModeMap { Code = 0xA1, Mode = Mode.Agat_T32_Pal16_FgBg },
            new ModeMap { Code = 0xC1, Mode = Mode.Agat_T64_Pal16_FgBg },
            new ModeMap { Code = 0x9A, Mode = Mode.Apple_280_192_HiRes },
            new ModeMap { Code = 0xAA, Mode = Mode.Apple_T40 },
            new ModeMap { Code = 0xBA, Mode = Mode.Apple_T80 },
            new ModeMap { Code = 0xCA, Mode = Mode.Apple_40_48_LoRes },
            new ModeMap { Code = 0xDA, Mode = Mode.Apple_80_48_DoubleLoRes },
            new ModeMap { Code = 0xEA, Mode = Mode.Apple_140_192_DoubleHiResColor },
            new ModeMap { Code = 0xFA, Mode = Mode.Apple_560_192_DoubleHiResMono },
        };

        private static readonly PaletteMap[] _palettes =
        {
            new PaletteMap { Code = 0, Palette = Palette.Agat_1 },
            new PaletteMap { Code = 1, Palette = Palette.Agat_2 },
            new PaletteMap { Code = 2, Palette = Palette.Agat_3 },
            new PaletteMap { Code = 3, Palette = Palette.Agat_4 },
            new PaletteMap { Code = 8, Palette = Palette.Agat_1_Gray },
            new PaletteMap { Code = 9, Palette = Palette.Agat_2_Gray },
            new PaletteMap { Code = 10, Palette = Palette.Agat_3_Gray },
            new PaletteMap { Code = 11, Palette = Palette.Agat_4_Gray },
            new PaletteMap { Code = 15, Palette = Palette.Custom },
        };

        private string _charset;
        private string _comment;

        private struct ModeMap
        {
            public byte Code;
            public Mode Mode;
        }

        private struct PaletteMap
        {
            public byte Code;
            public Palette Palette;
        }
    }
}
