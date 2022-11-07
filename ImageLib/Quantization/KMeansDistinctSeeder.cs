using System.Collections.Generic;
using System.Linq;

namespace ImageLib.Quantization
{
    public class KMeansDistinctSeeder<T> : IKMeansSeeder<T>
    {
        private readonly IEqualityComparer<T> _comparer;

        public KMeansDistinctSeeder(IEqualityComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public IEnumerable<T> Seed(IList<T> samples, int k)
        {
            return samples.Distinct(_comparer).Take(k);
        }
    }
}
