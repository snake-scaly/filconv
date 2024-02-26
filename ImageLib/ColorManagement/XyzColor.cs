using ImageLib.Util;

namespace ImageLib.ColorManagement
{
    public struct XyzColor
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        
        public XyzColor Add(XyzColor b) => new XyzColor { X = X + b.X, Y = Y + b.Y, Z = Z + b.Z };
        public XyzColor Sub(XyzColor b) => new XyzColor { X = X - b.X, Y = Y - b.Y, Z = Z - b.Z };
        public XyzColor Mul(double s) => new XyzColor { X = X * s, Y = Y * s, Z = Z * s };
        public XyzColor Div(double s) => new XyzColor { X = X / s, Y = Y / s, Z = Z / s };

        public XyzColor Clamp()
        {
            return new XyzColor
            {
                X = ColorUtils.Clamp(X, 0, 1),
                Y = ColorUtils.Clamp(Y, 0, 1),
                Z = ColorUtils.Clamp(Z, 0, 1),
            };
        }
    }
}
