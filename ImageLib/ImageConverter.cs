using System.Drawing;
using System.Drawing.Imaging;

namespace ImageLib
{
    public static class ImageConverter
    {
        public static Bitmap GetBitmap(byte[] bytes)
        {
            return GetBitmap(bytes, new HgrImageFormat());
        }

        public static byte[] GetBytes(Bitmap bmp)
        {
            return GetBytes(bmp, new HgrImageFormat());
        }

        public static Bitmap GetBitmap(byte[] bytes, ImageFormat format)
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

        public static byte[] GetBytes(Bitmap bmp, ImageFormat format)
        {
            byte[] bytes = new byte[format.ImageSizeInBytes];
            for (int y = 0; y < format.Height; ++y)
            {
                for (int x = 0; x < format.Width; ++x)
                {
                    format.SetPixel(bytes, x, y, bmp.GetPixel(x, y));
                }
            }
            return bytes;
        }
    }
}
