using ImageLib.Util;

namespace ImageLib.ColorManagement
{
    public static class XyzColorExtensions
    {
        public static LuvColor ToLuv(this XyzColor xyz) => ColorSpace.Luv.FromXyz(xyz);

        public static LabColor ToLab(this XyzColor xyz) => ColorSpace.Lab.FromXyz(xyz);

        public static XyzColor Add(this XyzColor a, XyzColor b)
        {
            return new XyzColor { X = a.X + b.X, Y = a.Y + b.Y, Z = a.Z + b.Z };
        }

        public static XyzColor Sub(this XyzColor a, XyzColor b)
        {
            return new XyzColor { X = a.X - b.X, Y = a.Y - b.Y, Z = a.Z - b.Z };
        }

        public static XyzColor Mul(this XyzColor a, double b)
        {
            return new XyzColor { X = a.X * b, Y = a.Y * b, Z = a.Z * b };
        }

        public static XyzColor Div(this XyzColor a, double b)
        {
            return new XyzColor { X = a.X / b, Y = a.Y / b, Z = a.Z / b };
        }

        public static XyzColor Clamp(this XyzColor xyz)
        {
            return new XyzColor
            {
                X = ColorUtils.Clamp(xyz.X, 0, 1),
                Y = ColorUtils.Clamp(xyz.Y, 0, 1),
                Z = ColorUtils.Clamp(xyz.Z, 0, 1),
            };
        }
    }
}
