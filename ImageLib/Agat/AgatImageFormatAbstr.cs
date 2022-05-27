using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FilLib;
using ImageLib.Gamut;
using ImageLib.Util;

namespace ImageLib.Agat
{
    public abstract class AgatImageFormatAbstr : INativeImageFormat
    {
        const double _defaultDpi = 96;

        // Error distribution coefficients by direction: East, South-West, South, South-East
        // These values distribute error inversely related to pixel distance
        const float _errDistrE = 0.2929f;
        const float _errDistrSw = 0.2071f;
        const float _errDistrS = 0.2929f;
        const float _errDistrSe = 0.2071f;

        public int ImageSizeInBytes => BytesPerScanline * Height;
        protected abstract int Width { get; }
        protected abstract int Height { get; }
        protected abstract int BitsPerPixel { get; }
        protected virtual IGamut Gamut { get; } = new SrgbGamut();
        protected int PixelsPerByte => 8 / BitsPerPixel;
        protected int BytesPerScanline => Width / PixelsPerByte;

        public AspectBitmap FromNative(NativeImage native)
        {
            var bmp = new WriteableBitmap(Width, Height, _defaultDpi, _defaultDpi, PixelFormats.Bgr32, null);
            int stride = Width * 4;
            byte[] pixels = new byte[Height * stride];
            int palette = 0;

            Rgb[] colors;
            if (native.Metadata?.PaletteType == ImageMeta.Palette.Custom)
            {
                colors = native.Metadata.CustomPalette.Select(UintToRgb).ToArray();
            }
            else
            {
                var paletteType = native.Metadata?.PaletteType ?? ImageMeta.Palette.Unknown;
                DecodePaletteVariant(paletteType, out var bw, out palette);
                colors = bw ? AgatPalette.MonoPalette : AgatPalette.ColorPalette;
            }

            for (int y = 0; y < Height; ++y)
            {
                int line = y * stride;
                for (int x = 0; x < Width; ++x)
                {
                    int pixel = line + x * 4;
                    Rgb c = GetBgr32Pixel(native.Data, colors, palette, x, y);
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

            var colors = AgatPalette.ColorPalette;
            var nativeColors = Enumerable
                .Range(0, 1 << BitsPerPixel)
                .Select(i => colors[MapColorIndexNativeToStandard(i, 0)])
                .ToArray();

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
                        pixel = GetBgr32Pixel(bytes, colors, 0, x, y);
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

            return new NativeImage { Data = bytes, FormatHint = new FormatHint(this) };
        }

        public virtual int ComputeMatchScore(NativeImage native)
        {
            return NativeImageFormatUtils.ComputeMatch(native, ImageSizeInBytes);
        }

        protected virtual int GetLineOffset(int y)
        {
            return y * BytesPerScanline;
        }

        /// Map a format-specific color index to the standard 16 color palette.
        /// <param name="index">index in the image's native bit per pixel</param>
        /// <param name="palette">one of the 4 hardware palettes</param>
        protected abstract int MapColorIndexNativeToStandard(int index, int palette);

        /// Get a palette variant for a mode. Unknown should retrieve the default palette.
        void DecodePaletteVariant(ImageMeta.Palette variant, out bool bw, out int palette)
        {
            switch (variant)
            {
                default:
                    bw = false;
                    palette = 0;
                    break;
                case ImageMeta.Palette.Agat_2:
                    bw = false;
                    palette = 1;
                    break;
                case ImageMeta.Palette.Agat_3:
                    bw = false;
                    palette = 2;
                    break;
                case ImageMeta.Palette.Agat_4:
                    bw = false;
                    palette = 3;
                    break;
                case ImageMeta.Palette.Agat_1_Gray:
                    bw = true;
                    palette = 0;
                    break;
                case ImageMeta.Palette.Agat_2_Gray:
                    bw = true;
                    palette = 1;
                    break;
                case ImageMeta.Palette.Agat_3_Gray:
                    bw = true;
                    palette = 2;
                    break;
                case ImageMeta.Palette.Agat_4_Gray:
                    bw = true;
                    palette = 3;
                    break;
            }
        }

        Rgb GetBgr32Pixel(byte[] pixels, Rgb[] colors, int palette, int x, int y)
        {
            int pixelIndex;
            int byteInLine = Math.DivRem(x, PixelsPerByte, out pixelIndex);
            int offset = GetLineOffset(y) + byteInLine;
            int b = offset < pixels.Length ? pixels[offset] : 0;
            b >>= (PixelsPerByte - pixelIndex - 1) * BitsPerPixel;
            b &= (1 << BitsPerPixel) - 1;
            return colors[MapColorIndexNativeToStandard(b, palette)];
        }

        void SetPixel(byte[] pixels, Rgb[] nativeColors, int x, int y, Rgb color)
        {
            int pixelIndex;
            int byteInLine = Math.DivRem(x, PixelsPerByte, out pixelIndex);
            int offset = GetLineOffset(y) + byteInLine;
            if (offset < pixels.Length)
            {
                int b = pixels[offset];
                int shift = (PixelsPerByte - pixelIndex - 1) * BitsPerPixel;
                int mask = ((1 << BitsPerPixel) - 1) << shift;
                int index = ColorUtils.BestMatch(color, nativeColors) << shift;
                b = b & ~mask | index;
                pixels[offset] = (byte)b;
            }
        }

        static void AddError(float re, float ge, float be, ref Error e)
        {
            e.R += re;
            e.G += ge;
            e.B += be;
        }

        static Rgb UintToRgb(uint i)
        {
            var r = (byte)(i >> 16);
            var g = (byte)(i >> 8);
            var b = (byte)(i >> 0);
            return Rgb.FromRgb(r, g, b);
        }

        struct Error
        {
            public float R, G, B;
        }
    }
}
