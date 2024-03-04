using System;

namespace ImageLib.ColorManagement
{
    public class LabColorSpace
    {
        private const double _sigma = 6.0 / 29;
        private const double _sigmaSquare = _sigma * _sigma;
        private const double _sigmaCube = _sigmaSquare * _sigma;
        private static readonly double _sigmaSquareRoot = Math.Sqrt(_sigma);

        // Modification to increase chroma contribution to distance.
        private const double _chromaScale = 0.7;

        private readonly XyzColor _whitePoint;

        public LabColorSpace(XyzColor whitePoint)
        {
            _whitePoint = whitePoint;
        }

        public LabColor FromXyz(XyzColor xyz)
        {
            // https://en.wikipedia.org/wiki/CIELAB_color_space#From_CIEXYZ_to_CIELAB

            var fX = F(xyz.X / _whitePoint.X);
            var fY = F(xyz.Y / _whitePoint.Y);
            var fZ = F(xyz.Z / _whitePoint.Z);
            return new LabColor { L = 116 * fY - 16, A = 500 * _chromaScale * (fX - fY), B = 200 * _chromaScale * (fY - fZ) };

            double F(double t) => t > _sigmaCube ? Math.Pow(t, 1.0 / 3) : t * _sigmaSquareRoot / 3 + 4.0 / 29;
        }

        public XyzColor ToXyz(LabColor lab)
        {
            // https://en.wikipedia.org/wiki/CIELAB_color_space#From_CIELAB_to_CIEXYZ

            var l = (lab.L + 16) / 116;

            return new XyzColor
            {
                X = _whitePoint.X * F(l + lab.A / 500 / _chromaScale),
                Y = _whitePoint.Y * F(l),
                Z = _whitePoint.Z * F(l - lab.B / 200 / _chromaScale),
            };
            
            double F(double t) => t > _sigma ? t * t * t : 3 * _sigmaSquare * (t - 4.0 / 29);
        }
    }
}
