using System;
using System.Collections.Generic;
using System.Linq;
using FilLib;
using ImageLib.Apple;
using ImageLib.Apple.HiRes;
using ImageLib.ColorManagement;
using ImageLib.Util;

namespace ImageLib.Agat
{
    public class AgatAppleImageFormat : INativeImageFormat
    {
        private static readonly IHiResFragmentRenderer _colorFragmentRenderer;
        private static readonly IHiResFragmentRenderer _monoFragmentRenderer;
        private static readonly IHiResPalette _hiResPaletteColor;
        private static readonly IHiResPalette _hiResPaletteMono;

        public IEnumerable<NativeDisplay>? SupportedDisplays { get; } =
            new[] { NativeDisplay.Color, NativeDisplay.Mono, NativeDisplay.Meta };
        public IEnumerable<NativeDisplay>? SupportedEncodingDisplays { get; } =
            new[] { NativeDisplay.Color, NativeDisplay.Mono, NativeDisplay.Meta };

        static AgatAppleImageFormat()
        {
            var basicPaletteColor = HardwareToAppleColorOrder(AgatHardwareColors.Color);
            _colorFragmentRenderer = new HiResFragmentRenderer(basicPaletteColor, new StripedHiResFillPolicy());
            _hiResPaletteColor = new HiResPaletteBuilder(_colorFragmentRenderer).Build();

            var basicPaletteMono = HardwareToAppleColorOrder(AgatHardwareColors.Mono);
            _monoFragmentRenderer = new HiResFragmentRenderer(basicPaletteMono, new StripedHiResFillPolicy());
            _hiResPaletteMono = new HiResPaletteBuilder(_monoFragmentRenderer).Build();
        }

        public AspectBitmap FromNative(NativeImage native, DecodingOptions options)
        {
            var colors = HardwareToAppleColorOrder(AgatColorUtils.NativeDisplayToColors(options.Display, native.Metadata));
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
            var metaPalette = ImageMeta.Palette.Agat_1;
            Rgb[] metaColors;

            switch (options.Display)
            {
                case NativeDisplay.Color:
                    palette = _hiResPaletteColor;
                    renderer = _colorFragmentRenderer;
                    metaColors = AgatHardwareColors.Color;
                    break;
                case NativeDisplay.Mono:
                    palette = _hiResPaletteMono;
                    renderer = _monoFragmentRenderer;
                    metaColors = AgatHardwareColors.Mono;
                    break;
                case NativeDisplay.Meta:
                    var basePalette = new AgatPaletteBuilder().Build(bitmap.AllPixelsInRect(0, 0, 280, 192), 6);
                    // Sorting colors by luminosity seems to give better results.
                    basePalette.Sort(new Palette(HardwareToAppleColorOrder(AgatHardwareColors.Mono)));
                    renderer = new HiResFragmentRenderer(basePalette.Select(x => x.Value).ToList(), new StripedHiResFillPolicy());
                    palette = new HiResPaletteBuilder(renderer).Build();
                    metaPalette = ImageMeta.Palette.Custom;
                    metaColors = AppleToHardwareColorOrder(basePalette);
                    break;
                default:
                    throw new Exception($"Unsupported display {options.Display:G}");
            }

            var quantizer = options.Dither
                ? (IHiResQuantizer)new HiResDitheringQuantizer(palette, renderer)
                : new HiResNearestColorQuantizer(palette);
            var data = quantizer.Quantize(bitmap);
            var meta = new ImageMeta
            {
                DisplayMode = ImageMeta.Mode.Agat_280_192_AppleHiRes,
                PaletteType = metaPalette,
                CustomPalette = metaColors.Select(AgatColorUtils.RgbToUint).ToList(),
            };
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
            return native.Metadata?.PaletteType == ImageMeta.Palette.Custom
                ? new DecodingOptions { Display = NativeDisplay.Meta }
                : new DecodingOptions { Display = NativeDisplay.Color, Palette = NativePalette.Default };
        }

        public IEnumerable<NativePalette>? GetSupportedPalettes(NativeDisplay display) => null;

        private static Rgb[] HardwareToAppleColorOrder(Rgb[] rgb16) => new[]
        {
            rgb16[0],
            rgb16[5],
            rgb16[4],
            rgb16[2],
            rgb16[1],
            rgb16[7],
        };

        private static Rgb[] AppleToHardwareColorOrder(Palette rgb6) => new[]
        {
            rgb6[0].Value,
            rgb6[4].Value,
            rgb6[3].Value,
            default,
            rgb6[2].Value,
            rgb6[1].Value,
            default,
            rgb6[5].Value,
            default,
            default,
            default,
            default,
            default,
            default,
            default,
            default,
        };
    }
}
