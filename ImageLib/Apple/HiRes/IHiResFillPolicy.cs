namespace ImageLib.Apple.HiRes
{
    public interface IHiResFillPolicy
    {
        HiResSimpleColor GetMiddleColor(HiResSimpleColor left, HiResSimpleColor middle, HiResSimpleColor right);
    }
}