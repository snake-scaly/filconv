using System.Drawing;
using System.Drawing.Imaging;

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

        public static byte[] GetBytes(Bitmap bmp, AgatImageFormat format)
        {
            byte[] bytes = new byte[format.ImageSizeInBytes];
            for (int y = 0; y < format.Height; ++y)
            {
                for (int x = 0; x < format.Width; ++x)
                {
                    Color pixel = (x < bmp.Width && y < bmp.Height) ? bmp.GetPixel(x, y) : Color.Black;
                    format.SetPixel(bytes, x, y, pixel);
                }
            }
            return bytes;
        }
    }
}
