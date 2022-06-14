using System.Collections.Generic;

namespace ImageLib.Quantization
{
    public interface IKMeansSeeder<T>
    {
        IEnumerable<T> Seed(IList<T> samples, int k);
    }
}
