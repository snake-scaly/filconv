using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageLib
{
    public interface AgatImageFormat
    {
        string Name { get; }
        int Width { get; }
        int Height { get; }
        double Aspect { get; }
        int ImageSizeInBytes { get; }

        Color GetPixel(byte[] pixels, int x, int y);
        void SetPixel(byte[] pixels, int x, int y, Color color);
    }
}
