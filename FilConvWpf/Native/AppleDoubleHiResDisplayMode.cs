using ImageLib;
using ImageLib.Apple;

namespace FilConvWpf.Native
{
    class AppleDoubleHiResDisplayMode : AppleDisplayModeBase
    {
        private readonly INativeImageFormat _pixelated;
        private readonly INativeImageFormat _perceptual;

        public AppleDoubleHiResDisplayMode()
            : base("FormatNameApple2DoubleHiRes", new Apple2DoubleHiResImageFormat())
        {
            _pixelated = Format;
            _perceptual = new Apple2DoubleHiResNtscImageFormat();
        }

        protected override void UpdateFormat()
        {
            Format = _ntsc ? _perceptual : _pixelated;
            OnFormatChanged();
        }
    }
}
