using System;
using ImageLib.Util;

namespace ImageLib.Apple
{
    public interface IColorWriter : IDisposable
    {
        void Write(Rgb c);
    }
}
