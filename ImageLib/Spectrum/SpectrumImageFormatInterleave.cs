using System;

namespace ImageLib.Spectrum
{
    public class SpectrumImageFormatInterleave : SpectrumImageFormatAbstr
    {
        protected override int GetLineOffset(int y)
        {
            return ((y & ~0x3F) << 5) | ((y & 0x07) << 8) | ((y & 0x38) << 2);
        }

        public override int ComputeMatchScore(NativeImage native)
        {
            return NativeImageFormatUtils.ComputeMatch(native, _totalBytes, ".scr");
        }

        /// <summary>
        /// Convert sequential lines to interleaved.
        /// </summary>
        /// <remarks>
        /// This method converts a Picler image with sequential pixel lines to native
        /// Spectrum interleaved line order.
        /// </remarks>
        /// <param name="sequential">Picler image to convert</param>
        /// <returns>Native Spectrum image.</returns>
        public NativeImage Interleave(NativeImage sequential)
        {
            var interleaved = new byte[_totalBytes];
            Array.Copy(sequential.Data, _paletteOffset, interleaved, _paletteOffset, _paletteSize);

            for (int y = 0; y < _height; ++y)
            {
                int srcOffset = y * _bytesPerLine;
                Array.Copy(sequential.Data, srcOffset, interleaved, GetLineOffset(y), _bytesPerLine);
            }

            return new NativeImage { Data = interleaved, FormatHint = new FormatHint(this) };
        }

        /// <summary>
        /// Convert interleaved lines to sequential.
        /// </summary>
        /// <remarks>
        /// This method converts a native Spectrum image to sequential pixel
        /// lines order, AKA Picler format.
        /// </remarks>
        /// <param name="interleaved">Spectrum image to convert</param>
        /// <returns>Picler sequential image.</returns>
        public NativeImage Deinterleave(NativeImage interleaved)
        {
            var sequential = new byte[_totalBytes];
            Array.Copy(interleaved.Data, _paletteOffset, sequential, _paletteOffset, _paletteSize);

            for (int y = 0; y < _height; ++y)
            {
                int dstOffset = y * _bytesPerLine;
                Array.Copy(interleaved.Data, GetLineOffset(y), sequential, dstOffset, _bytesPerLine);
            }

            return new NativeImage { Data = sequential, FormatHint = new FormatHint(new SpectrumImageFormatPicler()) };
        }
    }
}
