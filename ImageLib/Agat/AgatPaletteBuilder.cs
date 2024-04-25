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
                var seeder = new KMeansPlusPlusDeterministicSeeder<LabColor>(ops);
                var clustering = new KMeansClustering<LabColor>(ops, seeder);
                var samples = pixelList.Select(x => x.ToLab());
                var clusters = clustering.Find(samples, paletteSize);
                palette = clusters.Select(x => Quantize(x.ToSrgb()));
            }

            return new Palette(palette, paletteSize);
        }

        private static IEnumerable<Rgb>? CheckFits(IEnumerable<Rgb> pixels, int paletteSize)
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

        private class ColorSampleOps : ISampleOps<LabColor>
        {
            public double Metric(LabColor x, LabColor y) => x.Sub(y).LenSq();
            public LabColor Sum(LabColor x, LabColor y) => x.Add(y);
            public LabColor Average(LabColor sum, int count) => sum.Div(count);
            public bool CloseEnough(LabColor x, LabColor y) => Metric(x, y) < 1;
        }
    }
}
