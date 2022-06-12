namespace ImageLib
{
    /// <summary>
    /// Conversion options for
    /// <see cref="NativeImageFormat.ToNative(BitmapSource, EncodingOptions)"/>.
    /// </summary>
    public struct EncodingOptions
    {
        public NativeDisplay Display { get; set; }
        public NativePalette Palette { get; set; }
        public bool Dither { get; set; }
    }
}
