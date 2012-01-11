using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilConvWpf
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

        public override bool Equals(object obj)
        {
            if (!(obj is PictureScale))
            {
                return false;
            }
            PictureScale ps = (PictureScale)obj;
            return Scale == ps.Scale && ResizeToFit == ps.ResizeToFit;
        }

        public override int GetHashCode()
        {
            return Scale.GetHashCode() ^ (ResizeToFit ? ~0 : 0);
        }

        public static bool operator ==(PictureScale a, PictureScale b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PictureScale a, PictureScale b)
        {
            return !a.Equals(b);
        }
    }
}
