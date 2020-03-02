using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace ImageLib.Apple
{
    public class Apple2LoResNtscImageFormat : NativeImageFormat
    {
        private readonly bool _doubleResolution;

        public double Aspect
        {
            get { return NtscPictureBuilder.PixelAspect; }
        }

        public Apple2LoResNtscImageFormat(bool doubleResolution)
        {
            _doubleResolution = doubleResolution;
        }

        public BitmapSource FromNative(NativeImage native)
        {
            const int width = 40;
            const int height = 24;
            const int pageSize = 1024;
            int pixelWidth = _doubleResolution ? 7 : 14;
            var builder = new NtscPictureBuilder(0);

            for (int y = 0; y < height; ++y)
            {
                int inLineOffset = Apple2Utils.GetTextLineOffset(y);

                using (NtscScanLine l0 = builder.GetScanLine(y * 8 + 0))
                using (NtscScanLine l1 = builder.GetScanLine(y * 8 + 1))
                using (NtscScanLine l2 = builder.GetScanLine(y * 8 + 2))
                using (NtscScanLine l3 = builder.GetScanLine(y * 8 + 3))
                using (NtscScanLine l4 = builder.GetScanLine(y * 8 + 4))
                using (NtscScanLine l5 = builder.GetScanLine(y * 8 + 5))
                using (NtscScanLine l6 = builder.GetScanLine(y * 8 + 6))
                using (NtscScanLine l7 = builder.GetScanLine(y * 8 + 7))
                {
                    int tick = 0;

                    var scanlines = new NtscScanLine[]{ l0, l1, l2, l3, l4, l5, l6, l7 };

                    for (int x = 0; x < width; ++x)
                    {
                        int inPixelOffset = inLineOffset + x;

                        if (inPixelOffset >= native.Data.Length)
                            break;
                        WritePixelColumn(native.Data[inPixelOffset], scanlines, pixelWidth, ref tick);

                        if (_doubleResolution)
                        {
                            inPixelOffset += pageSize;
                            int value = inPixelOffset < native.Data.Length ? native.Data[inPixelOffset] : 0;
                            WritePixelColumn(value, scanlines, pixelWidth, ref tick);
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

        private void WritePixelColumn(int value, NtscScanLine[] scanlines, int pixelWidth, ref int tick)
        {
            int top = value & 0xF;
            int bottom = (value >> 4) & 0xF;

            for (int i = 0; i < pixelWidth; ++i)
            {
                int topBit = (top >> (tick & 3)) & 1;
                int bottomBit = (bottom >> (tick & 3)) & 1;

                scanlines[0].Write(topBit);
                scanlines[1].Write(topBit);
                scanlines[2].Write(topBit);
                scanlines[3].Write(topBit);

                scanlines[4].Write(bottomBit);
                scanlines[5].Write(bottomBit);
                scanlines[6].Write(bottomBit);
                scanlines[7].Write(bottomBit);

                ++tick;
            }
        }
    }
}
