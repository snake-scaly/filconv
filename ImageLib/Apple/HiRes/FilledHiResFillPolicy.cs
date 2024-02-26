namespace ImageLib.Apple.HiRes
{
    public class FilledHiResFillPolicy : IHiResFillPolicy
    {
        public HiResSimpleColor GetMiddleColor(HiResSimpleColor left, HiResSimpleColor middle, HiResSimpleColor right)
        {
            if (middle == HiResSimpleColor.Black)
                return left == right ? left : HiResSimpleColor.Black;
            if (left != HiResSimpleColor.Black || right != HiResSimpleColor.Black)
                return HiResSimpleColor.White;
            return middle;
        }
    }
}