using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageLib
{
    public class HgrImageFormat : AgatImageFormat
    {
        private const int _width = 256;
        private const int _height = 256;
        private const int _bytesPerScanline = 32;
        private const int _pixelsPerByte = 8;

        public string Name
        {
            get { return "HGR"; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public int ImageSizeInBytes
        {
            get { return _height * _bytesPerScanline; }
        }

        public Color GetPixel(byte[] pixels, int x, int y)
        {
            ValidateCoordinates(x, y);
            int b = pixels[y * _bytesPerScanline + (x / _pixelsPerByte)];
            return (b & (0x80 >> (x % _pixelsPerByte))) != 0 ? Color.White : Color.Black;
        }

        public void SetPixel(byte[] pixels, int x, int y, Color color)
        {
            ValidateCoordinates(x, y);
            
            int offset = y * 32 + (x / _pixelsPerByte);
            int bit = 0x80 >> (x % _pixelsPerByte);

            int b = pixels[offset];
            if (color.GetBrightness() >= 0.5)
            {
                b |= bit;
            }
            else
            {
                b &= ~bit;
            }
            pixels[offset] = (byte)b;
        }

        private void ValidateCoordinates(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x", x, "X must be between 0 and " + Width);
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y", y, "Y must be between 0 and " + Height);
        }
    }
}
