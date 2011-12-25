using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilConvGui
{
    struct PictureScale
    {
        public static PictureScale Single { get { return new PictureScale(1, false); } }
        public static PictureScale Double { get { return new PictureScale(2, false); } }
        public static PictureScale Triple { get { return new PictureScale(3, false); } }
        public static PictureScale Free { get { return new PictureScale(1, true); } }

        public double Scale { get; set; }
        public bool ResizeToFit { get; set; }

        public PictureScale(double scale, bool resizeToFit)
            : this()
        {
            Scale = scale;
            ResizeToFit = resizeToFit;
        }

        public static bool operator ==(PictureScale a, PictureScale b)
        {
            return a.Scale == b.Scale && a.ResizeToFit == b.ResizeToFit;
        }

        public static bool operator !=(PictureScale a, PictureScale b)
        {
            return !(a == b);
        }
    }
}
