using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageLib
{
    public class MgrImageFormat : AgatImageFormat
    {
        private const int _width = 128;
        private const int _height = 128;
        private const int _bpp = 4;
        private const int _bytesPerScanline = 64;

        public string Name
        {
            get { return "MGR"; }
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
            get { return _bytesPerScanline * _height; }
        }

        public System.Drawing.Color GetPixel(byte[] pixels, int x, int y)
        {
            ValidateCoordinates(x, y);
            int offset = y * _bytesPerScanline + x / 2;
            int b = pixels[offset];
            if ((x & 1) == 0)
            {
                b >>= _bpp;
            }
            b &= 0xF;
            return _colorPalette[b];
        }

        public void SetPixel(byte[] pixels, int x, int y, System.Drawing.Color color)
        {
            ValidateCoordinates(x, y);
            int offset = y * _bytesPerScanline + x / 2;
            int b = pixels[offset];
            int mask = 0xF;
            int index = GetClosestColor(_colorPalette, color);
            if ((x & 1) == 0)
            {
                mask <<= _bpp;
                index <<= _bpp;
            }
            b = b & ~mask | index;
            pixels[offset] = (byte)b;
        }

        private void ValidateCoordinates(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x", x, "X must be between 0 and " + Width);
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y", y, "Y must be between 0 and " + Height);
        }

        private static int GetClosestColor(Color[] palette, Color color)
        {
            int bestProx = GetColorProximity(palette[0], color);
            int bestPal = 0;

            for (int i = 1; i < palette.Length; ++i)
            {
                int prox = GetColorProximity(palette[i], color);
                if (prox < bestProx)
                {
                    bestProx = prox;
                    bestPal = i;
                }
            }

            return bestPal;
        }

        private static int GetColorProximity(Color c1, Color c2)
        {
            int dr = c1.R - c2.R;
            int dg = c1.G - c2.G;
            int db = c1.B - c2.B;
            return dr * dr + dg * dg + db * db;
        }

        private static readonly Color[] _colorPalette =
        {
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(217, 0, 0),
            Color.FromArgb(0, 217, 0),
            Color.FromArgb(217, 217, 0),
            Color.FromArgb(0, 0, 217),
            Color.FromArgb(217, 0, 217),
            Color.FromArgb(0, 217, 217),
            Color.FromArgb(217, 217, 217),
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(255, 0, 0),
            Color.FromArgb(0, 255, 0),
            Color.FromArgb(255, 255, 0),
            Color.FromArgb(0, 0, 255),
            Color.FromArgb(255, 0, 255),
            Color.FromArgb(0, 255, 255),
            Color.FromArgb(255, 255, 255),
        };

        private static readonly Color[] _bwPalette =
        {
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(102, 102, 102),
            Color.FromArgb(68, 68, 68),
            Color.FromArgb(171, 171, 171),
            Color.FromArgb(47, 47, 47),
            Color.FromArgb(149, 149, 149),
            Color.FromArgb(115, 115, 115),
            Color.FromArgb(217, 217, 217),
            Color.FromArgb(38, 38, 38),
            Color.FromArgb(140, 140, 140),
            Color.FromArgb(106, 106, 106),
            Color.FromArgb(208, 208, 208),
            Color.FromArgb(84, 84, 84),
            Color.FromArgb(187, 187, 187),
            Color.FromArgb(153, 153, 153),
            Color.FromArgb(255, 255, 255),
        };
    }
}
