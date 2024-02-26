namespace ImageLib.Apple.HiRes
{
    public class StripedHiResFillPolicy : IHiResFillPolicy
    {
        public HiResSimpleColor GetMiddleColor(HiResSimpleColor left, HiResSimpleColor middle, HiResSimpleColor right)
        {
            if (middle == HiResSimpleColor.Black)
                return HiResSimpleColor.Black;
            if (left != HiResSimpleColor.Black || right != HiResSimpleColor.Black)
                return HiResSimpleColor.White;
            return middle;
        }
    }
}