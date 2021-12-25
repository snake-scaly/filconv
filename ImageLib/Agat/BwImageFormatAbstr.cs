using ImageLib.Gamut;

namespace ImageLib.Agat
{
    public abstract class BwImageFormatAbstr : AgatImageFormatAbstr
    {
        protected override int BitsPerPixel => 1;

        protected override IGamut Gamut { get; } = new BlackAndWhiteGamut();

        protected override int MapColorIndexNativeToStandard(int index, int palette)
        {
            return _nativeToStandardColorMap[palette][index];
        }

        private static readonly int[][] _nativeToStandardColorMap =
        {
            new[] { 0, 15 },
            new[] { 15, 0 },
            new[] { 0, 2 },
            new[] { 2, 0 },
        };
    }
}
