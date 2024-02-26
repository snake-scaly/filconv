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
        private readonly LinearSeptet[] _entries;

        public HiResPalette(LinearSeptet[] entries)
        {
            _entries = entries;
        }

        public ColorMatchResult Match(LinearSeptet want, bool odd, byte previousByte)
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
            return new ColorMatchResult
            {
                Septet = _entries[bestIndex],
                Native = (byte)(bestIndex & 0xFF),
            };
            
            double SeptetDistanceSq(LinearSeptet a, LinearSeptet b)
            {
                return
                    DistanceSq(a.C1, b.C1) +
                    DistanceSq(a.C2, b.C2) +
                    DistanceSq(a.C3, b.C3) +
                    DistanceSq(a.C4, b.C4) +
                    DistanceSq(a.C5, b.C5) +
                    DistanceSq(a.C6, b.C6) +
                    DistanceSq(a.C7, b.C7);
            }

            double DistanceSq(XyzColor a, XyzColor b)
            {
                var d = a.Sub(b);
                return d.X * d.X + d.Y * d.Y + d.Z * d.Z;
            }
        }
    }
}