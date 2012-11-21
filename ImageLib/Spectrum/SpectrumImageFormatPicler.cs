using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageLib.Spectrum
{
    public class SpectrumImageFormatPicler : SpectrumImageFormatAbstr
    {
        protected override int GetLineOffset(int y)
        {
            return y * _bytesPerLine;
        }
    }
}
