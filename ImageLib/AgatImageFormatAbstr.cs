using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageLib
{
    public abstract class AgatImageFormatAbstr : AgatImageFormat
    {
        public abstract string Name { get; }
        public abstract int Width { get; }
        public abstract int Height { get; }

        public virtual double Aspect
        {
            get { return 4.0 / 3.0; }
        }

        public int ImageSizeInBytes
        {
            get { return BytesPerScanline * Height; }
        }

        protected abstract int BitsPerPixel { get; }
        protected abstract Color[] Palette { get; }

        protected int PixelsPerByte
        {
            get { return 8 / BitsPerPixel; }
        }

        protected int BytesPerScanline
        {
            get { return Width / PixelsPerByte; }
        }

        public Color GetPixel(byte[] pixels, int x, int y)
        {
            ValidateCoordinates(x, y);
            int pixelIndex;
            int byteInLine = Math.DivRem(x, PixelsPerByte, out pixelIndex);
            int offset = GetLineOffset(y) + byteInLine;
            int b = pixels[offset];
            b >>= (PixelsPerByte - pixelIndex - 1) * BitsPerPixel;
            b &= (1 << BitsPerPixel) - 1;
            return Palette[b];
        }

        public void SetPixel(byte[] pixels, int x, int y, Color color)
        {
            ValidateCoordinates(x, y);
            int pixelIndex;
            int byteInLine = Math.DivRem(x, PixelsPerByte, out pixelIndex);
            int offset = GetLineOffset(y) + byteInLine;
            int b = pixels[offset];
            int shift = (PixelsPerByte - pixelIndex - 1) * BitsPerPixel;
            int mask = ((1 << BitsPerPixel) - 1) << shift;
            int index = GetClosestColor(Palette, color) << shift;
            b = b & ~mask | index;
            pixels[offset] = (byte)b;
        }

        protected virtual int GetLineOffset(int y)
        {
            return y * BytesPerScanline;
        }

        void ValidateCoordinates(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x", x, "X must be between 0 and " + Width);
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y", y, "Y must be between 0 and " + Height);
        }

        static int GetClosestColor(Color[] palette, Color color)
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

        static int GetColorProximity(Color c1, Color c2)
        {
            int dr = c1.R - c2.R;
            int dg = c1.G - c2.G;
            int db = c1.B - c2.B;
            return dr * dr + dg * dg + db * db;
        }
    }
}
