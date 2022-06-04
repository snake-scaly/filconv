using ImageLib.Util;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FilLib;
using ImageLib.Common;

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

        public override NativeImage ToNative(BitmapSource bitmap, EncodingOptions options)
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

            var bmp = new WriteableBitmap(
                _bmpWidth, _bmpHeight, Constants.Dpi, Constants.Dpi, bmpPixelFormat, null);
            var srcRect = new Int32Rect(0, 0, _bmpWidth, _bmpHeight);
            bmp.WritePixels(srcRect, pixels, bmpStride, 0);
            return AspectBitmap.FromImageAspect(bmp, 4.0 / 3.0);
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
            Rgb c = Apple2Palettes.LoRes16[colorIndex];
            pixels[offset + _redByteOffset] = c.R;
            pixels[offset + _greenByteOffset] = c.G;
            pixels[offset + _blueByteOffset] = c.B;
        }

        // Read a 1x2 pixel block at the specified coordinates.
        private byte GetPixels(BitmapPixels pixels, int x, int y)
        {
            Rgb c1 = pixels.GetPixel(x, y);
            Rgb c2 = y < pixels.Height ? pixels.GetPixel(x, y + 1) : new Rgb();
            int v1 = ColorUtils.BestMatch(c1, Apple2Palettes.LoRes16);
            int v2 = ColorUtils.BestMatch(c2, Apple2Palettes.LoRes16);
            return (byte)(v1 | (v2 << 4));
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
    }
}
