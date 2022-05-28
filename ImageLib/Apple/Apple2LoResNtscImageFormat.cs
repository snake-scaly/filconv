using System;
using System.Windows.Media.Imaging;
using FilLib;

namespace ImageLib.Apple
{
    public class Apple2LoResNtscImageFormat : Apple2ImageFormatAbstr
    {
        private readonly bool _doubleResolution;

        public Apple2LoResNtscImageFormat(bool doubleResolution)
        {
            _doubleResolution = doubleResolution;
        }

        public override AspectBitmap FromNative(NativeImage native, DecodingOptions options)
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

                    var scanlines = new[] { l0, l1, l2, l3, l4, l5, l6, l7 };

                    for (int x = 0; x < width; ++x)
                    {
                        int inPixelOffset = inLineOffset + x;

                        if (inPixelOffset >= native.Data.Length)
                            break;

                        if (_doubleResolution)
                        {
                            int inPixelOffsetEven = inPixelOffset + pageSize;
                            int value = inPixelOffsetEven < native.Data.Length ? native.Data[inPixelOffsetEven] : 0;
                            WritePixelColumn(value, scanlines, pixelWidth, ref tick);
                        }

                        WritePixelColumn(native.Data[inPixelOffset], scanlines, pixelWidth, ref tick);
                    }
                }
            }

            return builder.GetBitmap();
        }

        public override NativeImage ToNative(BitmapSource bitmap, EncodingOptions options)
        {
            throw new InvalidOperationException();
        }

        public override int ComputeMatchScore(NativeImage native)
        {
            var preferredMode = _doubleResolution
                ? ImageMeta.Mode.Apple_80_48_DoubleLoRes
                : ImageMeta.Mode.Apple_40_48_LoRes;
            if (native.Metadata?.DisplayMode == preferredMode)
                return NativeImageFormatUtils.MetaMatchScore;
            return NativeImageFormatUtils.ComputeMatch(native, _doubleResolution ? 0x800 : 0x400);
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
