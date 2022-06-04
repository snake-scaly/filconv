using System;
using System.Windows.Media.Imaging;
using FilLib;
using ImageLib.Util;

namespace ImageLib.Apple
{
    public class Apple2HiResImageFormat : Apple2ImageFormatAbstr
    {
        private const int width = 280;
        private const int height = 192;
        private const int totalBytes = 0x2000;

        private readonly Apple2TvSet tvSet;

        public Apple2HiResImageFormat(Apple2TvSet tvSet)
        {
            this.tvSet = tvSet;
        }

        public override AspectBitmap FromNative(NativeImage native, DecodingOptions options)
        {
            switch (options.Display)
            {
                case NativeDisplay.Color: return Apple2HiResSimpleRenderer.Render(native, tvSet);
                case NativeDisplay.Mono: return NativeToBitStream(native, new MonoPictureBuilder());
                case NativeDisplay.Artifact: return NativeToBitStream(native, new NtscPictureBuilder(1));
                default: throw new ArgumentException($"Unsupported display {options.Display:G}", nameof(options));
            }
        }

        public override NativeImage ToNative(BitmapSource bitmap, EncodingOptions options)
        {
            var src = new BitmapPixels(bitmap);
            var dst = new AppleScreenHiRes();
            var writer = new AppleScreenWriter(dst);

            for (int y = 0; y < src.Height; y++)
            {
                if (y >= dst.Height)
                    break;

                writer.MoveToLine(y);
                var p1 = new PixelPipe(tvSet);
                var p2 = new PixelPipe(tvSet);

                for (int x = 0; x < src.Width; x += 7)
                {
                    if (x >= dst.Width)
                        break;

                    p1.ResetError();
                    p2.ResetError();

                    for (int i = 0; i < 7; i++)
                    {
                        Rgb c = x + i < src.Width ? src.GetPixel(x + i, y) : Rgb.FromRgb(0, 0, 0);
                        bool isOdd = ((x + i) & 1) != 0;
                        p1.PutPixel(c, false, isOdd);
                        p2.PutPixel(c, true, isOdd);
                    }

                    if (p2.Err < p1.Err)
                    {
                        writer.Write(p2.Bits | 128);
                        p1 = new PixelPipe(p2);
                    }
                    else
                    {
                        writer.Write(p1.Bits);
                        p2 = new PixelPipe(p1);
                    }
                }
            }

            return new NativeImage { Data = dst.Pixels, FormatHint = new FormatHint(this) };
        }

        public override int ComputeMatchScore(NativeImage native)
        {
            if (native.Metadata?.DisplayMode == ImageMeta.Mode.Apple_280_192_HiRes)
                return NativeImageFormatUtils.MetaMatchScore;
            return NativeImageFormatUtils.ComputeMatch(native, totalBytes);
        }

        private AspectBitmap NativeToBitStream(NativeImage native, IBitStreamPictureBuilder builder)
        {
            const int bytesPerLine = 40;
            const int pixelBitsCount = 7;
            const int pixelBitsMask = (1 << pixelBitsCount) - 1;

            for (int y = 0; y < height; ++y)
            {
                int lineOffset = Apple2Utils.GetHiResLineOffset(y);
                if (lineOffset >= native.Data.Length)
                    continue;

                using (IScanlineWriter scanline = builder.GetScanlineWriter(y))
                {
                    scanline.Write(0);

                    for (int i = 0; i < bytesPerLine; ++i)
                    {
                        int bitsOffset = lineOffset + i;
                        if (bitsOffset > native.Data.Length)
                            break;

                        int palette = native.Data[bitsOffset] >> pixelBitsCount;
                        int bits = native.Data[bitsOffset] & pixelBitsMask;
                        if (i + 1 < bytesPerLine)
                        {
                            bits |= (native.Data[bitsOffset + 1] & pixelBitsMask) << pixelBitsCount;
                        }

                        for (int tick = 0; tick < 14; ++tick)
                        {
                            int shift = (tick + palette) >> 1;
                            scanline.Write(bits >> shift);
                        }
                    }
                }
            }

            return AspectBitmap.FromImageAspect(builder.Build(), 4.0 / 3.0);
        }

        private class AppleScreenHiRes : AppleScreen
        {
            public AppleScreenHiRes()
            {
                Pixels = new byte[totalBytes];
            }

            public int Width => width;
            public int Height => height;
            public byte[] Pixels { get; }

            public int GetLineOffset(int lineIndex)
            {
                return Apple2Utils.GetHiResLineOffset(lineIndex);
            }
        }

        private class PixelPipe
        {
            private readonly Apple2TvSet _tv;
            private Apple2SimpleColor _prevPixel;
            private bool _setNext;

            public PixelPipe(Apple2TvSet tv)
            {
                _tv = tv;
            }

            public PixelPipe(PixelPipe o)
            {
                _tv = o._tv;
                Bits = o.Bits;
                _prevPixel = o._prevPixel;
                _setNext = o._setNext;
            }

            public int Bits { get; private set; }
            public double Err { get; private set; }

            public void PutPixel(Rgb c, bool shiftBit, bool isOdd)
            {
                Bits >>= 1;

                Apple2SimpleColor thisPixel = Apple2HiResUtils.GetPixelColor(true, shiftBit, isOdd);
                Rgb thisColor;

                if (_setNext)
                {
                    _setNext = false;
                    Bits |= 0x40;
                    thisColor = _tv.GetMiddleColor(_prevPixel, thisPixel, Apple2SimpleColor.Black);
                }
                else
                {
                    Apple2SimpleColor nextPixel = Apple2HiResUtils.GetPixelColor(true, shiftBit, !isOdd);

                    Rgb[] palette = new Rgb[4];
                    palette[0] = _tv.GetMiddleColor(_prevPixel, Apple2SimpleColor.Black, Apple2SimpleColor.Black);
                    palette[1] = _tv.GetMiddleColor(_prevPixel, thisPixel, Apple2SimpleColor.Black);
                    palette[2] = _tv.GetMiddleColor(Apple2SimpleColor.Black, nextPixel, Apple2SimpleColor.Black);
                    palette[3] = _tv.GetMiddleColor(_prevPixel, thisPixel, nextPixel);

                    int bestMatch = ColorUtils.BestMatch(c, palette);
                    thisColor = palette[bestMatch];

                    switch (bestMatch)
                    {
                        case 0:
                            thisPixel = Apple2SimpleColor.Black;
                            break;
                        case 1:
                            Bits |= 0x40;
                            break;
                        case 2:
                            thisColor = _tv.GetMiddleColor(_prevPixel, Apple2SimpleColor.Black, nextPixel);
                            goto case 0;
                        case 3:
                            _setNext = true;
                            goto case 1;
                    }
                }

                _prevPixel = thisPixel;

                Err += ColorUtils.GetDistanceSq(c, thisColor);
            }

            public void ResetError()
            {
                Err = 0;
            }
        }
    }
}
