using System;
using System.Collections.Generic;
using FilLib;
using ImageLib.Apple.BitStream;
using ImageLib.ColorManagement;
using ImageLib.Quantization;

namespace ImageLib.Apple
{
    public class Apple2LoResImageFormat : Apple2ImageFormatAbstr
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

        public Apple2LoResImageFormat(bool doubleResolution)
        {
            _doubleResolution = doubleResolution;
            _totalBytes = doubleResolution ? _nativePage * 2 : _nativePage;
            _bmpBlockWidth = doubleResolution ? 2 : 1;
            _bmpWidth = _nativeWidth * _bmpBlockWidth;
            _bmpHeight = _nativeHeight * _bmpBlockHeight;
        }

        public override AspectBitmap FromNative(NativeImage native, DecodingOptions options)
        {
            switch (options.Display)
            {
                case NativeDisplay.Color: return NativeToColor(native);
                case NativeDisplay.Mono: return NativeToBitStream(native, new MonoPictureBuilder());
                case NativeDisplay.Artifact: return NativeToBitStream(native, new NtscPictureBuilder(0));
                default: throw new ArgumentException($"Unsupported display {options.Display:G}", nameof(options));
            }
        }

        public override NativeImage ToNative(IReadOnlyPixels src, EncodingOptions options)
        {
            byte[] nativePixels = new byte[_totalBytes];
            var dst = new WriteableNative(_doubleResolution, nativePixels);
            var quantizer = options.Dither ? (IQuantizer)new FloydSteinbergDithering() : new NearestColorQuantizer();
            quantizer.Quantize(src, dst, Apple2HardwareColors.LoRes16);
            return new NativeImage { Data = nativePixels, FormatHint = new FormatHint(this) };
        }

        public override int ComputeMatchScore(NativeImage native)
        {
            var preferredMode = _doubleResolution
                ? ImageMeta.Mode.Apple_80_48_DoubleLoRes
                : ImageMeta.Mode.Apple_40_48_LoRes;
            if (native.Metadata?.DisplayMode == preferredMode)
                return NativeImageFormatUtils.MetaMatchScore;
            return NativeImageFormatUtils.ComputeMatch(native, _totalBytes);
        }

        private AspectBitmap NativeToColor(NativeImage native)
        {
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

            return AspectBitmap.FromImageAspect(new Bgr32BitmapData(pixels, _bmpWidth, _bmpHeight), 4.0 / 3.0);
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
            Rgb c = Apple2HardwareColors.LoRes16[colorIndex].Value;
            pixels[offset + _redByteOffset] = c.R;
            pixels[offset + _greenByteOffset] = c.G;
            pixels[offset + _blueByteOffset] = c.B;
        }

        private AspectBitmap NativeToBitStream(NativeImage native, IBitStreamPictureBuilder builder)
        {
            const int width = 40;
            const int height = 24;
            const int pageSize = 1024;
            int pixelWidth = _doubleResolution ? 7 : 14;

            for (int y = 0; y < height; ++y)
            {
                int inLineOffset = Apple2Utils.GetTextLineOffset(y);

                using (IScanlineWriter l0 = builder.GetScanlineWriter(y * 8 + 0))
                using (IScanlineWriter l1 = builder.GetScanlineWriter(y * 8 + 1))
                using (IScanlineWriter l2 = builder.GetScanlineWriter(y * 8 + 2))
                using (IScanlineWriter l3 = builder.GetScanlineWriter(y * 8 + 3))
                using (IScanlineWriter l4 = builder.GetScanlineWriter(y * 8 + 4))
                using (IScanlineWriter l5 = builder.GetScanlineWriter(y * 8 + 5))
                using (IScanlineWriter l6 = builder.GetScanlineWriter(y * 8 + 6))
                using (IScanlineWriter l7 = builder.GetScanlineWriter(y * 8 + 7))
                {
                    int tick = 0;

                    var scanlines = new[] { l0, l1, l2, l3, l4, l5, l6, l7 };

                    for (int x = 0; x < width; ++x)
                    {
                        int inPixelOffset = inLineOffset + x;

                        if (inPixelOffset >= native.Data.Length)
                            break;

                        if (_doubleResolution)
                        {
                            int inPixelOffsetEven = inPixelOffset + pageSize;
                            int value = inPixelOffsetEven < native.Data.Length ? native.Data[inPixelOffsetEven] : 0;
                            WritePixelColumn(value, scanlines, pixelWidth, ref tick);
                        }

                        WritePixelColumn(native.Data[inPixelOffset], scanlines, pixelWidth, ref tick);
                    }
                }
            }

            return AspectBitmap.FromImageAspect(builder.Build(), 4.0 / 3.0);
        }

        private void WritePixelColumn(int value, IScanlineWriter[] scanlines, int pixelWidth, ref int tick)
        {
            int top = value & 0xF;
            int bottom = (value >> 4) & 0xF;

            for (int i = 0; i < pixelWidth; ++i)
            {
                int topBit = (top >> (tick & 3)) & 1;
                int bottomBit = (bottom >> (tick & 3)) & 1;

                scanlines[0].Write(topBit);
                scanlines[1].Write(topBit);
                scanlines[2].Write(topBit);
                scanlines[3].Write(topBit);

                scanlines[4].Write(bottomBit);
                scanlines[5].Write(bottomBit);
                scanlines[6].Write(bottomBit);
                scanlines[7].Write(bottomBit);

                ++tick;
            }
        }
        
        private class WriteableNative : IWriteablePixels<int>
        {
            private readonly bool _doubleResolution;
            private readonly IList<byte> _pixels;

            public WriteableNative(bool doubleResolution, IList<byte> pixels)
            {
                _doubleResolution = doubleResolution;
                _pixels = pixels;
                Width = doubleResolution ? 80 : 40;
            }

            public int Width { get; }
            public int Height => 48;

            public void SetPixel(int x, int y, int pixel)
            {
                if (x < 0 || x >= Width)
                    throw new ArgumentOutOfRangeException(nameof(x));
                if (y < 0 || y >= Height)
                    throw new ArgumentOutOfRangeException(nameof(y));
                if (pixel < 0 || pixel >= 16)
                    throw new ArgumentOutOfRangeException(nameof(pixel));

                var offset = Apple2Utils.GetTextLineOffset(y >> 1);
                if (!_doubleResolution)
                    offset += x;
                else if ((x & 1) == 0)
                    offset += _nativePage + (x >> 1);
                else
                    offset += x >> 1;

                if ((y & 1) == 0)
                    _pixels[offset] = (byte)((_pixels[offset] & 0xF0) | pixel);
                else
                    _pixels[offset] = (byte)((_pixels[offset] & 0x0F) | (pixel << 4));
            }
        }
    }
}
