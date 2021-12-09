namespace ImageLib
{
    /// <summary>
    /// Image in its native representation.
    /// </summary>
    /// <para>Data in <c>NativeImage</c> is typically a snapshot of
    /// some device's video memory.</para>
    public class NativeImage
    {
        /// <summary>
        /// Raw image data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Hint about the possible data format.
        /// </summary>
        public FormatHint FormatHint { get; set; }
    }
}
