using System.Collections.Generic;
using System.Linq;

namespace ImageLib.Quantization
{
    public class KMeansClustering<T>
    {
        private readonly ISampleOps<T> _sampleOps;
        private readonly IKMeansSeeder<T> _seeder;

        public KMeansClustering(ISampleOps<T> sampleOps, IKMeansSeeder<T> seeder)
        {
            _sampleOps = sampleOps;
            _seeder = seeder;
        }

        public IEnumerable<T> Find(IEnumerable<T> samples, int k)
        {
            var points = samples.ToList();
            var clusters = _seeder.Seed(points, k).Select(x => new Cluster { Centroid = x }).ToList();
            bool repeat = true;

            var n = points.Count;
            k = clusters.Count;

            while (repeat)
            {
                repeat = false;

                for (var i = 0; i < n; i++)
                {
                    var point = points[i];
                    var cluster = Closest(clusters, point);
                    if (cluster.Count != 0)
                        cluster.Accumulator = _sampleOps.Sum(cluster.Accumulator, point);
                    else
                        cluster.Accumulator = point;
                    cluster.Count++;
                }

                for (var i = 0; i < k; i++)
                {
                    var cluster = clusters[i];
                    if (cluster.Count == 0)
                    {
                        // This cluster was consumed by nearby clusters.
                        cluster.Centroid = default;
                        continue;
                    }
                    var newCentroid = _sampleOps.Average(cluster.Accumulator, cluster.Count);
                    if (!_sampleOps.CloseEnough(newCentroid, cluster.Centroid))
                    {
                        cluster.Centroid = newCentroid;
                        repeat = true;
                    }
                    cluster.Count = 0;
                }
            }

            return clusters.Select(x => x.Centroid);
        }

        private Cluster Closest(IList<Cluster> clusters, T point)
        {
            Cluster best = null;
            var bestMetric = double.PositiveInfinity;
            var k = clusters.Count;
            for (var i = 0; i < k; i++)
            {
                var cluster = clusters[i];
                var metric = _sampleOps.Metric(cluster.Centroid, point);
                if (metric < bestMetric)
                {
                    best = cluster;
                    bestMetric = metric;
                }
            }
            return best;
        }

        private class Cluster
        {
            public T Centroid;
            public T Accumulator;
            public int Count;
        }
    }
}
