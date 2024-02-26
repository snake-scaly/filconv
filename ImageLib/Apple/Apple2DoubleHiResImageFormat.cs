using System;
using System.Collections.Generic;
using FilLib;
using ImageLib.Apple.BitStream;
using ImageLib.ColorManagement;
using ImageLib.Quantization;

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

        public override NativeImage ToNative(IReadOnlyPixels src, EncodingOptions options)
        {
            var data = new byte[_bytesPerHalfScreen * 2];
            var dst = new WriteableNative(data);
            var quantizer = options.Dither ? (IQuantizer)new FloydSteinbergDithering() : new NearestColorQuantizer();
            quantizer.Quantize(src, dst, Apple2HardwareColors.DoubleHiRes16);
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
                        Rgb c = Apple2HardwareColors.DoubleHiRes16[pixelValue].Value;

                        int dstPixelOffset = y * stride + (w * _pixelsPerWord + i) * bytesPerBmpPixel;
                        pixels[dstPixelOffset + 2] = c.R;
                        pixels[dstPixelOffset + 1] = c.G;
                        pixels[dstPixelOffset + 0] = c.B;
                    }
                }
            }

            return AspectBitmap.FromImageAspect(new Bgr32BitmapData(pixels, _width, _height), 4.0 / 3.0);
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

        private class WriteableNative : IWriteablePixels<int>
        {
            private readonly IList<byte> _pixels;

            public WriteableNative(IList<byte> pixels)
            {
                _pixels = pixels;
            }

            public int Width => _width;
            public int Height => _height;

            public void SetPixel(int x, int y, int pixel)
            {
                if (x < 0 || x >= Width)
                    throw new ArgumentOutOfRangeException(nameof(x));
                if (y < 0 || y >= Height)
                    throw new ArgumentOutOfRangeException(nameof(y));
                if (pixel < 0 || pixel >= 16)
                    throw new ArgumentOutOfRangeException(nameof(pixel));

                var blockOffset = Math.DivRem(x, 7, out var pixelInBlock) * 2;
                var offset = Apple2Utils.GetHiResLineOffset(y) + blockOffset;

                var block = GetBlock(offset);
                block &= ~(0xF << (4 * pixelInBlock));
                block |= pixel << (4 * pixelInBlock);
                SetBlock(offset, block);
            }

            private int GetBlock(int offset)
            {
                return
                    ((_pixels[offset] & 0x7F) << 0) |
                    ((_pixels[offset + _bytesPerHalfScreen] & 0x7F) << 7) |
                    ((_pixels[offset + 1] & 0x7F) << 14) |
                    ((_pixels[offset + _bytesPerHalfScreen + 1] & 0x7F) << 21);
            }

            private void SetBlock(int offset, int block)
            {
                _pixels[offset] = (byte)((block >> 0) & 0x7F);
                _pixels[offset + _bytesPerHalfScreen] = (byte)((block >> 7) & 0x7F);
                _pixels[offset + 1] = (byte)((block >> 14) & 0x7F);
                _pixels[offset + _bytesPerHalfScreen + 1] = (byte)((block >> 21) & 0x7F);
            }
        }
    }
}
