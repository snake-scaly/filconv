using System;
using FilLib;
using ImageLib.Apple.BitStream;
using ImageLib.Apple.HiRes;

namespace ImageLib.Apple
{
    public class Apple2HiResImageFormat : Apple2ImageFormatAbstr
    {
        private static readonly IHiResFragmentRenderer _renderer;
        private static readonly IHiResPalette _palette;

        private const int width = 280;
        private const int height = 192;
        private const int totalBytes = 0x2000;

        static Apple2HiResImageFormat()
        {
            _renderer = new HiResFragmentRenderer(Apple2HardwareColors.HiRes, new FilledHiResFillPolicy());
            _palette = new HiResPaletteBuilder(_renderer).Build();
        }

        public override AspectBitmap FromNative(NativeImage native, DecodingOptions options)
        {
            switch (options.Display)
            {
                case NativeDisplay.Color: return NativeToRgb(native);
                case NativeDisplay.Mono: return NativeToBitStream(native, new MonoPictureBuilder());
                case NativeDisplay.Artifact: return NativeToBitStream(native, new NtscPictureBuilder(0));
                default: throw new ArgumentException($"Unsupported display {options.Display:G}", nameof(options));
            }
        }

        public override NativeImage ToNative(IReadOnlyPixels src, EncodingOptions options)
        {
            var quantizer = options.Dither
                ? (IHiResQuantizer)new HiResDitheringQuantizer(_palette, _renderer)
                : new HiResNearestColorQuantizer(_palette);
            var data = quantizer.Quantize(src);
            var meta = new ImageMeta { DisplayMode = ImageMeta.Mode.Apple_280_192_HiRes };
            return new NativeImage { Data = data, Metadata = meta };
        }

        public override int ComputeMatchScore(NativeImage native)
        {
            if (native.Metadata?.DisplayMode == ImageMeta.Mode.Apple_280_192_HiRes)
                return NativeImageFormatUtils.MetaMatchScore;
            return NativeImageFormatUtils.ComputeMatch(native, totalBytes);
        }

        private AspectBitmap NativeToRgb(NativeImage native)
        {
            var pixels = new byte[width * 4 * height];

            for (var y = 0; y < height; y++)
            {
                _renderer.RenderLine(
                    new ArraySegment<byte>(native.Data, Apple2Utils.GetHiResLineOffset(y), 40),
                    new ArraySegment<byte>(pixels, width * 4 * y, width * 4));
            }

            return AspectBitmap.FromImageAspect(new Bgr32BitmapData(pixels, width, height), 4 / 3.0);
        }

        private AspectBitmap NativeToBitStream(NativeImage native, IBitStreamPictureBuilder builder)
        {
            for (int y = 0; y < height; ++y)
            {
                int lineOffset = Apple2Utils.GetHiResLineOffset(y);
                if (lineOffset >= native.Data.Length)
                    continue;

                using (IScanlineWriter scanline = builder.GetScanlineWriter(y))
                {
                    var outBit = 0;

                    for (var i = 0; i < 40; i++)
                    {
                        var dataByte = native.Data[lineOffset + i];
                        var shiftReg = dataByte & 0x7F;
                        var phaseShift = (dataByte >> 7) & 1;

                        for (var t = 0; t < 14; t++)
                        {
                            if ((t & 1) == phaseShift)
                            {
                                outBit = shiftReg & 1;
                                shiftReg >>= 1;
                            }
                            scanline.Write(outBit);
                        }
                    }
                }
            }

            return AspectBitmap.FromImageAspect(builder.Build(), 4.0 / 3.0);
        }
    }
}
