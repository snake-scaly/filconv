using System.Drawing;
using System.Drawing.Imaging;
using System;
using System.Linq;

namespace ImageLib
{
    public static class AgatImageConverter
    {
        public static Bitmap GetBitmap(byte[] bytes, AgatImageFormat format)
        {
            var bmp = new Bitmap(format.Width, format.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < format.Height; ++y)
            {
                for (int x = 0; x < format.Width; ++x)
                {
                    bmp.SetPixel(x, y, format.GetPixel(bytes, x, y));
                }
            }
            return bmp;
        }

        public static byte[] GetBytes(Bitmap bmp, AgatImageFormat format, bool dither)
        {
            var currentLineErrors = new Error[format.Width];
            var nextLineErrors = new Error[format.Width];

            byte[] bytes = new byte[format.ImageSizeInBytes];
            for (int y = 0; y < format.Height; ++y)
            {
                for (int x = 0; x < format.Width; ++x)
                {
                    Color pixel = (x < bmp.Width && y < bmp.Height) ? bmp.GetPixel(x, y) : Color.Black;

                    double r = 0, g = 0, b = 0;
                    if (dither)
                    {
                        r = Clamp(pixel.R + currentLineErrors[x].R);
                        g = Clamp(pixel.G + currentLineErrors[x].G);
                        b = Clamp(pixel.B + currentLineErrors[x].B);
                        pixel = Color.FromArgb(Round(r), Round(g), Round(b));
                    }

                    format.SetPixel(bytes, x, y, pixel);

                    if (dither)
                    {
                        pixel = format.GetPixel(bytes, x, y);
                        double re = (r - pixel.R) / 4;
                        double ge = (g - pixel.G) / 4;
                        double be = (b - pixel.B) / 4;

                        if (x + 1 < format.Width)
                        {
                            AddError(re, ge, be, ref currentLineErrors[x + 1]);
                        }
                        if (y + 1 < format.Height)
                        {
                            AddError(re, ge, be, ref nextLineErrors[x]);
                            if (x - 1 >= 0)
                            {
                                AddError(re, ge, be, ref nextLineErrors[x - 1]);
                            }
                            if (x + 1 < format.Width)
                            {
                                AddError(re, ge, be, ref nextLineErrors[x + 1]);
                            }
                        }
                    }
                }

                currentLineErrors = nextLineErrors;
                nextLineErrors = new Error[format.Width];
            }
            return bytes;
        }

        static double Clamp(double c)
        {
            return Math.Min(Math.Max(c, 0), 255);
        }

        static byte Round(double c)
        {
            return (byte)Math.Round(c);
        }

        static void AddError(double re, double ge, double be, ref Error e)
        {
            e.R += re;
            e.G += ge;
            e.B += be;
        }

        struct Error
        {
            public double R, G, B;
         
            public Error(double r, double g, double b)
            {
                R = r;
                G = g;
                B = b;
            }
        }
    }
}
