namespace ImageLib.ColorManagement
{
    public static class LabColorExtensions
    {
        public static XyzColor ToXyz(this LabColor a) => ColorSpace.Lab.ToXyz(a);

        public static Rgb ToSrgb(this LabColor a) => ColorSpace.Srgb.FromXyz(a.ToXyz());

        public static LabColor Add(this LabColor a, LabColor b) =>
            new LabColor { L = a.L + b.L, A = a.A + b.A, B = a.B + b.B };

        public static LabColor Sub(this LabColor a, LabColor b) =>
            new LabColor { L = a.L - b.L, A = a.A - b.A, B = a.B - b.B };

        public static LabColor Div(this LabColor a, double b) =>
            new LabColor { L = a.L / b, A = a.A / b, B = a.B / b };

        public static double LenSq(this LabColor a) => a.L * a.L + a.A * a.A + a.B * a.B;
    }
}
