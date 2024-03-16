using System;
using System.Collections.Generic;
using FilLib;
using ImageLib.Apple.BitStream;
using ImageLib.Apple.HiRes;

namespace ImageLib.Apple
{
    public class Apple2HiResImageFormat : Apple2ImageFormatAbstr
    {
        private const int _width = 280;
        private const int _height = 192;
        private const int _totalBytes = 0x2000;

        private static readonly IHiResFragmentRenderer _rendererFilled;
        private static readonly IHiResPalette _paletteFilled;
        private static readonly IHiResFragmentRenderer _rendererStriped;
        private static readonly IHiResPalette _paletteStriped;

        static Apple2HiResImageFormat()
        {
            _rendererFilled = new HiResFragmentRenderer(Apple2HardwareColors.HiRes, new FilledHiResFillPolicy());
            _paletteFilled = new HiResPaletteBuilder(_rendererFilled).Build();
            _rendererStriped = new HiResFragmentRenderer(Apple2HardwareColors.HiRes, new StripedHiResFillPolicy());
            _paletteStriped = new HiResPaletteBuilder(_rendererStriped).Build();
        }

        public override IEnumerable<NativeDisplay> SupportedDisplays { get; } =
            new[] { NativeDisplay.ColorFilled, NativeDisplay.ColorStriped, NativeDisplay.Mono, NativeDisplay.Artifact };

        public override IEnumerable<NativeDisplay> SupportedEncodingDisplays { get; } =
            new[] { NativeDisplay.ColorFilled, NativeDisplay.ColorStriped };

        public override AspectBitmap FromNative(NativeImage native, DecodingOptions options)
        {
            switch (options.Display)
            {
                case NativeDisplay.ColorFilled: return NativeToRgb(native, fill: true);
                case NativeDisplay.ColorStriped: return NativeToRgb(native, fill: false);
                case NativeDisplay.Mono: return NativeToBitStream(native, new MonoPictureBuilder());
                case NativeDisplay.Artifact: return NativeToBitStream(native, new NtscPictureBuilder(0));
                default: throw new ArgumentException($"Unsupported display {options.Display:G}", nameof(options));
            }
        }

        public override NativeImage ToNative(IReadOnlyPixels src, EncodingOptions options)
        {
            var fill = options.Display == NativeDisplay.ColorFilled;
            var palette = fill ? _paletteFilled : _paletteStriped;
            var renderer = fill ? _rendererFilled : _rendererStriped;
            var quantizer = options.Dither
                ? (IHiResQuantizer)new HiResDitheringQuantizer(palette, renderer)
                : new HiResNearestColorQuantizer(palette);
            var data = quantizer.Quantize(src);
            var meta = new ImageMeta { DisplayMode = ImageMeta.Mode.Apple_280_192_HiRes };
            return new NativeImage { Data = data, Metadata = meta };
        }

        public override int ComputeMatchScore(NativeImage native)
        {
            if (native.Metadata?.DisplayMode == ImageMeta.Mode.Apple_280_192_HiRes)
                return NativeImageFormatUtils.MetaMatchScore;
            return NativeImageFormatUtils.ComputeMatch(native, _totalBytes);
        }

        private AspectBitmap NativeToRgb(NativeImage native, bool fill)
        {
            var pixels = new byte[_width * 4 * _height];
            var renderer = fill ? _rendererFilled : _rendererStriped;

            for (var y = 0; y < _height; y++)
            {
                renderer.RenderLine(
                    new ArraySegment<byte>(native.Data, Apple2Utils.GetHiResLineOffset(y), 40),
                    new ArraySegment<byte>(pixels, _width * 4 * y, _width * 4));
            }

            return AspectBitmap.FromImageAspect(new Bgr32BitmapData(pixels, _width, _height), 4 / 3.0);
        }

        private AspectBitmap NativeToBitStream(NativeImage native, IBitStreamPictureBuilder builder)
        {
            for (int y = 0; y < _height; ++y)
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
