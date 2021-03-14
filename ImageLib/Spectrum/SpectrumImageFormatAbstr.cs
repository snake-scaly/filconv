using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace ImageLib.Spectrum
{
    public abstract class SpectrumImageFormatAbstr : INativeImageFormat
    {
        private const double _defaultDpi = 96;

        protected const int _bytesPerLine = 32;
        protected const int _paletteBytesPerLine = 32;
        protected const int _width = 256;
        protected const int _height = 192;
        protected const int _colorLines = 24;
        protected const int _paletteOffset = _height * _bytesPerLine;
        protected const int _paletteSize = _colorLines * _bytesPerLine;
        protected const int _totalBytes = (_height + _colorLines) * _bytesPerLine;

        public double Aspect
        {
            get { return 1.0; }
        }

        public BitmapSource FromNative(NativeImage native)
        {
            int dstStride = _width * 4;
            int size = dstStride * _height;
            byte[] pixels = new byte[size];

            for (int y = 0; y < _height; ++y)
            {
                int srcLine = GetLineOffset(y);
                int srcColorLine = GetColorOffset(y);
                int dstLine = dstStride * y;
                for (int x = 0; x < _bytesPerLine; ++x)
                {
                    int bw = GetBwSafe(native.Data, srcLine + x);
                    int color = GetColorSafe(native.Data, srcColorLine + x);
                    for (int i = 0; i < 8; ++i)
                    {
                        Color pixelColor = GetPixelColor((bw & (0x80 >> i)) != 0, color);
                        int dstOffset = dstLine + (x * 8 + i) * 4;
                        pixels[dstOffset] = pixelColor.B;
                        pixels[dstOffset + 1] = pixelColor.G;
                        pixels[dstOffset + 2] = pixelColor.R;
                    }
                }
            }

            var bmp = new WriteableBitmap(_width, _height, _defaultDpi / Aspect, _defaultDpi, PixelFormats.Bgr32, null);
            bmp.WritePixels(new Int32Rect(0, 0, _width, _height), pixels, dstStride, 0);
            return bmp;
        }

        private static int GetBwSafe(byte[] data, int offset)
        {
            return (offset >= 0 && offset < data.Length) ? data[offset] : 0;
        }

        private static int GetColorSafe(byte[] data, int offset)
        {
            return (offset >= 0 && offset < data.Length) ? data[offset] : 0x47;
        }

        protected abstract int GetLineOffset(int y);

        private static int GetColorOffset(int y)
        {
            return _bytesPerLine * _height + _paletteBytesPerLine * (y / 8);
        }

        private Color GetPixelColor(bool isPixelSet, int colorSelector)
        {
            int value = (colorSelector & 0x40) != 0 ? 255 : 217;
            int rgb = isPixelSet ? colorSelector : (colorSelector >> 3);
            int g = (rgb & 4) != 0 ? value : 0;
            int r = (rgb & 2) != 0 ? value : 0;
            int b = (rgb & 1) != 0 ? value : 0;
            return Color.FromRgb((byte)r, (byte)g, (byte)b);
        }

        public NativeImage ToNative(BitmapSource bitmap, EncodingOptions options)
        {
            throw new NotSupportedException("Conversion to Spectrum format is not supported");
        }

        public abstract int ComputeMatchScore(NativeImage native);
    }
}
