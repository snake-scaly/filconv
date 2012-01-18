namespace ImageLib
{
    /// <summary>
    /// Conversion options for
    /// <see cref="NativeImageFormat.ToNative(BitmapSource, EncodingOptions)"/>.
    /// </summary>
    public struct EncodingOptions
    {
        public bool Dither { get; set; }
    }
}
