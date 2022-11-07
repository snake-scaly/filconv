using System;
using System.Collections.Generic;

namespace ImageLib.Quantization
{
    public class KMeansRandomSeeder<T> : IKMeansSeeder<T>
    {
        public IEnumerable<T> Seed(IList<T> samples, int k)
        {
            var random = new Random();
            for (var i = 0; i < k; i++)
                yield return samples[(int)(random.NextDouble() * samples.Count)];
        }
    }
}
