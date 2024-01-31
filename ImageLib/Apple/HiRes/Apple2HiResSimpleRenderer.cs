using System;
using System.Collections.Generic;
using ImageLib.Util;

namespace ImageLib.Apple.HiRes
{
    internal static class Apple2HiResSimpleRenderer
    {
        public static AspectBitmap Render(NativeImage native, Apple2TvSet tvSet)
        {
            Apple2SimpleColor[][] simple = ToSimpleColor(native);
            Rgb[][] colors = tvSet.ProcessColors(simple);

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
                    Rgb c = colors[y][x];
                    pixels[pixelOffset++] = c.B;
                    pixels[pixelOffset++] = c.G;
                    pixels[pixelOffset++] = c.R;
                    ++pixelOffset;
                }
            }

            return AspectBitmap.FromImageAspect(new Bgr32BitmapData(pixels, width, height), 4.0 / 3.0);
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
        /// An array of 192 lines, 280 colors each.
        /// </returns>
        private static Apple2SimpleColor[][] ToSimpleColor(NativeImage native)
        {
            const int width = 280;
            const int height = 192;
            const int pixelsPerByte = 7;

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
                    result.Add(Apple2HiResUtils.GetPixelColor(pixelBit, shiftBit, oddity));
                    oddity = !oddity;
                }
            }

            return result;
        }
    }
}
