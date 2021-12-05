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

        public string Charset { get; set; }

        public string Comment { get; set; }

        public static bool TryParse(Fil fil, out ImageMeta result)
        {
            result = null;

            if (!fil.Type.HasAddrSize || fil.Sectors.Length == 0 || (fil.Sectors.Length & 0xFF) != 0)
                return false;

            var metaOffset = (fil.Sectors.Length - 1) / 256 * 256;

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

        private static Mode ParseDisplayMode(byte[] data, int metaOffset)
        {
            byte code = data[metaOffset + 6];

            // GigaScreen is Agat
            if ((code & 0x0F) == 0x0D)
                code &= 0xF0;

            switch (code)
            {
                case 0x10: return Mode.Agat_256_256_Mono;
                case 0x40: return Mode.Agat_64_64_Pal16;
                case 0x50: return Mode.Agat_128_128_Pal16;
                case 0x60: return Mode.Agat_256_256_Pal4;
                case 0x70: return Mode.Agat_512_256_Mono;
                case 0x80: return Mode.Agat_128_256_Pal16;
                case 0x90: return Mode.Agat_280_192_AppleHiRes;
                case 0x21: return Mode.Agat_T32_Pal16;
                case 0x31: return Mode.Agat_T64_Mono;
                case 0x41: return Mode.Agat_T64_Pal16;
                case 0xA1: return Mode.Agat_T32_Pal16_FgBg;
                case 0xC1: return Mode.Agat_T64_Pal16_FgBg;
                case 0x9A: return Mode.Apple_280_192_HiRes;
                case 0xAA: return Mode.Apple_T40;
                case 0xBA: return Mode.Apple_T80;
                case 0xCA: return Mode.Apple_40_48_LoRes;
                case 0xDA: return Mode.Apple_80_48_DoubleLoRes;
                case 0xEA: return Mode.Apple_140_192_DoubleHiResColor;
                case 0xFA: return Mode.Apple_560_192_DoubleHiResMono;
                default: return Mode.Unknown;
            }
        }

        private static Palette ParsePaletteType(byte[] data, int metaOffset)
        {
            switch (data[metaOffset + 7] >> 4)
            {
                case 0: return Palette.Agat_1;
                case 1: return Palette.Agat_2;
                case 2: return Palette.Agat_3;
                case 3: return Palette.Agat_4;
                case 8: return Palette.Agat_1_Gray;
                case 9: return Palette.Agat_2_Gray;
                case 10: return Palette.Agat_3_Gray;
                case 11: return Palette.Agat_4_Gray;
                case 15: return Palette.Custom;
                default: return Palette.Unknown;
            }
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
    }
}
