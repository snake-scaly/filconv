using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilConvWpf
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

        public override bool Equals(object obj)
        {
            if (!(obj is PictureAspect))
            {
                return false;
            }
            PictureAspect pa = (PictureAspect)obj;
            return Aspect == pa.Aspect;
        }

        public override int GetHashCode()
        {
            return Aspect.GetHashCode();
        }

        public static bool operator ==(PictureAspect a, PictureAspect b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PictureAspect a, PictureAspect b)
        {
            return !a.Equals(b);
        }
    }
}
