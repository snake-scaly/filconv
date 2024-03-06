using System;
using System.Collections.Generic;
using System.Linq;
using FilLib;
using ImageLib.ColorManagement;
using ImageLib.Quantization;
using ImageLib.Util;

namespace ImageLib.Agat
{
    public abstract class AgatImageFormatAbstr : INativeImageFormat
    {
        private static readonly NativePalette[] _displayPalettes =
            { NativePalette.Agat1, NativePalette.Agat2, NativePalette.Agat3, NativePalette.Agat4 };

        public IEnumerable<NativeDisplay> SupportedDisplays { get; } =
            new[] { NativeDisplay.Color, NativeDisplay.Mono, NativeDisplay.Meta };

        public IEnumerable<NativeDisplay> SupportedEncodingDisplays { get; } =
            new[] { NativeDisplay.Color, NativeDisplay.Mono, NativeDisplay.Meta };

        protected abstract int Width { get; }
        protected abstract int Height { get; }
        protected abstract int BitsPerPixel { get; }
        protected abstract ImageMeta.Mode MetaMode { get; }
        protected int PixelsPerByte => 8 / BitsPerPixel;
        protected int BytesPerScanline => Width / PixelsPerByte;

        private int ImageSizeInBytes => BytesPerScanline * Height;

        public AspectBitmap FromNative(NativeImage native, DecodingOptions options)
        {
            int stride = Width * 4;
            byte[] pixels = new byte[Height * stride];

            var colors = AgatColorUtils.NativeDisplayToColors(options.Display, native.Metadata);
            var paletteIndex = options.Display == NativeDisplay.Meta ? 0 : NativePaletteToIndex(options.Palette);

            for (int y = 0; y < Height; ++y)
            {
                int line = y * stride;
                for (int x = 0; x < Width; ++x)
                {
                    int pixel = line + x * 4;
                    Rgb c = GetBgr32Pixel(native.Data, colors, paletteIndex, x, y);
                    pixels[pixel] = c.B;
                    pixels[pixel + 1] = c.G;
                    pixels[pixel + 2] = c.R;
                }
            }
            return AspectBitmap.FromImageAspect(new Bgr32BitmapData(pixels, Width, Height), 4.0 / 3.0);
        }

        public NativeImage ToNative(IReadOnlyPixels src, EncodingOptions options)
        {
            var paletteIndex = options.Display == NativeDisplay.Meta ? 0 : NativePaletteToIndex(options.Palette);
            var palette = BuildDisplayPalette(src, options.Display, paletteIndex);
            
            byte[] bytes = new byte[ImageSizeInBytes];
            var dst = new WriteableNative(Width, Height, bytes, this);

            var quantizer = options.Dither ? (IQuantizer)new FloydSteinbergDithering() : new NearestColorQuantizer();
            quantizer.Quantize(src, dst, palette);

            ImageMeta meta = BuildMeta(options, palette);
            return new NativeImage { Data = bytes, Metadata = meta };
        }

        public int ComputeMatchScore(NativeImage native)
        {
            if (native.Metadata?.DisplayMode == MetaMode)
                return NativeImageFormatUtils.MetaMatchScore;
            return NativeImageFormatUtils.ComputeMatch(native, ImageSizeInBytes);
        }

        public DecodingOptions GetDefaultDecodingOptions(NativeImage native)
        {
            if (native.Metadata == null)
                return new DecodingOptions { Display = NativeDisplay.Color, Palette = NativePalette.Agat1 };

            if (native.Metadata.PaletteType == ImageMeta.Palette.Custom)
                return new DecodingOptions { Display = NativeDisplay.Meta, Palette = NativePalette.Agat1 };

            DecodePaletteVariant(native.Metadata.PaletteType, out var bw, out var palette);

            return new DecodingOptions
            {
                Display = bw ? NativeDisplay.Mono : NativeDisplay.Color,
                Palette = palette
            };
        }

        public virtual IEnumerable<NativePalette> GetSupportedPalettes(NativeDisplay display)
        {
            if (display == NativeDisplay.Meta)
                return null;
            return _displayPalettes;
        }

        protected virtual int GetLineOffset(int y)
        {
            return y * BytesPerScanline;
        }

        /// Map a format-specific color index to the standard 16 color palette.
        /// <param name="index">index in the image's native bit per pixel</param>
        /// <param name="palette">one of the 4 hardware palettes</param>
        protected abstract int MapColorIndexNativeToStandard(int index, int palette);

        private static int NativePaletteToIndex(NativePalette palette)
        {
            switch (palette)
            {
                case NativePalette.Default:
                case NativePalette.Agat1: return 0;
                case NativePalette.Agat2: return 1;
                case NativePalette.Agat3: return 2;
                case NativePalette.Agat4: return 3;
                default: throw new ArgumentException($"Unsupported palette {palette:G}", nameof(palette));
            }
        }

        /// Get a palette variant for a mode. Unknown should retrieve the default palette.
        private void DecodePaletteVariant(ImageMeta.Palette variant, out bool bw, out NativePalette palette)
        {
            switch (variant)
            {
                default:
                    bw = false;
                    palette = NativePalette.Agat1;
                    break;
                case ImageMeta.Palette.Agat_2:
                    bw = false;
                    palette = NativePalette.Agat2;
                    break;
                case ImageMeta.Palette.Agat_3:
                    bw = false;
                    palette = NativePalette.Agat3;
                    break;
                case ImageMeta.Palette.Agat_4:
                    bw = false;
                    palette = NativePalette.Agat4;
                    break;
                case ImageMeta.Palette.Agat_1_Gray:
                    bw = true;
                    palette = NativePalette.Agat1;
                    break;
                case ImageMeta.Palette.Agat_2_Gray:
                    bw = true;
                    palette = NativePalette.Agat2;
                    break;
                case ImageMeta.Palette.Agat_3_Gray:
                    bw = true;
                    palette = NativePalette.Agat3;
                    break;
                case ImageMeta.Palette.Agat_4_Gray:
                    bw = true;
                    palette = NativePalette.Agat4;
                    break;
            }
        }

