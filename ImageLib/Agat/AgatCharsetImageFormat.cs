using System;
using System.Collections.Generic;
using ImageLib.ColorManagement;

namespace ImageLib.Agat;

public class AgatCharsetImageFormat : INativeImageFormat
{
    private readonly bool _agat9;
    private const int _width = 7 * 16 + 15;
    private const int _height = 8 * 16 + 15;

    private static readonly byte[] _bg = [0, 0, 0, 255];
    private static readonly byte[] _fg = [255, 255, 255, 255];
    private static readonly byte[] _grid = [255, 0, 0, 255];

    public IEnumerable<NativeDisplay>? SupportedDisplays => null;
    public IEnumerable<NativeDisplay>? SupportedEncodingDisplays => null;

    public AgatCharsetImageFormat(bool agat9)
    {
        _agat9 = agat9;
    }

    public AspectBitmap FromNative(NativeImage native, DecodingOptions options)
    {
        var src = native.Data;
        var dst = new byte[_width * _height * 4];

        for (var i = 0; i < dst.Length; i += 4)
            Array.Copy(_grid, 0, dst, i, _grid.Length);

        for (var i = 0; i < 256; i++)
        {
            var srcOffset = i * 8;
            var row = Math.DivRem(i, 16, out var column);
            var dstOffset = (row * 9 * _width + column * 8) * 4;

            for (var j = 0; j < 8; j++, srcOffset++, dstOffset += _width * 4)
            {
                if (srcOffset >= src.Length)
                    goto Done;

                RenderByte(Shuffle(src[srcOffset]), dst, dstOffset);
            }
        }

        Done:
        return new AspectBitmap(new Bgr32BitmapData(dst, _width, _height), 1);
    }

    public NativeImage ToNative(IReadOnlyPixels bitmap, EncodingOptions options)
    {
        var dst = new byte[256 * 8];

        for (var row = 0; row < 16; row++)
        {
            for (var column = 0; column < 16; column++)
            {
                var dstOffset = (row * 16 + column) * 8;
                for (var i = 0; i < 8; i++)
                    dst[dstOffset + i] = Shuffle(DecodeByte(bitmap, column * 8, row * 9 + i));
            }
        }

        return new NativeImage { Data = dst };
    }

    public int ComputeMatchScore(NativeImage native)
    {
        return NativeImageFormatUtils.ComputeMatch(native, 256);
    }

    public DecodingOptions GetDefaultDecodingOptions(NativeImage native)
    {
        return new DecodingOptions();
    }

    public IEnumerable<NativePalette>? GetSupportedPalettes(NativeDisplay display)
    {
        return null;
    }

    private byte Shuffle(byte b)
    {
        return _agat9 ? Flip(b) : (byte)~b;
    }

    private static void RenderByte(byte pixels, byte[] dst, int offset)
    {
        for (var i = 0; i < 7; i++, pixels >>= 1)
        {
            var c = (pixels & 1) == 0 ? _bg : _fg;
            Array.Copy(c, 0, dst, offset + i * 4, c.Length);
        }
    }

    private static byte DecodeByte(IReadOnlyPixels bitmap, int x, int y)
    {
        byte result = 0;

        for (var i = 0; i < 7; i++, x++)
        {
            var c = GetPixelSafe(bitmap, x, y);
            if (c.R != 0 || c.G != 0 || c.B != 0)
                result |= (byte)(1 << i);
        }

        return result;
    }

    private static byte Flip(byte b)
    {
        var x = ((b & 0xAA) >> 1) | ((b & 0x55) << 1);
        x = ((x & 0xCC) >> 2) | ((x & 0x33) << 2);
        x = ((x & 0xF0) >> 4) | ((x & 0x0F) << 4);
        return (byte)x;
    }

    private static Rgb GetPixelSafe(IReadOnlyPixels bitmap, int x, int y)
    {
        if (x < 0 || y < 0 || x >= bitmap.Width || y >= bitmap.Height)
            return Rgb.FromRgb(_bg[2], _bg[1], _bg[0]);
        return bitmap.GetPixel(x, y);
    }
}
