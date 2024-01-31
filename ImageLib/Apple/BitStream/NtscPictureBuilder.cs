using System;
using ImageLib.Util;

namespace ImageLib.Apple.BitStream
{
    /// <summary>
    /// Manages conversion of raw Apple graphics data into an image
    /// similar to that which would be displayed by an actual NTSC
    /// TV-set.
    /// </summary>
    /// <remarks>
    /// <para>Use this class as follows:</para>
    /// <list type="number">
    /// <item>Create an instance of the class.</item>
    /// <item>For each of the image lines:</item>
    /// <list type="number">
    /// <item>Use the <see cref="GetScanlineWriter"/> method to get the scanline renderer.</item>
    /// <item>Feed all the bits of the line into the renderer.</item>
    /// <item>Close, or dispose of, the renderer. It's a good idea to wrap it in a <code>using</code> block.</item>
    /// </list>
    /// <item>Use the <see cref="Build"/> method to retrieve the resulting bitmap.</item>
    /// </list>
    /// </remarks>
    internal class NtscPictureBuilder : IBitStreamPictureBuilder
    {
        private const int _width = 560;
        private const int _height = 192;
        private const int _linesPerScanline = 3;
        private const int _scanlinePadding = 3;

        private readonly int _phase;
        private readonly Bgr32BitmapData _bitmap;

        /// <summary>
        /// Create a builder instance.
        /// </summary>
        /// <remarks>
        /// The NTSC picture is always 560x192 bits in size.
        /// </remarks>
        /// <param name="phase">phase of the first bit in each scanline. See <see cref="YIQColor"/> for more detail</param>
        public NtscPictureBuilder(int phase)
        {
            _phase = phase;
            var pixels = new byte[_width * _height * _linesPerScanline * 4];
            _bitmap = new Bgr32BitmapData(pixels, _width, _height * _linesPerScanline);
        }

        /// <summary>
        /// Get a scanline renderer for a scanline.
        /// </summary>
        /// <remarks>
        /// Close or dispose of the returned object when done. The scanline won't render
        /// if the renderer is not properly closed.
        /// </remarks>
        /// <param name="index">scanline to get a renderer for</param>
        /// <returns>New scanline renderer object.</returns>
        public IScanlineWriter GetScanlineWriter(int index)
        {
            if (index < 0 || index >= _height)
                throw new ArgumentOutOfRangeException(nameof(index), index, "Must be within [0, " + _height + ")");

            var doubleColorWriter = new BitmapDoubleColorWriter(_bitmap, index * _linesPerScanline);
            var partialLineColorWriter = new PartialLineColorWriter(doubleColorWriter, _scanlinePadding, _width);
            return new NtscScanlineWriter(partialLineColorWriter, _phase);
        }

        /// <summary>
        /// Get the rendered bitmap.
        /// </summary>
        /// <returns>A bitmap object with scanlines rendered so far.</returns>
        public Bgr32BitmapData Build()
        {
            return _bitmap;
        }
    }
}
