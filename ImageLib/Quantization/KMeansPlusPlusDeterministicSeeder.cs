using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageLib.Quantization
{
    public class KMeansPlusPlusDeterministicSeeder<T> : IKMeansSeeder<T>
    {
        private readonly ISampleOps<T> _sampleOps;
        private readonly Random _rng = new Random();

        public KMeansPlusPlusDeterministicSeeder(ISampleOps<T> sampleOps)
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

            // Find the image gamut center.
            var center = samples[0];
            for (var i = 1; i < n; i++)
                center = _sampleOps.Sum(center, samples[i]);
            center = _sampleOps.Average(center, n);

            var work = new WeighedSample[n];

            // Initial weights are metrics to the center.
            for (var i = 0; i < n; i++)
            {
                var s = samples[i];
                var ws = new WeighedSample { Sample = s, Metric = _sampleOps.Metric(s, center) };
                work[i] = ws;
            }

            for (var i = 0; i < k; i++)
            {
                // Move the best sample to the i-th position.
                Array.Sort(work, i, n - i);
                var currentChoice = work[i];

                // Reduce metrics for samples which are now closer to the new seed.
                for (var j = i + 1; j < n; j++)
                {
                    var c = work[j];
                    var m = _sampleOps.Metric(c.Sample, currentChoice.Sample);
                    if (!(m < c.Metric))
                        continue;
                    c.Metric = m;
                    work[j] = c;
                }
            }

            // Return the first k samples from the work array. They're the seeds.
            return work.Take(k).Select(x => x.Sample);
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
