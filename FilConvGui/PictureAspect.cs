using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilConvGui
{
    struct PictureAspect
    {
        public static PictureAspect Original { get { return new PictureAspect(1); } }
        public static PictureAspect Television { get { return new PictureAspect(4.0 / 3.0); } }

        public double Aspect { get; set; }

        public PictureAspect(double aspect)
            : this()
        {
            Aspect = aspect;
        }

        public static bool operator ==(PictureAspect a, PictureAspect b)
        {
            return a.Aspect == b.Aspect;
        }

        public static bool operator !=(PictureAspect a, PictureAspect b)
        {
            return !(a == b);
        }
    }
}
