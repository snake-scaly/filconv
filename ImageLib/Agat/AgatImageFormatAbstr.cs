using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        public virtual double Aspect => 4.0 / 3.0;
        public int ImageSizeInBytes => BytesPerScanline * Height;
        protected abstract int Width { get; }
        protected abstract int Height { get; }
        protected abstract int BitsPerPixel { get; }
        protected abstract Color[] Palette { get; }
        protected virtual IGamut Gamut { get; } = new SrgbGamut();
        protected int PixelsPerByte => 8 / BitsPerPixel;
        protected int BytesPerScanline => Width / PixelsPerByte;

        public BitmapSource FromNative(NativeImage native)
        {
            var bmp = new WriteableBitmap(Width, Height, _defaultDpi / Aspect, _defaultDpi, PixelFormats.Bgr32, null);
            int stride = Width * 4;
            byte[] pixels = new byte[Height * stride];
            for (int y = 0; y < Height; ++y)
            {
                int line = y * stride;
                for (int x = 0; x < Width; ++x)
                {
                    int pixel = line + x * 4;
                    Color c = GetBgr32Pixel(native.Data, x, y);
                    pixels[pixel] = c.B;
                    pixels[pixel + 1] = c.G;
                    pixels[pixel + 2] = c.R;
                }
            }
            Int32Rect srcRect = new Int32Rect(0, 0, Width, Height);
            bmp.WritePixels(srcRect, pixels, stride, 0);
            return bmp;
        }

        public NativeImage ToNative(BitmapSource bitmap, EncodingOptions options)
        {
            var src = new BitmapPixels(bitmap);

            var currentLineErrors = new Error[Width];
            var nextLineErrors = new Error[Width];

            byte[] bytes = new byte[ImageSizeInBytes];
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    Color pixel = (x < src.Width && y < src.Height) ? src.GetPixel(x, y) : Colors.Black;

                    float r = 0, g = 0, b = 0;
                    if (options.Dither)
                    {
                        pixel = Gamut.FromSrgb(pixel);
                        r = Clamp(pixel.R + currentLineErrors[x].R);
                        g = Clamp(pixel.G + currentLineErrors[x].G);
                        b = Clamp(pixel.B + currentLineErrors[x].B);
                        pixel = Color.FromRgb(Round(r), Round(g), Round(b));
                    }

                    SetPixel(bytes, x, y, pixel);

                    if (options.Dither)
                    {
                        pixel = GetBgr32Pixel(bytes, x, y);
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

        Color GetBgr32Pixel(byte[] pixels, int x, int y)
        {
            ValidateCoordinates(x, y);
            int pixelIndex;
            int byteInLine = Math.DivRem(x, PixelsPerByte, out pixelIndex);
            int offset = GetLineOffset(y) + byteInLine;
            int b = offset < pixels.Length ? pixels[offset] : 0;
            b >>= (PixelsPerByte - pixelIndex - 1) * BitsPerPixel;
            b &= (1 << BitsPerPixel) - 1;
            return Palette[b];
        }

        void SetPixel(byte[] pixels, int x, int y, Color color)
        {
            ValidateCoordinates(x, y);
            int pixelIndex;
            int byteInLine = Math.DivRem(x, PixelsPerByte, out pixelIndex);
            int offset = GetLineOffset(y) + byteInLine;
            if (offset < pixels.Length)
            {
                int b = pixels[offset];
                int shift = (PixelsPerByte - pixelIndex - 1) * BitsPerPixel;
                int mask = ((1 << BitsPerPixel) - 1) << shift;
                int index = ColorUtils.BestMatch(color, Palette) << shift;
                b = b & ~mask | index;
                pixels[offset] = (byte)b;
            }
        }

        protected virtual int GetLineOffset(int y)
        {
            return y * BytesPerScanline;
        }

        void ValidateCoordinates(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x", x, "X must be between 0 and " + Width);
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y", y, "Y must be between 0 and " + Height);
        }

        public int ComputeMatchScore(NativeImage native)
        {
            return NativeImageFormatUtils.ComputeMatch(native, ImageSizeInBytes);
        }

        static float Clamp(float c)
        {
            return Math.Min(Math.Max(c, 0), 255);
        }

        static byte Round(float c)
        {
            return (byte)Math.Round(c);
        }

        static void AddError(float re, float ge, float be, ref Error e)
        {
            e.R += re;
            e.G += ge;
            e.B += be;
        }

        struct Error
        {
            public float R, G, B;
        }
    }
}
