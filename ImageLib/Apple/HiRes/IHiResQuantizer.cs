namespace ImageLib.Apple.HiRes
{
    public interface IHiResQuantizer
    {
        byte[] Quantize(IReadOnlyPixels src);
    }
}