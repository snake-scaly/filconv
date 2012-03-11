using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageLib.Spectrum
{
    public class SpectrumImageFormatInterleave : SpectrumImageFormatAbstr
    {
        protected override int GetLineOffset(int y)
        {
            return ((y & ~0x3F) << 5) | ((y & 0x07) << 8) | ((y & 0x38) << 2);
        }
    }
}
