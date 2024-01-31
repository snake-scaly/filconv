namespace ImageLib.Apple.BitStream
{
    internal interface IBitStreamPictureBuilder
    {
        /// Get a scanline builder.
        /// <remarks>
        /// Close or dispose of the returned object when done. The scanline won't render
        /// if the scanline builder is not properly closed.
        /// </remarks>
        IScanlineWriter GetScanlineWriter(int index);

        /// Get the rendered bitmap.
        Bgr32BitmapData Build();
    }
}
