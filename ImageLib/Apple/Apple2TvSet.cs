using System.Windows.Media;

namespace ImageLib.Apple
{
    public interface Apple2TvSet
    {
        double Aspect { get; }
        Color[][] ProcessColors(Apple2SimpleColor[][] simpleColors);
        Color GetMiddleColor(Apple2SimpleColor left, Apple2SimpleColor middle, Apple2SimpleColor right);
        Apple2SimpleColor GetBestMatch(Color color, bool isOdd);
    }
}
