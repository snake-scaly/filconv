﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageLib.Apple
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
    /// <item>Use the <see cref="GetScanLine"/> method to get the scanline renderer.</item>
    /// <item>Feed all the bits of the line into the renderer.</item>
    /// <item>Close, or dispose of, the renderer. It's a good idea to wrap it in a <code>using</code> block.</item>
    /// </list>
    /// <item>Use the <see cref="GetBitmap"/> method to retrieve the resulting bitmap.</item>
    /// <item>Use the <see cref="GetPixelAspect"/> method to retrieve the bitmap's pixel aspect ratio.</item>
    /// </list>
    /// </remarks>
    public class NtscPictureBuilder
    {
        private const int _width = 560;
        private const int _height = 192;
        private const int _linesPerScanline = 3;
        private const int _scanlineBufferSize = 6;

        private int _phase;
        private WriteableBitmap _bitmap;

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

            const double dpi = 96;

            int widthWithOverdraw = _width + _scanlineBufferSize;
            _bitmap = new WriteableBitmap(widthWithOverdraw, _height * _linesPerScanline, dpi / PixelAspect, dpi, PixelFormats.Bgr32, null);
        }

        /// <summary>
        /// Get aspect ratio of the generated bitmap pixels.
        /// </summary>
        public static double PixelAspect
        {
            get
            {
                const double bitsPerNtscLine = 753.2;
                const double ntscScanLinesPerField = 262.5;
                const double screenRelativeWidth = 4;
                const double screenRelativeHeight = 3;
                return (ntscScanLinesPerField * _linesPerScanline / bitsPerNtscLine) * (screenRelativeWidth / screenRelativeHeight);
            }
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
        public NtscScanLine GetScanLine(int index)
        {
            if (index < 0 || index >= _height)
                throw new ArgumentOutOfRangeException("index", index, "Must be within [0, " + _height + ")");

            var writer = new BitmapDoubleColorWriter(_bitmap, index * _linesPerScanline);
            try
            {
                return new NtscScanLine(writer, _phase);
            }
            catch
            {
                writer.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Get the rendered bitmap.
        /// </summary>
        /// <returns>A bitmap object with scanlines rendered so far.</returns>
        public BitmapSource GetBitmap()
        {
            return _bitmap;
        }
    }
}
