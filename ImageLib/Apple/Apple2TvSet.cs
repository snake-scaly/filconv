using ImageLib.Util;

namespace ImageLib.Apple
{
    public interface Apple2TvSet
    {
        double Aspect { get; }
        Rgb[][] ProcessColors(Apple2SimpleColor[][] simpleColors);
        Rgb GetMiddleColor(Apple2SimpleColor left, Apple2SimpleColor middle, Apple2SimpleColor right);
        Apple2SimpleColor GetBestMatch(Rgb color, bool isOdd);
    }
}
