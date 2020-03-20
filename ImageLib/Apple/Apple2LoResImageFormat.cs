using ImageLib.Util;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageLib.Apple
{
    public class Apple2LoResImageFormat : NativeImageFormat
    {
        // Lo-res mode is always 40x24 blocks. Each block is either 1x2 pixels,
        // or 2x2 pixels in double lo-res.
        private const int _nativeWidth = 40;
        private const int _nativeHeight = 24;
        private const int _nativePage = 1024;
        private int _totalBytes;

        private const int _redByteOffset = 2;
        private const int _greenByteOffset = 1;
        private const int _blueByteOffset = 0;

        private readonly bool _doubleResolution;

        private readonly int _bmpBlockWidth;
        private const int _bmpBlockHeight = 2; // A lo-res line is always 2 pixels tall.
        private readonly int _bmpWidth;
        private readonly int _bmpHeight;
        private const double _bmpDpi = 96;

        public double Aspect
        {
            get { return (4.0 * _bmpHeight) / (3.0 * _bmpWidth); }
        }

        public Apple2LoResImageFormat(bool doubleResolution)
        {
            _doubleResolution = doubleResolution;
            _totalBytes = doubleResolution ? _nativePage * 2 : _nativePage;
            _bmpBlockWidth = doubleResolution ? 2 : 1;
            _bmpWidth = _nativeWidth * _bmpBlockWidth;
            _bmpHeight = _nativeHeight * _bmpBlockHeight;
        }

        public BitmapSource FromNative(NativeImage native)
        {
            PixelFormat bmpPixelFormat = PixelFormats.Bgr32;
            const int bmpBPP = 4;

            int bmpStride = _bmpWidth * bmpBPP;

            byte[] pixels = new byte[_bmpHeight * bmpStride];

            for (int y = 0; y < _nativeHeight; ++y)
            {
                int inLineOffset = Apple2Utils.GetTextLineOffset(y);
                int outLineOffset = y * _bmpBlockHeight * bmpStride;

                for (int x = 0; x < _nativeWidth; ++x)
                {
                    int inPixelOffset = inLineOffset + x;
                    int outPixelOffset = outLineOffset + x * _bmpBlockWidth * bmpBPP;

                    if (inPixelOffset >= native.Data.Length)
                        break;

                    if (_doubleResolution)
                    {
                        PutPixels(native.Data[inPixelOffset], pixels, outPixelOffset + bmpBPP, bmpStride);
                        if (inPixelOffset + _nativePage < native.Data.Length)
                            PutPixels(native.Data[inPixelOffset + _nativePage], pixels, outPixelOffset, bmpStride);
                    }
                    else
                    {
                        PutPixels(native.Data[inPixelOffset], pixels, outPixelOffset, bmpStride);
                    }
                }
            }

            var bmp = new WriteableBitmap(_bmpWidth, _bmpHeight, _bmpDpi / Aspect, _bmpDpi, bmpPixelFormat, null);
            var srcRect = new Int32Rect(0, 0, _bmpWidth, _bmpHeight);
            bmp.WritePixels(srcRect, pixels, bmpStride, 0);
            return bmp;
        }

        public NativeImage ToNative(BitmapSource bitmap, EncodingOptions options)
        {
            int w = Math.Min(bitmap.PixelWidth, _bmpWidth);
            int h = Math.Min(bitmap.PixelHeight, _bmpHeight);
            var src = new BitmapPixels(bitmap);

            byte[] nativePixels = new byte[_totalBytes];

            for (int y = 0; y < h; y += _bmpBlockHeight)
            {
                int nativeLineOffset = Apple2Utils.GetTextLineOffset(y / _bmpBlockHeight);

                for (int x = 0; x < w; x += _bmpBlockWidth)
                {
                    int nativePixelOffset = nativeLineOffset + x / _bmpBlockWidth;
                    if (_doubleResolution)
                    {
                        nativePixels[nativePixelOffset + _nativePage] = GetPixels(src, x, y);
                        if (x + 1 < w)
                            nativePixels[nativePixelOffset] = GetPixels(src, x + 1, y);
                    }
                    else
                    {
                        nativePixels[nativePixelOffset] = GetPixels(src, x, y);
                    }
                }
            }

            return new NativeImage(nativePixels, new FormatHint(this));
        }

        public int ComputeMatchScore(NativeImage native)
        {
            return NativeImageFormatUtils.ComputeMatch(native, _totalBytes);
        }

        // Write a 1x2 pixel block at the specified offset.
        private void PutPixels(int nativeByte, byte[] pixels, int offset, int stride)
        {
            PutPixel(nativeByte & 0xF, pixels, offset);
            PutPixel((nativeByte >> 4) & 0xF, pixels, offset + stride);
        }

        // Write a color pixel at the specified offset.
        private void PutPixel(int colorIndex, byte[] pixels, int offset)
        {
            Color c = Apple2Palettes.LoRes16[colorIndex];
            pixels[offset + _redByteOffset] = c.R;
            pixels[offset + _greenByteOffset] = c.G;
            pixels[offset + _blueByteOffset] = c.B;
        }

        // Read a 1x2 pixel block at the specified coordinates.
        private byte GetPixels(BitmapPixels pixels, int x, int y)
        {
            Color c1 = pixels.GetPixel(x, y);
            Color c2 = y < pixels.Height ? pixels.GetPixel(x, y + 1) : new Color();
            int v1 = ColorUtils.BestMatch(c1, Apple2Palettes.LoRes16);
            int v2 = ColorUtils.BestMatch(c2, Apple2Palettes.LoRes16);
            return (byte)(v1 | (v2 << 4));
        }
    }
}
