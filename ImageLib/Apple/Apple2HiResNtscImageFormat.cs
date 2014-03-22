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
            const int width = 560;
            const int height = 192;
            const int firstBitPhase = 1;
            const int bytesPerLine = 40;

            var builder = new NtscPictureBuilder(width, height, firstBitPhase);

            for (int y = 0; y < height; ++y)
            {
                int lineOffset = Apple2Utils.GetHiResLineOffset(y);
                bool previousShift = false;

                if (lineOffset >= native.Data.Length)
                    continue;

                using (NtscScanLine scanline = builder.GetScanLine(y))
                {
                    for (int i = 0; i < bytesPerLine; ++i)
                    {
                        int bitsOffset = lineOffset + i;
                        if (bitsOffset > native.Data.Length)
                            break;

                        int bits = native.Data[bitsOffset];
                        bool skipBit = false;

                        bool shift = (bits & 0x80) == 0; // zero bit 7 means 90 degree phase shift
                        if (shift && !previousShift)
                        {
                            scanline.Write(0);
                        }
                        else if (!shift && previousShift)
                        {
                            skipBit = true;
                        }
                        previousShift = shift;

                        for (int b = 0; b < 7; ++b)
                        {
                            if (skipBit)
                            {
                                skipBit = false;
                            }
                            else
                            {
                                scanline.Write(bits);
                            }
                            scanline.Write(bits);
                            bits >>= 1;
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
