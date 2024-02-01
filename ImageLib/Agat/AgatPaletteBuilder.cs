using System;
using System.Collections.Generic;
using System.Linq;
using ImageLib.ColorManagement;
using ImageLib.Quantization;
using ImageLib.Util;

namespace ImageLib.Agat
{
    public class AgatPaletteBuilder
    {
        public Palette Build(IEnumerable<Rgb> pixels, int paletteSize)
        {
            var pixelList = pixels.Select(Quantize).ToList();

            var palette = CheckFits(pixelList, paletteSize);

            if (palette == null)
            {
                var ops = new ColorSampleOps();
                var seeder = new KMeansPlusPlusDeterministicSeeder<XyzColor>(ops);
                var clustering = new KMeansClustering<XyzColor>(ops, seeder);
                var samples = pixelList.Select(x => ColorSpace.Srgb.ToXyz(x));
                var clusters = clustering.Find(samples, paletteSize);
                palette = clusters.Select(x => Quantize(ColorSpace.Srgb.FromXyz(x)));
            }

            return new Palette(palette, paletteSize);
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

        private class ColorSampleOps : ISampleOps<XyzColor>
        {
            public double Metric(XyzColor x, XyzColor y)
            {
                var d = x.Sub(y);
                return d.X * d.X + d.Y * d.Y + d.Z * d.Z;
            }

            public XyzColor Sum(XyzColor x, XyzColor y)
            {
                return x.Add(y);
            }

            public XyzColor Average(XyzColor sum, int count)
            {
                return sum.Div(count);
            }

            public bool CloseEnough(XyzColor x, XyzColor y)
            {
                bool c(double a, double b) => Math.Abs(a - b) <= 0.001;
                return c(x.X, y.X) && c(x.Y, y.Y) && c(x.Z, y.Z);
            }
        }
    }
}
