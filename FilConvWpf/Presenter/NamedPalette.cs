using ImageLib;

namespace FilConvWpf.Presenter
{
    internal class NamedPalette : NamedChoice
    {
        public readonly NativePalette Palette;

        public NamedPalette(string name, NativePalette palette)
            : base(name)
        {
            Palette = palette;
        }
    }
}
