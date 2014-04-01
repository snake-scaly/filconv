using ImageLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageLib.Apple
{
    public class Apple2LoResImageFormat : NativeImageFormat
    {
        private const int _width = 40;
        private const int _height = 48;
        private const double _dpi = 96;

        private static readonly PixelFormat _pixelFormat = PixelFormats.Bgr32;
        private const int _bytesPerPixel = 4;
        private const int _halfByteShift = 4;
        private const int _halfByteMask = 15;
        private const int _oddMask = 1;

        private const int _pixelLinesPerByteLine = 2;
        private const int _byteLinesPerBlock = 3;
        private const int _bytesPerBlock = 128;
        private const int _totalBlocks = 8;
        private const int _totalBytes = _bytesPerBlock * _totalBlocks;

        private const int _redByteOffset = 2;
        private const int _greenByteOffset = 1;
        private const int _blueByteOffset = 0;

        public double Aspect
        {
            get { return (4.0 * _height) / (3.0 * _width); }
        }

        public BitmapSource FromNative(NativeImage native)
        {
            int stride = _width * _bytesPerPixel;
            byte[] pixels = new byte[_height * stride];

            for (int y = 0; y < _height; ++y)
            {
                int inLineOffset = Apple2Utils.GetLoResLineOffset(y);
                if (inLineOffset >= native.Data.Length)
                    continue;

                int shift = _halfByteShift * (y & _oddMask);
                int outLineOffset = y * stride;

                for (int x = 0; x < _width; ++x)
                {
                    int inPixelOffset = inLineOffset + x;
                    if (inPixelOffset >= native.Data.Length)
                        break;

                    int pixelValue = (native.Data[inPixelOffset] >> shift) & _halfByteMask;
                    Color c = Apple2Palettes.LoRes16[pixelValue];

                    int outPixelOffset = outLineOffset + x * _bytesPerPixel;
                    pixels[outPixelOffset + _redByteOffset] = c.R;
                    pixels[outPixelOffset + _greenByteOffset] = c.G;
                    pixels[outPixelOffset + _blueByteOffset] = c.B;
                }
            }

            var bmp = new WriteableBitmap(_width, _height, _dpi / Aspect, _dpi, _pixelFormat, null);
            var srcRect = new Int32Rect(0, 0, _width, _height);
            bmp.WritePixels(srcRect, pixels, stride, 0);
            return bmp;
        }

        public NativeImage ToNative(BitmapSource bitmap, EncodingOptions options)
        {
            int w = Math.Min(bitmap.PixelWidth, _width);
            int h = Math.Min(bitmap.PixelHeight, _height);
            var src = new BitmapPixels(bitmap);

            byte[] nativePixels = new byte[_totalBytes];

            for (int y = 0; y < h; ++y)
            {
                int nativeLineOffset = Apple2Utils.GetLoResLineOffset(y);
                int shift = _halfByteShift * (y & _oddMask);

                for (int x = 0; x < w; ++x)
                {
                    int nativePixelOffset = nativeLineOffset + x;
                    int pixelValue = ColorUtils.BestMatch(src.GetPixel(x, y), Apple2Palettes.LoRes16);
                    nativePixels[nativePixelOffset] |= (byte)(pixelValue << shift);
                }
            }

            return new NativeImage(nativePixels, new FormatHint(this));
        }

        public int ComputeMatchScore(NativeImage native)
        {
            return NativeImageFormatUtils.ComputeMatch(native, _totalBytes);
        }
    }
}
