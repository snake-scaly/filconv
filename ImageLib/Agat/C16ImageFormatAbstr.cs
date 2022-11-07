using System.Collections.Generic;

namespace ImageLib.Agat
{
    public abstract class C16ImageFormatAbstr : AgatImageFormatAbstr
    {
        public override IEnumerable<NativePalette> SupportedPalettes => null;

        protected override int BitsPerPixel => 4;

        protected override int MapColorIndexNativeToStandard(int index, int palette)
        {
            return index;
        }
    }
}
