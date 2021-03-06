using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FilLib;
using ImageLib.Common;
using ImageLib.Gamut;
using ImageLib.Util;

namespace ImageLib.Agat
{
    public abstract class AgatImageFormatAbstr : INativeImageFormat
    {
        // Error distribution coefficients by direction: East, South-West, South, South-East
        // These values distribute error inversely related to pixel distance
        private const float _errDistrE = 0.2929f;
        private const float _errDistrSw = 0.2071f;
        private const float _errDistrS = 0.2929f;
        private const float _errDistrSe = 0.2071f;

        public IEnumerable<NativeDisplay> SupportedDisplays { get; } =
            new[] { NativeDisplay.Color, NativeDisplay.Mono, NativeDisplay.MonoA7, NativeDisplay.Meta };

        public virtual IEnumerable<NativePalette> SupportedPalettes { get; } =
            new[] { NativePalette.Agat1, NativePalette.Agat2, NativePalette.Agat3, NativePalette.Agat4 };

        protected abstract int Width { get; }
        protected abstract int Height { get; }
        protected abstract int BitsPerPixel { get; }
        protected abstract ImageMeta.Mode MetaMode { get; }
        protected virtual IGamut Gamut { get; } = new SrgbGamut();
        protected int PixelsPerByte => 8 / BitsPerPixel;
        protected int BytesPerScanline => Width / PixelsPerByte;

        private int ImageSizeInBytes => BytesPerScanline * Height;

        public AspectBitmap FromNative(NativeImage native, DecodingOptions options)
        {
            var bmp = new WriteableBitmap(Width, Height, Constants.Dpi, Constants.Dpi, PixelFormats.Bgr32, null);
            int stride = Width * 4;
            byte[] pixels = new byte[Height * stride];

            var colors = AgatColorUtils.NativeDisplayToColors(options.Display, native.Metadata);
            var paletteIndex = NativePaletteToIndex(options.Palette);

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
            Int32Rect srcRect = new Int32Rect(0, 0, Width, Height);
            bmp.WritePixels(srcRect, pixels, stride, 0);
            return AspectBitmap.FromImageAspect(bmp, 4.0 / 3.0);
        }

        public NativeImage ToNative(BitmapSource bitmap, EncodingOptions options)
        {
            var src = new BitmapPixels(bitmap);

            var currentLineErrors = new Error[Width];
            var nextLineErrors = new Error[Width];

            var paletteIndex = NativePaletteToIndex(options.Palette);
            ImageMeta meta = BuildMeta(src, options);

            var colors = AgatColorUtils.NativeDisplayToColors(options.Display, meta);
            var nativeColors = GetNativeColors(colors, paletteIndex);

            byte[] bytes = new byte[ImageSizeInBytes];
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    Rgb pixel = (x < src.Width && y < src.Height) ? src.GetPixel(x, y) : Rgb.FromRgb(0, 0, 0);

                    float r = 0, g = 0, b = 0;
                    if (options.Dither)
                    {
                        pixel = Gamut.FromSrgb(pixel);
                        r = ColorUtils.Clamp(pixel.R + currentLineErrors[x].R, 0, 255);
                        g = ColorUtils.Clamp(pixel.G + currentLineErrors[x].G, 0, 255);
                        b = ColorUtils.Clamp(pixel.B + currentLineErrors[x].B, 0, 255);
                        pixel = Rgb.FromRgb((byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b));
                    }

                    SetPixel(bytes, nativeColors, x, y, pixel);

                    if (options.Dither)
                    {
                        pixel = GetBgr32Pixel(bytes, colors, paletteIndex, x, y);
                        float re = r - pixel.R;
                        float ge = g - pixel.G;
                        float be = b - pixel.B;

                        if (x + 1 < Width)
                        {
                            AddError(re * _errDistrE, ge * _errDistrE, be * _errDistrE, ref currentLineErrors[x + 1]);
                        }
                        if (y + 1 < Height)
                        {
                            AddError(re * _errDistrS, ge * _errDistrS, be * _errDistrS, ref nextLineErrors[x]);
                            if (x - 1 >= 0)
                            {
                                AddError(re * _errDistrSw, ge * _errDistrSw, be * _errDistrSw, ref nextLineErrors[x - 1]);
                            }
                            if (x + 1 < Width)
                            {
                                AddError(re * _errDistrSe, ge * _errDistrSe, be * _errDistrSe, ref nextLineErrors[x + 1]);
                            }
                        }
                    }
                }

