using ImageLib.ColorManagement;

namespace ImageLib.Util
{
    /// Represents a color in a <see cref="Palette"/>.
    /// <remarks>
    /// Default value represents an unimportant color.
    /// </remarks>
    public struct PaletteEntry
    {
        public Rgb Value { get; }
        public LabColor Perceptual { get; }
        public bool Important { get; }

        /// Create an important color with the given value.
        public PaletteEntry(Rgb rgb, bool important = true)
        {
            Value = rgb;
            Perceptual = rgb.ToLab();
            Important = important;
        }
    }
}
