using System;
using System.Collections.Generic;
using ImageLib.ColorManagement;

namespace ImageLib.Spectrum
{
    public abstract class SpectrumImageFormatAbstr : INativeImageFormat
    {
        protected const int _bytesPerLine = 32;
        protected const int _paletteBytesPerLine = 32;
        protected const int _width = 256;
        protected const int _height = 192;
        protected const int _colorLines = 24;
        protected const int _paletteOffset = _height * _bytesPerLine;
        protected const int _paletteSize = _colorLines * _bytesPerLine;
        protected const int _totalBytes = (_height + _colorLines) * _bytesPerLine;

        public IEnumerable<NativeDisplay> SupportedDisplays => null;
        public IEnumerable<NativeDisplay> SupportedEncodingDisplays => null;

        public abstract int ComputeMatchScore(NativeImage native);

        public AspectBitmap FromNative(NativeImage native, DecodingOptions options)
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
                        Rgb pixelColor = GetPixelColor((bw & (0x80 >> i)) != 0, color);
                        int dstOffset = dstLine + (x * 8 + i) * 4;
                        pixels[dstOffset] = pixelColor.B;
                        pixels[dstOffset + 1] = pixelColor.G;
                        pixels[dstOffset + 2] = pixelColor.R;
                    }
                }
            }

            return new AspectBitmap(new Bgr32BitmapData(pixels, _width, _height), 1);
        }

        public NativeImage ToNative(IReadOnlyPixels bitmap, EncodingOptions options)
        {
            throw new NotSupportedException("Conversion to Spectrum format is not supported");
        }

        public DecodingOptions GetDefaultDecodingOptions(NativeImage native) => default;
        public IEnumerable<NativePalette> GetSupportedPalettes(NativeDisplay display) => null;

        protected abstract int GetLineOffset(int y);

        private static int GetBwSafe(byte[] data, int offset)
        {
            return (offset >= 0 && offset < data.Length) ? data[offset] : 0;
        }

        private static int GetColorSafe(byte[] data, int offset)
        {
            return (offset >= 0 && offset < data.Length) ? data[offset] : 0x47;
        }

        private static int GetColorOffset(int y)
        {
            return _bytesPerLine * _height + _paletteBytesPerLine * (y / 8);
        }

        private Rgb GetPixelColor(bool isPixelSet, int colorSelector)
        {
            int value = (colorSelector & 0x40) != 0 ? 255 : 217;
            int rgb = isPixelSet ? colorSelector : (colorSelector >> 3);
            int g = (rgb & 4) != 0 ? value : 0;
            int r = (rgb & 2) != 0 ? value : 0;
            int b = (rgb & 1) != 0 ? value : 0;
            return Rgb.FromRgb((byte)r, (byte)g, (byte)b);
        }
    }
}
