using System;
using System.Collections.Generic;
using FilLib;
using ImageLib.Apple;
using ImageLib.Apple.HiRes;
using ImageLib.ColorManagement;

namespace ImageLib.Agat
{
    public class AgatAppleImageFormat : INativeImageFormat
    {
        private static readonly IHiResFragmentRenderer _colorFragmentRenderer;
        private static readonly IHiResFragmentRenderer _monoFragmentRenderer;
        private static readonly IHiResPalette _hiResPaletteColor;
        private static readonly IHiResPalette _hiResPaletteMono;

        public IEnumerable<NativeDisplay> SupportedDisplays { get; } =
            new[] { NativeDisplay.Color, NativeDisplay.Mono };
        public IEnumerable<NativeDisplay> SupportedEncodingDisplays { get; } =
            new[] { NativeDisplay.Color, NativeDisplay.Mono };

        static AgatAppleImageFormat()
        {
            var basicPaletteColor = PalToApple(AgatColorUtils.NativeDisplayToColors(NativeDisplay.Color, null));
            _colorFragmentRenderer = new HiResFragmentRenderer(basicPaletteColor, new StripedHiResFillPolicy());
            _hiResPaletteColor = new HiResPaletteBuilder(_colorFragmentRenderer).Build();

            var basicPaletteMono = PalToApple(AgatColorUtils.NativeDisplayToColors(NativeDisplay.Mono, null));
            _monoFragmentRenderer = new HiResFragmentRenderer(basicPaletteMono, new StripedHiResFillPolicy());
            _hiResPaletteMono = new HiResPaletteBuilder(_monoFragmentRenderer).Build();
        }

        public AspectBitmap FromNative(NativeImage native, DecodingOptions options)
        {
            var colors = PalToApple(AgatColorUtils.NativeDisplayToColors(options.Display, native.Metadata));
            var stripedRenderer = new HiResFragmentRenderer(colors, new StripedHiResFillPolicy());
            var pixels = new byte[280 * 4 * 192];
            for (var y = 0; y < 192; y++)
            {
                stripedRenderer.RenderLine(
                    new ArraySegment<byte>(native.Data, Apple2Utils.GetHiResLineOffset(y), 40),
                    new ArraySegment<byte>(pixels, 280 * 4 * y, 280 * 4));
            }
            return AspectBitmap.FromImageAspect(new Bgr32BitmapData(pixels, 280, 192), 4 / 3.0);
        }

        public NativeImage ToNative(IReadOnlyPixels bitmap, EncodingOptions options)
        {
            IHiResPalette palette;
            IHiResFragmentRenderer renderer;

            switch (options.Display)
            {
                case NativeDisplay.Color:
                    palette = _hiResPaletteColor;
                    renderer = _colorFragmentRenderer;
                    break;
                case NativeDisplay.Mono:
                    palette = _hiResPaletteMono;
                    renderer = _monoFragmentRenderer;
                    break;
                default:
                    throw new Exception($"Unsupported display {options.Display:G}");
            }

            var quantizer = options.Dither
                ? (IHiResQuantizer)new HiResDitheringQuantizer(palette, renderer)
                : new HiResNearestColorQuantizer(palette);
            var data = quantizer.Quantize(bitmap);
            var meta = new ImageMeta { DisplayMode = ImageMeta.Mode.Agat_280_192_AppleHiRes };
            return new NativeImage { Data = data, Metadata = meta };
        }

        public int ComputeMatchScore(NativeImage native)
        {
            if (native.Metadata?.DisplayMode == ImageMeta.Mode.Agat_280_192_AppleHiRes)
                return NativeImageFormatUtils.MetaMatchScore;
            return NativeImageFormatUtils.ComputeMatch(native, 0x2000);
        }

        public DecodingOptions GetDefaultDecodingOptions(NativeImage native)
        {
            return new DecodingOptions { Display = NativeDisplay.Color, Palette = NativePalette.Default };
        }

        public IEnumerable<NativePalette> GetSupportedPalettes(NativeDisplay display) => null;

        private static Rgb[] PalToApple(IList<Rgb> agatColors)
        {
            return new[]
            {
                agatColors[0],
                agatColors[13],
                agatColors[12],
                agatColors[10],
                agatColors[9],
                agatColors[15]
            };
        }
    }
}
