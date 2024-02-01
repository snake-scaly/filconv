using System;
using ImageLib.ColorManagement;

namespace ImageLib.Apple.BitStream
{
    public interface IColorWriter : IDisposable
    {
        void Write(Rgb c);
    }
}
