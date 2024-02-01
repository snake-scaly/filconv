using System.Collections.Generic;
using System.Linq;
using ImageLib.ColorManagement;

namespace ImageLib.Util
{
    /// <summary>
    /// A color palette.
    /// </summary>
    /// <remarks>
    /// A palette may contain unimportant colors. These colors don't participate in color matching
    /// and don't affect palette sorting.
    /// </remarks>
    public class Palette : List<PaletteEntry>
    {
        /// <summary>
        /// Create an empty palette.
        /// </summary>
        public Palette()
        {
        }

        /// <summary>
        /// Create a palette of a given size populated with unimportant colors.
        /// </summary>
        public Palette(int size)
            : base(Enumerable.Repeat(default(PaletteEntry), size))
        {
        }

        /// <summary>
        /// Create a palette containing only the given colors.
        /// </summary>
        /// <remarks>
        /// All colors are marked as important.
        /// </remarks>
        public Palette(IEnumerable<Rgb> colors)
            : base(colors.Select(x => new PaletteEntry(x)))
        {
        }

        /// <summary>
        /// Create a palette of a specific size.
        /// </summary>
        /// <remarks>
        /// The given colors are placed at the start of the palette and are marked as important.
        /// The rest is filled with the default Rgb value and is marked as unimportant.
        /// </remarks>
        public Palette(IEnumerable<Rgb> colors, int size)
            : base(colors
                .Select(x => new PaletteEntry(x))
                .Concat(Enumerable.Repeat(default(PaletteEntry), int.MaxValue))
                .Take(size))
        {
        }

        /// <summary>
        /// Find the best match for the given color.
        /// </summary>
        /// <remarks>
        /// Unimportant colors are not considered.
        /// </remarks>
        public int Match(XyzColor c)
        {
            double sq(double x) => x * x;
            double diff(XyzColor a, XyzColor b) => sq(a.X - b.X) + sq(a.Y - b.Y) + sq(a.Z - b.Z);

            var n = Count;
            var best = -1;
            var bestD = double.PositiveInfinity;

            for (var i = 0; i < n; i++)
            {
                var entry = this[i];

                if (!entry.Important)
                    continue;

                var d = diff(c, entry.Linear);
                if (d < bestD)
                {
                    best = i;
                    bestD = d;
                }
            }

            return best;
        }

        public void Sort(Palette template)
        {
            double diff(PaletteEntry a, PaletteEntry b)
            {
                if (!a.Important || !b.Important)
                    return 10;
                var d = a.Linear.Sub(b.Linear);
                return d.X * d.X + d.Y * d.Y + d.Z * d.Z;
            }

            void swap(int i, int j)
            {
                var tmp = this[i];
                this[i] = this[j];
                this[j] = tmp;
            }

            var repeat = true;

            // Minimize square distances between palette and native colors.
            while (repeat)
            {
                repeat = false;

                for (var i = 0; i < Count - 1; i++)
                {
                    for (var j = i + 1; j < Count; j++)
                    {
                        var pi = this[i];
                        var pj = this[j];
                        var ti = template[i];
                        var tj = template[j];

                        if (!pi.Important && !pj.Important)
                            continue;
                        if (!ti.Important && !tj.Important)
                            continue;

                        var m1 = diff(pi, ti) + diff(pj, tj);
                        var m2 = diff(pi, tj) + diff(pj, ti);
                        if (m2 < m1)
                        {
                            swap(i, j);
                            repeat = true;
                        }
                    }
                }
            }
        }
    }
}
