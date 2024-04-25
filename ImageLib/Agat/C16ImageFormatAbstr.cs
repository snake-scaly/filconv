using System.Collections.Generic;

namespace ImageLib.Agat
{
    public abstract class C16ImageFormatAbstr : AgatImageFormatAbstr
    {
        protected override int BitsPerPixel => 4;
        protected override int MapColorIndexNativeToStandard(int index, int palette) => index;
        public override IEnumerable<NativePalette>? GetSupportedPalettes(NativeDisplay display) => null;
    }
}
