using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ImageLib;

namespace FilConv;

public static class AspectBitmapExtensions
{
    public static AspectBitmapSource ToAspectBitmapSource(this AspectBitmap bmp)
    {
        var pinnedPixels = GCHandle.Alloc(bmp.Bitmap.Pixels, GCHandleType.Pinned);

        try
        {
            var pixelPtr = pinnedPixels.AddrOfPinnedObject();

            var writeableBitmap = new WriteableBitmap(
                PixelFormat.Bgra8888,
                AlphaFormat.Opaque,
                pixelPtr,
                new PixelSize(bmp.Bitmap.Width, bmp.Bitmap.Height),
                new Vector(96, 96),
                bmp.Bitmap.Width * 4);

            return new AspectBitmapSource(writeableBitmap, bmp.PixelAspect);
        }
        finally
        {
            pinnedPixels.Free();
        }
    }
}
