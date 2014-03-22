using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ImageLib.Apple
{
    public interface ColorWriter : IDisposable
    {
        void Write(Color c);
        void Close();
    }
}
