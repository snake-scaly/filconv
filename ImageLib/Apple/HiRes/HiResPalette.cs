using ImageLib.ColorManagement;

namespace ImageLib.Apple.HiRes
{
    public class HiResPalette : IHiResPalette
    {
        // 2^13 possible color combinations in a single septet. The bits in the index, starting from the
        // most significant bit, are OPpNnCccccccc, where
        //  O - parity of the screen column of the first pixel in the septet
        //  P - shift bit of the previous septet
        //  p - last (most significant) pixel bit of the previous septet
        //  N - shift bit of the next septet
        //  n - first (least significant) pixel bit of the next septet
        //  C - shift bit of the septet being matched
        //  c - pixel bits of the septet being matched
        private readonly Septet[] _entries;

        public HiResPalette(Septet[] entries)
        {
            _entries = entries;
        }

        public byte Match(Septet want, bool odd, byte previousByte)
        {
            var offset = (odd ? 1 << 12 : 0) | ((previousByte & 0xC0) << 4);
            
            var bestDist = double.PositiveInfinity;
            var bestIndex = -1;

            // Check all possible byte values and the next byte's potential shift and first color bit.
            for (var i = 0; i < (1 << 10); i++)
            {
                var d = SeptetDistanceSq(want, _entries[offset + i]);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestIndex = offset + i;
                }
            }
            
            // Return the best match ignoring the guessed next byte bits. The next byte will be influenced
            // by this one but no guarantee that it will be exactly as we guessed. A mismatch will count
            // towards the quantization error.
            return (byte)(bestIndex & 0xFF);

            double SeptetDistanceSq(Septet a, Septet b)
            {
                return
                    a.C1.Sub(b.C1).LenSq() +
                    a.C2.Sub(b.C2).LenSq() +
                    a.C3.Sub(b.C3).LenSq() +
                    a.C4.Sub(b.C4).LenSq() +
                    a.C5.Sub(b.C5).LenSq() +
                    a.C6.Sub(b.C6).LenSq() +
                    a.C7.Sub(b.C7).LenSq();
            }
        }
    }
}
