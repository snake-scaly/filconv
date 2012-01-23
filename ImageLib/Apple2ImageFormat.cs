using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace ImageLib
{
    public class Apple2ImageFormat : NativeImageFormat
    {
        private const int width = 280;
        private const int height = 192;
        private const double dpi = 72;
        private const int pixelsPerByte = 7;
        private const int interleave = 1;

        private Apple2TvSet tvSet;

        public double Aspect
        {
            get { return tvSet.Aspect; }
        }

        public Apple2ImageFormat(Apple2TvSet tvSet)
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

            WriteableBitmap result = new WriteableBitmap(width, height, dpi, dpi, PixelFormats.Bgr32, null);
            result.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
            return result;
        }

        public NativeImage ToNative(BitmapSource bitmap, EncodingOptions options)
        {
            // Encoding is not supported.
            int nativeStride = width / pixelsPerByte;
            byte[] pixels = new byte[nativeStride * height];
            return new NativeImage(pixels, new FormatHint(this));
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
    }
}
