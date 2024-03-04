using System;

namespace ImageLib.ColorManagement
{
    public class LuvColorSpace
    {
        private const double _linearThreshold = 0.008856451679; // (6/29)^3
        private const double _linearFactorFromXyz = 903.296296296296; // (29/3)^3
        private const double _linearFactorToXyz = 0.00110705646; // (3/29)^3

        private readonly double _whitePointY;
        private readonly Prime _whitePointPrime;

        public LuvColorSpace(XyzColor whitePoint)
        {
            _whitePointY = whitePoint.Y;
            _whitePointPrime = GetPrime(whitePoint);
        }

        public LuvColor FromXyz(XyzColor xyz)
        {
            // https://en.wikipedia.org/wiki/CIELUV#The_forward_transformation

            var yNorm = xyz.Y / _whitePointY;

            var l = yNorm <= _linearThreshold
                ? _linearFactorFromXyz * yNorm
                : 116 * Math.Pow(yNorm, 1/3.0) - 16;

            if (l < 1)
                return default;

            var prime = GetPrime(xyz);

            return new LuvColor
            {
                L = l,
                U = 13 * l * (prime.U - _whitePointPrime.U),
                V = 13 * l * (prime.V - _whitePointPrime.V),
            };
        }

        public XyzColor ToXyz(LuvColor luv)
        {
            if (luv.L < 1)
                return default;

            var uPrime = luv.U / 13 / luv.L + _whitePointPrime.U;
            var vPrime = luv.V / 13 / luv.L + _whitePointPrime.V;

            var y = luv.L <= 8
                ? _whitePointY * luv.L * _linearFactorToXyz
                : _whitePointY * Math.Pow((luv.L + 16) / 116, 3);

            return new XyzColor
            {
                Y = y,
                X = y * 9.0 * uPrime / 4.0 / vPrime,
                Z = y * (12 - 3 * uPrime - 20 * vPrime) / 4 / vPrime,
            };
        }

        private static Prime GetPrime(XyzColor xyz)
        {
            var primeDenomRec = 1 / (xyz.X + 15 * xyz.Y + 3 * xyz.Z);
            return new Prime { U = 4 * xyz.X * primeDenomRec, V = 9 * xyz.Y * primeDenomRec };
        }

        private struct Prime
        {
            public double U, V;
        }
    }
}
