using ImageLib.Util;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FilLib;
using ImageLib.Apple.BitStream;
using ImageLib.Common;

namespace ImageLib.Apple
{
    public class Apple2DoubleHiResImageFormat : Apple2ImageFormatAbstr
    {
        private const int _bytesPerHalfScreen = 8192;
        private const int _pixelsPerWord = 7;
        private const int _bitsPerApplePixel = 4;
        private const int _significantBitsPerByte = 7;
        private const int _significantBitsMask = (1 << _significantBitsPerByte) - 1;
        private const int _bytesPerWord = 2;

        private const int _width = 140;
        private const int _height = 192;

        public override AspectBitmap FromNative(NativeImage native, DecodingOptions options)
        {
            switch (options.Display)
            {
                case NativeDisplay.Color: return NativeToColor(native);
                case NativeDisplay.Mono: return NativeToBitStream(native, new MonoPictureBuilder());
                case NativeDisplay.Artifact: return NativeToBitStream(native, new NtscPictureBuilder(1));
                default: throw new ArgumentException($"Unsupported display {options.Display:G}", nameof(options));
            }
        }

        public override NativeImage ToNative(BitmapSource bitmap, EncodingOptions options)
        {
            int w = Math.Min(bitmap.PixelWidth, _width);
            int h = Math.Min(bitmap.PixelHeight, _height);
            int wordWidth = (w + _pixelsPerWord - 1) / _pixelsPerWord;

            var src = new BitmapPixels(bitmap);

            var data = new byte[_bytesPerHalfScreen * 2];

            for (int y = 0; y < h; ++y)
            {
                int lineOffset = Apple2Utils.GetHiResLineOffset(y);

                for (int i = 0; i < wordWidth; ++i)
                {
                    int word = 0;
                    int x0 = i * _pixelsPerWord;
                    int maxJ = Math.Min(_pixelsPerWord, w - x0);

                    for (int j = 0; j < maxJ; ++j)
                    {
                        int bits = ColorUtils.BestMatch(src.GetPixel(x0 + j, y), Apple2Palettes.DoubleHiRes16);
                        word |= bits << (j * _bitsPerApplePixel);
                    }

                    int wordOffsetLo = lineOffset + i * _bytesPerWord;
                    int wordOffsetHi = wordOffsetLo + _bytesPerHalfScreen;

                    data[wordOffsetLo] = (byte)(word & _significantBitsMask);
                    data[wordOffsetHi] = (byte)((word >> _significantBitsPerByte) & _significantBitsMask);
                    data[wordOffsetLo + 1] = (byte)((word >> _significantBitsPerByte * 2) & _significantBitsMask);
                    data[wordOffsetHi + 1] = (byte)((word >> _significantBitsPerByte * 3) & _significantBitsMask);
                }
            }

            return new NativeImage { Data = data, FormatHint = new FormatHint(this) };
        }

        public override int ComputeMatchScore(NativeImage native)
        {
            if (native.Metadata?.DisplayMode == ImageMeta.Mode.Apple_140_192_DoubleHiResColor)
                return NativeImageFormatUtils.MetaMatchScore;
            return NativeImageFormatUtils.ComputeMatch(native, _bytesPerHalfScreen * 2);
        }

        private AspectBitmap NativeToColor(NativeImage native)
        {
            const int bytesPerBmpPixel = 4;
            const int applePixelMask = (1 << _bitsPerApplePixel) - 1;
            const int wordsPerLine = 20;

            int stride = _width * bytesPerBmpPixel;
            var pixels = new byte[stride * _height];

            for (int y = 0; y < _height; ++y)
            {
                int lineOffset = GetLineOffset(y);
                if (lineOffset + _bytesPerHalfScreen >= native.Data.Length)
                    continue;

                for (int w = 0; w < wordsPerLine; ++w)
                {
                    int wordOffsetLo = lineOffset + w * _bytesPerWord;
                    int wordOffsetHi = wordOffsetLo + _bytesPerHalfScreen;
                    if (wordOffsetHi + _bytesPerWord > native.Data.Length)
                        break;

                    int word =
                        (native.Data[wordOffsetLo] & _significantBitsMask) |
                        ((native.Data[wordOffsetHi] & _significantBitsMask) << _significantBitsPerByte) |
                        ((native.Data[wordOffsetLo + 1] & _significantBitsMask) << _significantBitsPerByte * 2) |
                        ((native.Data[wordOffsetHi + 1] & _significantBitsMask) << _significantBitsPerByte * 3);

                    for (int i = 0; i < _pixelsPerWord; ++i)
                    {
                        int pixelValue = (word >> (i * _bitsPerApplePixel)) & applePixelMask;
                        Rgb c = Apple2Palettes.DoubleHiRes16[pixelValue];

                        int dstPixelOffset = y * stride + (w * _pixelsPerWord + i) * bytesPerBmpPixel;
                        pixels[dstPixelOffset + 2] = c.R;
                        pixels[dstPixelOffset + 1] = c.G;
                        pixels[dstPixelOffset + 0] = c.B;
                    }
                }
            }

            var bmp = new WriteableBitmap(
                _width, _height, Constants.Dpi, Constants.Dpi, PixelFormats.Bgr32, null);
            var rect = new Int32Rect(0, 0, _width, _height);
            bmp.WritePixels(rect, pixels, stride, 0);

            return AspectBitmap.FromImageAspect(bmp, 4.0 / 3.0);
        }

        private AspectBitmap NativeToBitStream(NativeImage native, IBitStreamPictureBuilder builder)
        {
            const int wordsPerLine = 20;
            const int bytesPerWord = 2;
            const int significantBitsPerByte = 7;
            const int significantBitsMask = (1 << significantBitsPerByte) - 1;

            for (int y = 0; y < _height; ++y)
            {
                int lineOffset = Apple2Utils.GetHiResLineOffset(y);
                if (lineOffset + _bytesPerHalfScreen >= native.Data.Length)
                    continue;

                using (var scanline = builder.GetScanlineWriter(y))
                {
                    for (int w = 0; w < wordsPerLine; ++w)
                    {
                        int wordOffsetLo = lineOffset + w * bytesPerWord;
                        int wordOffsetHi = wordOffsetLo + _bytesPerHalfScreen;
                        if (wordOffsetHi + bytesPerWord > native.Data.Length)
                            break;

                        int word =
                            (native.Data[wordOffsetLo] & significantBitsMask) |
                            ((native.Data[wordOffsetHi] & significantBitsMask) << significantBitsPerByte) |
                            ((native.Data[wordOffsetLo + 1] & significantBitsMask) << significantBitsPerByte * 2) |
                            ((native.Data[wordOffsetHi + 1] & significantBitsMask) << significantBitsPerByte * 3);

                        for (int i = 0; i < significantBitsPerByte * 4; ++i)
                        {
                            scanline.Write(word & 1);
                            word >>= 1;
                        }
                    }
                }
            }

            return AspectBitmap.FromImageAspect(builder.Build(), 4.0 / 3.0);
        }

        private int GetLineOffset(int lineIndex)
        {
            int block = lineIndex & 7;
            int subBlock = (lineIndex >> 3) & 7;
            int line = (lineIndex >> 6) & 3;
            return block * 1024 + subBlock * 128 + line * 40;
        }
    }
}
