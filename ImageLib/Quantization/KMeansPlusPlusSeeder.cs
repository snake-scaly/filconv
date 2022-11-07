using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageLib.Quantization
{
    public class KMeansPlusPlusSeeder<T> : IKMeansSeeder<T>
    {
        private readonly ISampleOps<T> _sampleOps;
        private readonly Random _rng = new Random();

        public KMeansPlusPlusSeeder(ISampleOps<T> sampleOps)
        {
            _sampleOps = sampleOps;
        }

        public IEnumerable<T> Seed(IList<T> samples, int k)
        {
            var n = samples.Count;
            if (n <= k)
            {
                return samples;
            }

            // Choose the first seed randomly.
            var firstSeedIndex = _rng.Next(n);
            var firstSeed = samples[firstSeedIndex];

            var work = new WeighedSample[n];
            var totalWeight = 0.0;

            // Initial weights are metrics to the first sample. Compute total weight.
            for (var i = 0; i < n; i++)
            {
                var s = samples[i];
                var ws = new WeighedSample { Sample = s, Metric = _sampleOps.Metric(s, firstSeed) };
                totalWeight += ws.Metric;
                work[i] = ws;
            }

            // Move the first sample to index 0.
            Swap(work, 0, firstSeedIndex);

            for (var i = 1; i < k; i++)
            {
                // Pick a random weight within totalWeight.
                var choice = _rng.NextDouble() * totalWeight;

                // Sum weights in the list. The item that overshoots choice is our next seed.
                // Heavier items cover more weight range and therefore are more likely to be chosen.
                int j;
                for (j = i; j < n - 1; j++)
                {
                    choice -= work[j].Metric;
                    if (choice <= 0)
                        break;
                }

                // Move the seed out of the remaining samples.
                Swap(work, i, j);
                var currentChoice = work[i];
                totalWeight -= currentChoice.Metric;

                // Reduce metrics for samples which are now closer to the new seed and adjust totalWeight accordingly.
                for (j = i + 1; j < n; j++)
                {
                    var c = work[j];
                    var m = _sampleOps.Metric(c.Sample, currentChoice.Sample);
                    if (m < c.Metric)
                    {
                        totalWeight -= c.Metric - m;
                        c.Metric = m;
                        work[j] = c;
                    }
                }
            }

            // Return the first k samples from the work array. They're the seeds.
            return work.Take(k).Select(x => x.Sample);
        }

        private void Swap(IList<WeighedSample> samples, int i, int j)
        {
            var tmp = samples[i];
            samples[i] = samples[j];
            samples[j] = tmp;
        }

        private struct WeighedSample : IComparable<WeighedSample>
        {
            public T Sample;
            public double Metric;

            public int CompareTo(WeighedSample other)
            {
                return other.Metric.CompareTo(Metric);
            }
        }
    }
}
