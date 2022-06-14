using System;
using System.Collections.Generic;
using System.Linq;
using ImageLib.Quantization;
using Xunit;

namespace ImageLib.Tests
{
    public class KMeansClusteringTests
    {
        private readonly KMeansClustering<double> _clustering;

        public KMeansClusteringTests()
        {
            _clustering = new KMeansClustering<double>(
                new DoubleSampleOps(),
                new KMeansDistinctSeeder<double>(new DoubleEqualityComparer()));
        }

        [Fact]
        public void Find_Basic()
        {
            var samples = new[] { 0.0, 1.0, 10.0, 11.0 };
            var clusters = _clustering.Find(samples, 2).ToList();
            Assert.Equal(2, clusters.Count);
            Assert.Contains(0.5, clusters);
            Assert.Contains(10.5, clusters);
        }

        [Fact]
        public void Find_RepeatedSamples()
        {
            var samples = new[] { 0.0, 0.0, 1.0, 10.0, 11.0 };
            var clusters = _clustering.Find(samples, 2).ToList();
            Assert.Equal(2, clusters.Count);
            Assert.Contains(1.0 / 3, clusters);
            Assert.Contains(10.5, clusters);
        }

        [Fact]
        public void Find_NotEnoughDistinctValues()
        {
            var samples = new[] { 0.0, 0.0, 0.0, 1.0, 1.0 };
            var clusters = _clustering.Find(samples, 3).ToList();
            Assert.Equal(2, clusters.Count);
            Assert.Contains(0.0, clusters);
            Assert.Contains(1.0, clusters);
        }

        private class DoubleSampleOps : ISampleOps<double>
        {
            public double Metric(double x, double y)
            {
                return Math.Abs(x - y);
            }

            public double Sum(double x, double y)
            {
                return x + y;
            }

            public double Average(double sum, int count)
            {
                return sum / count;
            }

            public bool CloseEnough(double x, double y)
            {
                return x == y;
            }
        }

        private class DoubleEqualityComparer : IEqualityComparer<double>
        {
            public bool Equals(double x, double y)
            {
                return x == y;
            }

            public int GetHashCode(double obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
