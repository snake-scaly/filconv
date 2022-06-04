using System;

namespace ImageLib.Apple
{
    /// Users must call Dispose to properly write colors to the destination.
    public interface IScanlineWriter : IDisposable
    {
        /// Write one bit into the scanline.
        void Write(int bit);
    }
}
