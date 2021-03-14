using ImageLib;
using ImageLib.Apple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;

namespace FilConvWpf.Native
{
    class AppleLoResDisplayMode : AppleDisplayModeBase
    {
        private readonly INativeImageFormat _pixelated;
        private readonly INativeImageFormat _perceptual;
        
        public AppleLoResDisplayMode(bool doubleResolution)
            : base(doubleResolution ? "FormatNameApple2DoubleLoRes" : "FormatNameApple2LoRes",
                   new Apple2LoResImageFormat(doubleResolution))
        {
            _pixelated = Format;
            _perceptual = new Apple2LoResNtscImageFormat(doubleResolution);
        }

        protected override void UpdateFormat()
        {
            Format = _ntsc ? _perceptual : _pixelated;
            OnFormatChanged();
        }
    }
}
