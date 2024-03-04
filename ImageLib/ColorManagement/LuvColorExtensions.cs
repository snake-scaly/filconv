namespace ImageLib.ColorManagement
{
    public static class LuvColorExtensions
    {
        public static XyzColor ToXyz(this LuvColor a) => ColorSpace.Luv.ToXyz(a);

        public static Rgb ToSrgb(this LuvColor a) => ColorSpace.Srgb.FromXyz(a.ToXyz());

        public static LuvColor Add(this LuvColor a, LuvColor b) =>
            new LuvColor { L = a.L + b.L, U = a.U + b.U, V = a.V + b.V };

        public static LuvColor Sub(this LuvColor a, LuvColor b) =>
            new LuvColor { L = a.L - b.L, U = a.U - b.U, V = a.V - b.V };

        public static LuvColor Div(this LuvColor a, double b) =>
            new LuvColor { L = a.L / b, U = a.U / b, V = a.V / b };

        public static double LenSq(this LuvColor a) => a.L * a.L + a.U * a.U + a.V * a.V;
    }
}
