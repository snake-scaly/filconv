namespace ImageLib.Spectrum
{
    public class SpectrumImageFormatPicler : SpectrumImageFormatAbstr
    {
        protected override int GetLineOffset(int y)
        {
            return y * _bytesPerLine;
        }

        public override int ComputeMatchScore(NativeImage native)
        {
            return NativeImageFormatUtils.ComputeMatch(native, _totalBytes, ".bol");
        }
    }
}
