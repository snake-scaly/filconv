using System;
using System.Collections.Generic;
using ImageLib.ColorManagement;

namespace ImageLib.Apple.HiRes
{
    public class HiResFragmentRenderer : IHiResFragmentRenderer
    {
        private readonly IList<Rgb> _palette;
        private readonly IHiResFillPolicy _fillPolicy;

        public HiResFragmentRenderer(IList<Rgb> palette, IHiResFillPolicy fillPolicy)
        {
            _palette = palette;
            _fillPolicy = fillPolicy;
        }

        public void RenderLine(ArraySegment<byte> src, ArraySegment<byte> dst, bool startColumnIsOdd = false)
        {
            foreach (var triplet in Triplets(SimpleColors(src, startColumnIsOdd)))
            {
                var sc = _fillPolicy.GetMiddleColor(triplet[0], triplet[1], triplet[2]);
                var color = _palette[(int)sc];
                ((IList<byte>)dst)[0] = color.B;
                ((IList<byte>)dst)[1] = color.G;
                ((IList<byte>)dst)[2] = color.R;
                dst = new ArraySegment<byte>(dst.Array, dst.Offset + 4, dst.Count - 4);
            }
        }

        private static IEnumerable<HiResSimpleColor> SimpleColors(ArraySegment<byte> src, bool startColumnIsOdd)
        {
            bool oddity = startColumnIsOdd;

            foreach (var pixel in src)
            {
                bool shiftBit = (pixel & 0x80) != 0;
                for (int j = 0; j < 7; ++j)
                {
                    bool pixelBit = (pixel & (1 << j)) != 0;
                    yield return GetPixelColor(pixelBit, shiftBit, oddity);
                    oddity = !oddity;
                }
            }
        }

        /// <summary>
        /// Calculate direct pixel color based on its characteristics.
        /// </summary>
        /// <param name="pixelBit">state of the pixel bit itself</param>
        /// <param name="shiftBit">state of the most significant bit in the pixel's byte</param>
        /// <param name="isOdd">whether the horizontal screen position of the pixel is odd</param>
        /// <returns>One of the <see cref="HiResSimpleColor"/> values.</returns>
        public static HiResSimpleColor GetPixelColor(bool pixelBit, bool shiftBit, bool isOdd)
        {
            if (!pixelBit)
            {
                return HiResSimpleColor.Black;
            }
            if (isOdd)
            {
                return shiftBit ? HiResSimpleColor.Orange : HiResSimpleColor.Violet;
            }
            return shiftBit ? HiResSimpleColor.Blue : HiResSimpleColor.Green;
        }

        private static IEnumerable<T[]> Triplets<T>(IEnumerable<T> src)
        {
            // src is extended with an additional zero element at the start and at the end.

            T x0 = default, x1 = default;
            var skip = 1;

            foreach (var x2 in src)
            {
                if (skip == 0)
                    yield return new[] { x0, x1, x2 };
                else
                    skip--;
                x0 = x1;
                x1 = x2;
            }

            yield return new[] { x0, x1, default };
        }
    }
}
