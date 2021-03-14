using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageLib.Util;

namespace ImageLib.Apple
{
    public class Apple2HiResImageFormat : INativeImageFormat
    {
        private const int width = 280;
        private const int height = 192;
        private const double dpi = 72;
        private const int pixelsPerByte = 7;
        private const int interleave = 1;
        private const int totalBytes = 0x2000;

        private Apple2TvSet tvSet;

        public double Aspect
        {
            get { return tvSet.Aspect; }
        }

        public Apple2HiResImageFormat(Apple2TvSet tvSet)
        {
            this.tvSet = tvSet;
        }

        public BitmapSource FromNative(NativeImage native)
        {
            Apple2SimpleColor[][] simple = ToSimpleColor(native);
            Color[][] colors = tvSet.ProcessColors(simple);

            int height = colors.Length;
            int width = colors[0].Length;

            int stride = 4 * width;
            int size = stride * height;
            byte[] pixels = new byte[size];
            int pixelOffset = 0;

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Color c = colors[y][x];
                    pixels[pixelOffset++] = c.B;
                    pixels[pixelOffset++] = c.G;
                    pixels[pixelOffset++] = c.R;
                    ++pixelOffset;
                }
            }

            WriteableBitmap result = new WriteableBitmap(width, height, dpi / Aspect, dpi, PixelFormats.Bgr32, null);
            result.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return result;
        }

        /// <summary>
        /// Convert Apple image snapshot into an intermediate color matrix.
        /// </summary>
        /// <remarks>
        /// This method converts a native memory representation of an
        /// Apple image into a rectangular matrix of simple Apple colors
        /// with sequential lines which can be further processed to
        /// produce actual display colors.
        /// </remarks>
        /// <param name="native">native image to decode</param>
        /// <returns>
        /// An array of <see cref="height"/> lines, <see cref="width"/>
        /// colors each.
        /// </returns>
        private static Apple2SimpleColor[][] ToSimpleColor(NativeImage native)
        {
            Apple2SimpleColor[][] result = new Apple2SimpleColor[height][];
            int nativeStride = width / pixelsPerByte;

            for (int y = 0; y < height; ++y)
            {
                result[y] = new Apple2SimpleColor[width];
                int lineOffset = ((y & 0x07) << 10) + ((y & 0x38) << 4) + (y >> 6) * 40;
                int lineEnd = Math.Min(lineOffset + nativeStride, native.Data.Length);
                int x = 0;
                for (int byteOffset = lineOffset; byteOffset < lineEnd - 1; byteOffset += 2)
                {
                    foreach (Apple2SimpleColor c in DecodeWord(native.Data, byteOffset))
                    {
                        result[y][x++] = c;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Decode a 16-bit word of the image.
        /// </summary>
        /// <param name="pixels">bytes with the image pixels</param>
        /// <param name="offset">offset of the first byte of the word to decode</param>
        /// <returns>An array of 14 decoded colors.</returns>
        private static IList<Apple2SimpleColor> DecodeWord(byte[] pixels, int offset)
        {
            List<Apple2SimpleColor> result = new List<Apple2SimpleColor>(16);
            bool oddity = false;

            for (int i = 0; i < 2; ++i)
            {
                byte pixel = pixels[offset++];
                bool shiftBit = (pixel & 0x80) != 0;
                for (int j = 0; j < 7; ++j)
                {
                    bool pixelBit = (pixel & (1 << j)) != 0;
                    result.Add(GetPixelColor(pixelBit, shiftBit, oddity));
                    oddity = !oddity;
                }
            }

            return result;
        }

        /// <summary>
        /// Calculate direct pixel color based on its characteristics.
        /// </summary>
        /// <param name="pixelBit">state of the pixel bit itself</param>
        /// <param name="shiftBit">state of the most significant bit in the pixel's byte</param>
        /// <param name="isOdd">whether the horizontal screen position of the pixel is odd</param>
        /// <returns>One of the <see cref="Apple2SimpleColor"/> values.</returns>
        private static Apple2SimpleColor GetPixelColor(bool pixelBit, bool shiftBit, bool isOdd)
        {
            if (!pixelBit)
            {
                return Apple2SimpleColor.Black;
            }
            if (isOdd)
            {
                return shiftBit ? Apple2SimpleColor.Orange : Apple2SimpleColor.Violet;
            }
            return shiftBit ? Apple2SimpleColor.Blue : Apple2SimpleColor.Green;
        }

        public NativeImage ToNative(BitmapSource bitmap, EncodingOptions options)
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
                        Color c = x + i < src.Width ? src.GetPixel(x + i, y) : Colors.Black;
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

            return new NativeImage(dst.Pixels, new FormatHint(this));
        }

        public int ComputeMatchScore(NativeImage native)
        {
            return NativeImageFormatUtils.ComputeMatch(native, totalBytes);
        }

        private class AppleScreenHiRes : AppleScreen
        {
            private byte[] _pixels;

            public AppleScreenHiRes()
            {
                _pixels = new byte[totalBytes];
            }

            public AppleScreenHiRes(AppleScreenHiRes o)
            {
                _pixels = new byte[totalBytes];
                o.Pixels.CopyTo(Pixels, 0);
            }

            public int Width
            {
                get { return width; }
            }

            public int Height
            {
                get { return height; }
            }

            public int ByteWidth
            {
                get { return width / pixelsPerByte; }
            }

            public byte[] Pixels
            {
                get { return _pixels; }
            }

            public int GetLineOffset(int lineIndex)
            {
                return Apple2Utils.GetHiResLineOffset(lineIndex);
            }
        }

        private class PixelPipe
        {
            private Apple2TvSet _tv;
            private int _bits;
            private Apple2SimpleColor _prevPixel;
            private bool _setNext;
            private double _err;

            public PixelPipe(Apple2TvSet tv)
            {
                _tv = tv;
            }

            public PixelPipe(PixelPipe o)
            {
                _tv = o._tv;
                _bits = o._bits;
                _prevPixel = o._prevPixel;
                _setNext = o._setNext;
            }

            public int Bits
            {
                get { return _bits; }
            }

            public double Err
            {
                get { return _err; }
            }

            public void PutPixel(Color c, bool shiftBit, bool isOdd)
            {
                _bits >>= 1;

                Apple2SimpleColor thisPixel = GetPixelColor(true, shiftBit, isOdd);
                Color thisColor;

                if (_setNext)
                {
                    _setNext = false;
                    _bits |= 0x40;
                    thisColor = _tv.GetMiddleColor(_prevPixel, thisPixel, Apple2SimpleColor.Black);
                }
                else
                {
                    Apple2SimpleColor nextPixel = GetPixelColor(true, shiftBit, !isOdd);

                    Color[] palette = new Color[4];
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
                            _bits |= 0x40;
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

                _err += ColorUtils.GetDistanceSq(c, thisColor);
            }

            public void ResetError()
            {
                _err = 0;
            }
        }
    }
}
