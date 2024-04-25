using System.Collections.Generic;
using System.Linq;

namespace ImageLib.Quantization
{
    public class KMeansClustering<T>
        where T : struct
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

            while (repeat)
            {
                repeat = false;

                foreach (var point in points)
                {
                    var cluster = Closest(clusters, point);
                    cluster.Accumulator = cluster.Count == 0 ? point : _sampleOps.Sum(cluster.Accumulator, point);
                    cluster.Count++;
                }

                foreach (var cluster in clusters)
                {
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
            Cluster best = clusters[0];
            var bestMetric = _sampleOps.Metric(best.Centroid, point);
            foreach (var cluster in clusters.Skip(1))
            {
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
