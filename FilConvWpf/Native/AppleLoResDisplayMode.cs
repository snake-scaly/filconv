using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageLib.Apple;

namespace FilConvWpf.Native
{
    class AppleLoResDisplayMode : NativeDisplayMode
    {
        public AppleLoResDisplayMode() :
            base("FormatNameApple2LoRes", new Apple2LoResImageFormat())
        {

        }
    }
}
