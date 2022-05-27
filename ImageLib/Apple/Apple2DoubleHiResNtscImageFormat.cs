using System;
using System.Windows.Media.Imaging;

namespace ImageLib.Apple
{
    public class Apple2DoubleHiResNtscImageFormat : INativeImageFormat
    {
        private const int _lines = 192;
        private const int _bytesPerHalfScreen = 8192;

        public AspectBitmap FromNative(NativeImage native)
        {
            const int wordsPerLine = 20;
            const int bytesPerWord = 2;
            const int significantBitsPerByte = 7;
            const int significantBitsMask = (1 << significantBitsPerByte) - 1;
            const int initialPhase = 1;

            var builder = new NtscPictureBuilder(initialPhase);

            for (int y = 0; y < _lines; ++y)
            {
                int lineOffset = Apple2Utils.GetHiResLineOffset(y);
                if (lineOffset + _bytesPerHalfScreen >= native.Data.Length)
                    continue;

                using (NtscScanLine scanline = builder.GetScanLine(y))
                {
                    for (int w = 0; w < wordsPerLine; ++w)
                    {
                        int wordOffsetLo = lineOffset + w * bytesPerWord;
                        int wordOffsetHi = wordOffsetLo + _bytesPerHalfScreen;
                        if (wordOffsetHi + bytesPerWord > native.Data.Length)
                            break;

                        int word =
                            (native.Data[wordOffsetLo] & significantBitsMask) |
                            ((native.Data[wordOffsetHi] & significantBitsMask) << significantBitsPerByte) |
                            ((native.Data[wordOffsetLo + 1] & significantBitsMask) << significantBitsPerByte * 2) |
                            ((native.Data[wordOffsetHi + 1] & significantBitsMask) << significantBitsPerByte * 3);

                        for (int i = 0; i < significantBitsPerByte * 4; ++i)
                        {
                            scanline.Write(word & 1);
                            word >>= 1;
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
