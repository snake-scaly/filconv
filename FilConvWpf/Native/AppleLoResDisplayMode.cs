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
        private readonly NativeImageFormat _pixelated;
        private readonly NativeImageFormat _perceptual;
        
        public AppleLoResDisplayMode()
            : base("FormatNameApple2LoRes", new Apple2LoResImageFormat())
        {
            _pixelated = Format;
            _perceptual = new Apple2LoResNtscImageFormat();
        }

        protected override void UpdateFormat()
        {
            Format = _ntsc ? _perceptual : _pixelated;
            OnFormatChanged();
        }
    }
}
