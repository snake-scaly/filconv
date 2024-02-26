using ImageLib.Util;

namespace ImageLib.Quantization
{
    public interface IQuantizer
    {
        void Quantize(IReadOnlyPixels src, IWriteablePixels<int> dst, Palette palette);
    }
}
