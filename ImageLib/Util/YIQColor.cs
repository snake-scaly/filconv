using System;
using ImageLib.ColorManagement;

namespace ImageLib.Util
{
    /// <summary>
    /// Performs conversion from Apple hardware 4-bit color to an RGB color.
    /// </summary>
    /// <remarks>
    /// <para>Each bit is assigned a quasi-phase of 0, 1, 2, or 3. The phase of the least significnat
    /// bit can be specified in the <code>phase</code> parameter so that the color
    /// conversion can start from any position in the bit stream.</para>
    /// </remarks>
    public class YIQColor
    {
        /// <summary>
        /// Create a YIQ color using a predictable equal-lightness algorithm.
        /// </summary>
        /// <remarks>
        /// <para>This method should be used to generate palettes for two-way conversion.</para>
        /// <para>Only the 4 least significant bits are used.</para>
        /// </remarks>
        /// <param name="bits">bits to convert</param>
        /// <param name="phase">phase of the least significant bit, see class remarks</param>
        public static YIQColor From4BitsStrict(int bits, int phase)
        {
            var c = new YIQColor();
            c.SetYFrom4BitsStrict(bits);
            c.SetIQFromBits(ApplyPhase(bits, phase));
            return c;
        }

        /// <summary>
        /// Create a YIQ color using TV-like smoothing of the luma values.
        /// </summary>
        /// <remarks>
        /// <para>This method can be used to produce picture similar to that shown by an NTSC TV.</para>
        /// <para>Only the 6 least significant bits are used. The order of the bits must match the image order.</para>
        /// </remarks>
        /// <param name="bits">bits to convert</param>
        /// <param name="phase">phase of the least significant bit, see class remarks</param>
        public static YIQColor From6BitsPerceptual(int bits, int phase)
        {
            var c = new YIQColor();
            c.SetYFrom6BitsPerceptual(bits);
            c.SetIQFromBits(ApplyPhase(bits >> 1, phase));
            return c;
        }

        private static int ApplyPhase(int bits, int phase)
        {
            phase &= _twoBitMask;
            if (phase != 0)
            {
                bits = ((bits << phase) | ((bits & _fourBitMask) >> (4 - phase))) & _fourBitMask;
            }
            return bits;
        }

        public double Y { get; private set; }
        public double I { get; private set; }
        public double Q { get; private set; }

        public Rgb ToColor()
        {
            return ColorUtils.ColorFromYiq(Y, I, Q);
        }

        /// <summary>
        /// Set I and Q values based on the given Apple hardware bits.
        /// </summary>
        /// <remarks>
        /// Only the 4 least significant bits are used. The least significant bit must be at phase 0.
        /// </remarks>
        /// <param name="bits">bits to convert</param>
        private void SetIQFromBits(int bits)
        {
            const double iAmplitude = 0.5957;
            const double qAmplitude = 0.5226;
            const double preventOverSaturationFactor = 0.55;

            I = (((bits >> 0) & 1) - ((bits >> 1) & 1) - ((bits >> 2) & 1) + ((bits >> 3) & 1)) * 0.5 * iAmplitude * preventOverSaturationFactor;
            Q = (((bits >> 0) & 1) + ((bits >> 1) & 1) - ((bits >> 2) & 1) - ((bits >> 3) & 1)) * 0.5 * qAmplitude * preventOverSaturationFactor;
        }

        /// <summary>
        /// Set Y value using a predictable equal-lightness algorithm.
        /// </summary>
        /// <remarks>
        /// <para>This method should be used to generate palettes for two-way conversion.</para>
        /// <para>Only the 4 least significant bits are used. The order of the bits does not matter.</para>
        /// </remarks>
        /// <param name="bits">bits to convert</param>
        private void SetYFrom4BitsStrict(int bits)
        {
            Y = Math.Pow(_bitCount[bits & _fourBitMask] * 0.25, _yGammaCorrection);
        }

        /// <summary>
        /// Set Y value using TV-like smoothing of the luma values.
        /// </summary>
        /// <remarks>
        /// <para>This method can be used to produce picture similar to that shown by an NTSC TV.</para>
        /// <para>Only the 6 least significant bits are used. The order of the bits must match the image order.</para>
        /// </remarks>
        /// <param name="bits">bits to convert</param>
        private void SetYFrom6BitsPerceptual(int bits)
        {
            const double smoothFactor = 0.2;
            const double smoothFactor2 = 0.6;
            const double Y0 = smoothFactor;
            const double Y1 = smoothFactor2;
            const double Y2 = 1;
            const double Y3 = 1;
            const double Y4 = smoothFactor2;
            const double Y5 = smoothFactor;

            Y = (((bits >> 0) & 1) * Y0 + ((bits >> 1) & 1) * Y1 + ((bits >> 2) & 1) * Y2 + ((bits >> 3) & 1) * Y3 + ((bits >> 4) & 1) * Y4 + ((bits >> 5) & 1) * Y5) / (Y0 + Y1 + Y2 + Y3 + Y4 + Y5);
            Y = Math.Pow(Y, _yGammaCorrection);
        }

        private const int _twoBitMask = 3;
        private const int _fourBitMask = 15;
        private const double _yGammaCorrection = 0.8;
        private static readonly int[] _bitCount = new int[16] { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4 };
    }
}
