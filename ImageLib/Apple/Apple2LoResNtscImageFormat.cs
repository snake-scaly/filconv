using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace ImageLib.Apple
{
    public class Apple2LoResNtscImageFormat : NativeImageFormat
    {
        private const int _width = 40;
        private const int _height = 48;

        public double Aspect
        {
            get { return NtscPictureBuilder.PixelAspect; }
        }

        public BitmapSource FromNative(NativeImage native)
        {
            const int halfByteMask = 15;
            const int halfByteShift = 4;
            const int oddMask = 1;

            var builder = new NtscPictureBuilder(480, 192, 0);
            for (int y = 0; y < _height; ++y)
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
                    for (int x = 0; x < _width; ++x)
                    {
                        int inPixelOffset = inLineOffset + x;
                        if (inPixelOffset >= native.Data.Length)
                            break;

                        int pixelValue = (native.Data[inPixelOffset] >> shift) & halfByteMask;
                        pixelValue |= (pixelValue << 4) | (pixelValue << 8);

                        for (int i = 0; i < 12; ++i)
                        {
                            l1.Write(pixelValue);
                            l2.Write(pixelValue);
                            l3.Write(pixelValue);
                            l4.Write(pixelValue);
                            pixelValue >>= 1;
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
    }
}