        private IList<Rgb> GetNativeColors(IList<Rgb> colors, int paletteIndex)
        {
            var n = 1 << BitsPerPixel;
            var native = new Rgb[n];
            for (var i = 0; i < n; i++)
                native[i] = colors[MapColorIndexNativeToStandard(i, paletteIndex)];
            return native;
        }

        private Rgb GetBgr32Pixel(IList<byte> pixels, IList<Rgb> colors, int palette, int x, int y)
        {
            int byteInLine = Math.DivRem(x, PixelsPerByte, out var pixelIndex);
            int offset = GetLineOffset(y) + byteInLine;
            int b = offset < pixels.Count ? pixels[offset] : 0;
            b >>= (PixelsPerByte - pixelIndex - 1) * BitsPerPixel;
            b &= (1 << BitsPerPixel) - 1;
            return colors[MapColorIndexNativeToStandard(b, palette)];
        }

        private Palette BuildDisplayPalette(IReadOnlyPixels src, NativeDisplay display, int paletteIndex)
        {
            if (display == NativeDisplay.Meta)
                return BuildCustomPalette(src);
            return GetNativePalette(display, paletteIndex);
        }

        private static IList<Rgb> GetDisplayColors(NativeDisplay display)
        {
            switch (display)
            {
                case NativeDisplay.Mono: return AgatHardwareColors.Mono;
                default: return AgatHardwareColors.Color;
            }
        }

        private Palette GetNativePalette(NativeDisplay display, int paletteIndex)
        {
            var displayColors = GetDisplayColors(display);
            return new Palette(GetNativeColors(displayColors, paletteIndex));
        }

        private Palette BuildCustomPalette(IReadOnlyPixels src)
        {
            var k = 1 << BitsPerPixel;

            var paletteBuilder = new AgatPaletteBuilder();
            var palette = paletteBuilder.Build(src.AllPixelsInRect(0, 0, Width, Height), k);

            // 2BPP color palettes are pure chaos anyway, so sort custom palette
            // to look better in grayscale when BPP is low.
            var display = BitsPerPixel > 2 ? NativeDisplay.Color : NativeDisplay.Mono;
            var template = GetNativePalette(display, 0);
            palette.Sort(template);

            return palette;
        }

        private ImageMeta BuildMeta(EncodingOptions options, Palette palette)
        {
            ImageMeta.Palette paletteType = ImageMeta.Palette.Unknown;
            var pal16 = GetDisplayColors(options.Display);

            if (options.Display == NativeDisplay.Meta)
            {
                paletteType = ImageMeta.Palette.Custom;
                pal16 = new Rgb[16];
                for (var i = (1 << BitsPerPixel) - 1; i >= 0; i--)
                {
                    var j = MapColorIndexNativeToStandard(i, 0);
                    pal16[j] = palette[i].Value;
                }
            }
            else
            {
                var bw = options.Display == NativeDisplay.Mono;
                switch (options.Palette)
                {
                    case NativePalette.Default:
                    case NativePalette.Agat1:
                        paletteType = bw ? ImageMeta.Palette.Agat_1_Gray : ImageMeta.Palette.Agat_1;
                        break;
                    case NativePalette.Agat2:
                        paletteType = bw ? ImageMeta.Palette.Agat_2_Gray : ImageMeta.Palette.Agat_2;
                        break;
                    case NativePalette.Agat3:
                        paletteType = bw ? ImageMeta.Palette.Agat_3_Gray : ImageMeta.Palette.Agat_3;
                        break;
                    case NativePalette.Agat4:
                        paletteType = bw ? ImageMeta.Palette.Agat_4_Gray : ImageMeta.Palette.Agat_4;
                        break;
                }
            }

            return new ImageMeta
            {
                DisplayMode = MetaMode,
                PaletteType = paletteType,
                CustomPalette = pal16.Select(AgatColorUtils.RgbToUint).ToArray(),
            };
        }

        private class WriteableNative : IWriteablePixels<int>
        {
            private readonly AgatImageFormatAbstr _format;
            private readonly IList<byte> _pixels;

            public WriteableNative(int width, int height, IList<byte> pixels, AgatImageFormatAbstr format)
            {
                Width = width;
                Height = height;
                _pixels = pixels;
                _format = format;
            }

            public int Width { get; }
            public int Height { get; }

            public void SetPixel(int x, int y, int pixel)
            {
                int byteInLine = Math.DivRem(x, _format.PixelsPerByte, out var pixelIndex);
                int offset = _format.GetLineOffset(y) + byteInLine;
                if (offset >= _pixels.Count)
                    return;
                int b = _pixels[offset];
                int shift = (_format.PixelsPerByte - pixelIndex - 1) * _format.BitsPerPixel;
                int mask = ((1 << _format.BitsPerPixel) - 1) << shift;
                b = b & ~mask | (pixel << shift);
                _pixels[offset] = (byte)b;
            }
        }
    }
}
