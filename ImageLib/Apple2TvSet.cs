using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ImageLib
{
    public interface Apple2TvSet
    {
        double Aspect { get; }
        Color[][] ProcessColors(Apple2SimpleColor[][] simpleColors);
    }
}
