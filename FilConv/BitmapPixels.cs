using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ImageLib;
using ImageLib.ColorManagement;

namespace FilConv;

public class BitmapPixels : IReadOnlyPixels
{
    private readonly byte[] _pixels;
    private readonly int _stride;

    /// <summary>
    /// Extract pixel data from a bitmap.
    /// </summary>
    /// <param name="bitmap">bitmap to get pixels from</param>
    public BitmapPixels(Bitmap bitmap)
    {
        var bgr32 = new WriteableBitmap(bitmap.PixelSize, bitmap.Dpi, PixelFormat.Bgra8888);
        using (var fb = bgr32.Lock())
            bitmap.CopyPixels(fb, AlphaFormat.Opaque);

        Width = bgr32.PixelSize.Width;
        Height = bgr32.PixelSize.Height;
        _stride = Width * 4;

        _pixels = new byte[Height * _stride];
        var pinnedArray = GCHandle.Alloc(_pixels, GCHandleType.Pinned);

        try
        {
            var ptr = pinnedArray.AddrOfPinnedObject();
            bgr32.CopyPixels(new PixelRect(bgr32.PixelSize), ptr, _pixels.Length, _stride);
        }
        finally
        {
            pinnedArray.Free();
        }
    }

    public int Width { get; }
    public int Height { get; }

    public Rgb GetPixel(int x, int y)
    {
        ValidateCoordinates(x, y);
        int offset = 4 * x + _stride * y;
        return Rgb.FromRgb(_pixels[offset + 2], _pixels[offset + 1], _pixels[offset]);
    }

    private void ValidateCoordinates(int x, int y)
    {
        if (x < 0 || x >= Width)
            throw new ArgumentOutOfRangeException("x", x, "X must be between 0 and " + Width);
        if (y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException("y", y, "Y must be between 0 and " + Height);
    }
}
