using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace ImageLib.Apple
{
    public class Apple2LoResNtscImageFormat : NativeImageFormat
    {
        public double Aspect
        {
            get { return NtscPictureBuilder.PixelAspect; }
        }

        public BitmapSource FromNative(NativeImage native)
        {
            const int width = 40;
            const int height = 48;
            const int halfByteMask = 15;
            const int halfByteShift = 4;
            const int oddMask = 1;

            var builder = new NtscPictureBuilder(0);
            for (int y = 0; y < height; ++y)
            {
                int inLineOffset = Apple2Utils.GetLoResLineOffset(y);
                if (inLineOffset >= native.Data.Length)
                    continue;

                int shift = halfByteShift * (y & oddMask);

                using (NtscScanLine l1 = builder.GetScanLine(y * 4 + 0))
                using (NtscScanLine l2 = builder.GetScanLine(y * 4 + 1))
                using (NtscScanLine l3 = builder.GetScanLine(y * 4 + 2))
                using (NtscScanLine l4 = builder.GetScanLine(y * 4 + 3))
                {
                    int tick = 0;

                    for (int x = 0; x < width; ++x)
                    {
                        int inPixelOffset = inLineOffset + x;
                        if (inPixelOffset >= native.Data.Length)
                            break;

                        int pixelValue = (native.Data[inPixelOffset] >> shift) & halfByteMask;

                        for (int i = 0; i < 14; ++i)
                        {
                            int bit = pixelValue >> (tick & 3);
                            l1.Write(bit);
                            l2.Write(bit);
                            l3.Write(bit);
                            l4.Write(bit);
                            ++tick;
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
