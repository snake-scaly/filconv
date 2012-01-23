using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageLib
{
    public abstract class AgatImageFormatAbstr : NativeImageFormat
    {
        private const double defaultDpi = 96;

        public virtual double Aspect
        {
            get { return 4.0 / 3.0; }
        }

        public int ImageSizeInBytes
        {
            get { return BytesPerScanline * Height; }
        }

        protected abstract int Width { get; }
        protected abstract int Height { get; }
        protected abstract int BitsPerPixel { get; }
        protected abstract Color[] Palette { get; }

        protected int PixelsPerByte
        {
            get { return 8 / BitsPerPixel; }
        }

        protected int BytesPerScanline
        {
            get { return Width / PixelsPerByte; }
        }

        public BitmapSource FromNative(NativeImage native)
        {
            var bmp = new WriteableBitmap(Width, Height, defaultDpi, defaultDpi, PixelFormats.Bgr32, null);
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
            var bgr32 = new FormatConvertedBitmap(bitmap, PixelFormats.Bgr32, null, 0);

            int w = bgr32.PixelWidth;
            int h = bgr32.PixelHeight;
            int stride = w * 4;

            byte[] pixels = new byte[h * stride];
            bgr32.CopyPixels(pixels, stride, 0);

            var currentLineErrors = new Error[Width];
            var nextLineErrors = new Error[Width];

            byte[] bytes = new byte[ImageSizeInBytes];
            for (int y = 0; y < Height; ++y)
            {
                int linePos = y * stride;
                for (int x = 0; x < Width; ++x)
                {
                    int pixelPos = linePos + x * 4;
                    Color pixel = (x < w && y < h) ? GetBgr32Pixel(pixels, pixelPos) : Colors.Black;

                    double r = 0, g = 0, b = 0;
                    if (options.Dither)
                    {
                        r = Clamp(pixel.R + currentLineErrors[x].R);
                        g = Clamp(pixel.G + currentLineErrors[x].G);
                        b = Clamp(pixel.B + currentLineErrors[x].B);
                        pixel = Color.FromRgb(Round(r), Round(g), Round(b));
                    }

                    SetPixel(bytes, x, y, pixel);

                    if (options.Dither)
                    {
                        pixel = GetBgr32Pixel(bytes, x, y);
                        double re = (r - pixel.R) / 4;
                        double ge = (g - pixel.G) / 4;
                        double be = (b - pixel.B) / 4;

                        if (x + 1 < Width)
                        {
                            AddError(re, ge, be, ref currentLineErrors[x + 1]);
                        }
                        if (y + 1 < Height)
                        {
                            AddError(re, ge, be, ref nextLineErrors[x]);
                            if (x - 1 >= 0)
                            {
                                AddError(re, ge, be, ref nextLineErrors[x - 1]);
                            }
                            if (x + 1 < Width)
                            {
                                AddError(re, ge, be, ref nextLineErrors[x + 1]);
                            }
                        }
                    }
                }

                currentLineErrors = nextLineErrors;
                nextLineErrors = new Error[Width];
            }

            return new NativeImage(bytes, new FormatHint(this));
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

        static Color GetBgr32Pixel(byte[] bgr32, int offset)
        {
            return Color.FromRgb(bgr32[offset + 2], bgr32[offset + 1], bgr32[offset]);
        }

        static double Clamp(double c)
        {
            return Math.Min(Math.Max(c, 0), 255);
        }

        static byte Round(double c)
        {
            return (byte)Math.Round(c);
        }

        static void AddError(double re, double ge, double be, ref Error e)
        {
            e.R += re;
            e.G += ge;
            e.B += be;
        }

        struct Error
        {
            public double R, G, B;

            public Error(double r, double g, double b)
            {
                R = r;
                G = g;
                B = b;
            }
        }
    }
}