                currentLineErrors = nextLineErrors;
                nextLineErrors = new Error[Width];
            }

            return new NativeImage { Data = bytes, FormatHint = new FormatHint(this), Metadata = meta };
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
            int pixelIndex;
            int byteInLine = Math.DivRem(x, PixelsPerByte, out pixelIndex);
            int offset = GetLineOffset(y) + byteInLine;
            int b = offset < pixels.Count ? pixels[offset] : 0;
            b >>= (PixelsPerByte - pixelIndex - 1) * BitsPerPixel;
            b &= (1 << BitsPerPixel) - 1;
            return colors[MapColorIndexNativeToStandard(b, palette)];
        }

        private void SetPixel(IList<byte> pixels, IEnumerable<Rgb> nativeColors, int x, int y, Rgb color)
        {
            int byteInLine = Math.DivRem(x, PixelsPerByte, out var pixelIndex);
            int offset = GetLineOffset(y) + byteInLine;
            if (offset >= pixels.Count)
                return;
            int b = pixels[offset];
            int shift = (PixelsPerByte - pixelIndex - 1) * BitsPerPixel;
            int mask = ((1 << BitsPerPixel) - 1) << shift;
            int index = ColorUtils.BestMatch(color, nativeColors) << shift;
            b = b & ~mask | index;
            pixels[offset] = (byte)b;
        }

        private ImageMeta BuildMeta(BitmapPixels src, EncodingOptions options)
        {
            ImageMeta.Palette paletteType = ImageMeta.Palette.Unknown;
            uint[] customPalette = null;

            if (options.Display == NativeDisplay.Meta)
            {
                paletteType = ImageMeta.Palette.Custom;
                customPalette = BuildPalette(src, NativePaletteToIndex(options.Palette));
            }
            else
            {
                var bw = options.Display == NativeDisplay.Mono || options.Display == NativeDisplay.MonoA7;
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
                CustomPalette = customPalette,
            };
        }

        private uint[] BuildPalette(BitmapPixels src, int paletteIndex)
        {
            var paletteBuilder = new AgatPaletteBuilder();
            var colors = paletteBuilder.Build(AllPixelsForPalette(src), 1 << BitsPerPixel).ToList();

            SortPalette(colors, GetNativeColors(AgatPalettes.Color, paletteIndex));

            if (BitsPerPixel == 4)
            {
                return colors.Select(AgatColorUtils.RgbToUint).ToArray();
            }

            var palette = new uint[16];
            var i = 0;
            foreach (var color in colors)
            {
                palette[MapColorIndexNativeToStandard(i, paletteIndex)] = AgatColorUtils.RgbToUint(color);
                i++;
            }
            return palette;
        }

        private IEnumerable<Rgb> AllPixelsForPalette(BitmapPixels src)
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    if (x < src.Width && y < src.Height)
                        yield return src.GetPixel(x, y);
                    else
                        yield return Rgb.FromRgb(0, 0, 0);
                }
            }
        }

        private void SortPalette(IList<Rgb> palette, IList<Rgb> nativeColors)
        {
            int d(byte a, byte b) => (a - b) * (a - b);
            int cd(Rgb a, Rgb b) => d(a.R, b.R) + d(a.G, b.G) + d(a.B, b.B);

            void swap(int i, int j)
            {
                var tmp = palette[i];
                palette[i] = palette[j];
                palette[j] = tmp;
            }

            var repeat = true;

            // Minimize square distances between palette and native colors.
            while (repeat)
            {
                repeat = false;

                for (var i = 0; i < palette.Count - 1; i++)
                {
                    for (var j = i + 1; j < palette.Count; j++)
                    {
                        var m1 = cd(palette[i], nativeColors[i]) + cd(palette[j], nativeColors[j]);
                        var m2 = cd(palette[i], nativeColors[j]) + cd(palette[j], nativeColors[i]);
                        if (m2 < m1)
                        {
                            swap(i, j);
                            repeat = true;
                        }
                    }
                }
            }
        }

        private static void AddError(float re, float ge, float be, ref Error e)
        {
            e.R += re;
            e.G += ge;
            e.B += be;
        }

        private struct Error
        {
            public float R, G, B;
        }
    }
}
