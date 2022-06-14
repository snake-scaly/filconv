namespace ImageLib.Util
{
    public struct Rgb
    {
        public byte R { get; set; }

        public byte G { get; set; }

        public byte B { get; set; }

        public static Rgb FromRgb(byte r, byte g, byte b)
        {
            return new Rgb { R = r, G = g, B = b };
        }
    }
}
