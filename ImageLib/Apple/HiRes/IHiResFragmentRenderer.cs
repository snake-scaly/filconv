using System;

namespace ImageLib.Apple.HiRes
{
    public interface IHiResFragmentRenderer
    {
        /// <summary>
        /// Render a fragment of a line in Apple HiRes format.
        /// </summary>
        /// <param name="src">Source bytes of the fragment. Can be any length.
        /// E.g. for a normal HiRes picture it will be 40.</param>
        /// <param name="dst">Receiver for rendered BGR32 colors. Must be at least <c>src.Count * 7 * 4</c>.</param>
        /// <param name="startColumnIsOdd">Whether the first byte in <paramref name="src"/> is in an odd column.</param>
        void RenderLine(ArraySegment<byte> src, ArraySegment<byte> dst, bool startColumnIsOdd = false);
    }
}