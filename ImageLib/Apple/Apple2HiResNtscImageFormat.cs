using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace ImageLib.Apple
{
    public class Apple2HiResNtscImageFormat : NativeImageFormat
    {
        public double Aspect
        {
            get { return NtscPictureBuilder.PixelAspect; }
        }

        public BitmapSource FromNative(NativeImage native)
        {
            const int height = 192;
            const int firstBitPhase = 2;
            const int bytesPerLine = 40;
            const int pixelBitsCount = 7;
            const int pixelBitsMask = (1 << pixelBitsCount) - 1;

            var builder = new NtscPictureBuilder(firstBitPhase);

            for (int y = 0; y < height; ++y)
            {
                int lineOffset = Apple2Utils.GetHiResLineOffset(y);
                if (lineOffset >= native.Data.Length)
                    continue;

                using (NtscScanLine scanline = builder.GetScanLine(y))
                {
                    for (int i = 0; i < bytesPerLine; ++i)
                    {
                        int bitsOffset = lineOffset + i;
                        if (bitsOffset > native.Data.Length)
                            break;

                        int palette = native.Data[bitsOffset] >> pixelBitsCount;
                        int bits = native.Data[bitsOffset] & pixelBitsMask;
                        if (i + 1 < bytesPerLine)
                        {
                            bits |= (native.Data[bitsOffset + 1] & pixelBitsMask) << pixelBitsCount;
                        }

                        for (int tick = 0; tick < 14; ++tick)
                        {
                            int shift = (tick + palette) >> 1;
                            scanline.Write(bits >> shift);
                        }
                    }
                }
            }

            return builder.GetBitmap();
        }

        public NativeImage ToNative(BitmapSource bitmap, EncodingOptions options)
        {
            throw new NotImplementedException();
        }


        public int ComputeMatchScore(NativeImage native)
        {
            throw new NotImplementedException();
        }
    }
}
