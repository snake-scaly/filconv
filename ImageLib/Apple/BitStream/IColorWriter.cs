using System;
using ImageLib.Util;

namespace ImageLib.Apple.BitStream
{
    public interface IColorWriter : IDisposable
    {
        void Write(Rgb c);
    }
}
