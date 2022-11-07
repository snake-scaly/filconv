using System;
using System.Collections.Generic;
using System.Linq;
using ImageLib.Quantization;
using ImageLib.Util;

namespace ImageLib.Agat
{
    public class AgatPaletteBuilder
    {
        public IEnumerable<Rgb> Build(IEnumerable<Rgb> pixels, int paletteSize)
        {
            var pixelList = pixels.ToList();

            var palette = CheckFits(pixelList, paletteSize);

            if (palette == null)
            {
                var ops = new ColorSampleOps();
                var seeder = new KMeansPlusPlusDeterministicSeeder<ColorSample>(ops);
                var clustering = new KMeansClustering<ColorSample>(ops, seeder);
                var samples = pixelList.Select(ColorSample.FromRgb);
                var clusters = clustering.Find(samples, paletteSize);
                palette = clusters.Select(x => x.ToRgb());
            }

            return palette
                .Select(Quantize)
                .Concat(Enumerable.Repeat(Rgb.FromRgb(0, 0, 0), paletteSize))
                .Take(paletteSize);
        }

        private static IEnumerable<Rgb> CheckFits(IEnumerable<Rgb> pixels, int paletteSize)
        {
            var distinct = new HashSet<Rgb>();
            foreach (var c in pixels)
            {
                distinct.Add(c);
                if (distinct.Count > paletteSize)
                    return null;
            }
            return distinct;
        }

        private static Rgb Quantize(Rgb c) => Rgb.FromRgb(Quantize(c.R), Quantize(c.G), Quantize(c.B));
        private static byte Quantize(byte x) => (byte)((x + 8) / 17 * 17);
        private static double Linear(byte x) => Math.Pow(x / 255.0, 1 / 2.2);
        private static byte Gamma(double x) => (byte)Math.Round(Math.Pow(x, 2.2) * 255);

        private struct ColorSample
        {
            public double R;
            public double G;
            public double B;

            public static ColorSample FromRgb(Rgb c) =>
                new ColorSample { R = Linear(c.R), G = Linear(c.G), B = Linear(c.B) };

            public Rgb ToRgb() => Rgb.FromRgb(Gamma(R), Gamma(G), Gamma(B));
        }

        private class ColorSampleOps : ISampleOps<ColorSample>
        {
            public double Metric(ColorSample x, ColorSample y)
            {
                var dr = x.R - y.R;
                var dg = x.G - y.G;
                var db = x.B - y.B;
                return dr * dr + dg * dg + db * db;
            }

            public ColorSample Sum(ColorSample x, ColorSample y)
            {
                return new ColorSample { R = x.R + y.R, G = x.G + y.G, B = x.B + y.B };
            }

            public ColorSample Average(ColorSample sum, int count)
            {
                return new ColorSample { R = sum.R / count, G = sum.G / count, B = sum.B / count };
            }

            public bool CloseEnough(ColorSample x, ColorSample y)
            {
                bool c(double a, double b) => Math.Abs(a - b) < 0.001;
                return c(x.R, y.R) && c(x.G, y.G) && c(x.B, y.B);
            }
        }
    }
}
