using System;
using ImageLib.Util;

namespace ImageLib.Apple
{
    public interface ColorWriter : IDisposable
    {
        void Write(Rgb c);
        void Close();
    }
}
