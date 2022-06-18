using System;

namespace ImageLib.Util
{
    public struct Rgb : IEquatable<Rgb>
    {
        public byte R { get; set; }

        public byte G { get; set; }

        public byte B { get; set; }

        public static Rgb FromRgb(byte r, byte g, byte b)
        {
            return new Rgb { R = r, G = g, B = b };
        }

        public bool Equals(Rgb other)
        {
            return R == other.R && G == other.G && B == other.B;
        }

        public override bool Equals(object obj)
        {
            return obj is Rgb other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + R.GetHashCode();
                hash = hash * 31 + G.GetHashCode();
                hash = hash * 31 + B.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"#{R:X2}{G:X2}{B:X2}";
        }
    }
}
