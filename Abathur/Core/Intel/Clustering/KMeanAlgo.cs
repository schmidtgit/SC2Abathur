using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abathur.Core.Intel.Clustering
{
    class KMeanAlgo
    {
        IList<Tile> _rawDataToCluster = new List<Tile>();
        IList<Tile> _normalizedDataToClutser = new List<Tile>();
        IList<Tile> _clusters = new List<Tile>();
        private int _numberOfClusters;

        public void InitializeRawData(IList<Tile> points)
        {
            _rawDataToCluster = points;
        }

        private void NormalizeData()//Gaussian normalization
        {
            double xSum = 0.0;
            double ySum = 0.0;
            double zSum = 0.0;
            foreach (Tile tile in _rawDataToCluster)
            {
                xSum += tile.X;
                ySum += tile.Y;
                zSum += tile.Z;
            }
            double xMean = xSum / _rawDataToCluster.Count;
            double yMean = ySum / _rawDataToCluster.Count;
            double zMean = zSum / _rawDataToCluster.Count;

            double sumX = 0.0;
            double sumY = 0.0;
            double sumZ = 0.0;

            foreach (Tile tile in _rawDataToCluster)
            {
                sumX += Math.Pow(tile.X - xMean, 2);
                sumY += Math.Pow(tile.Y - yMean, 2);
                sumZ += Math.Pow(tile.Z - zMean, 2);
            }

            double xSD = sumX / _rawDataToCluster.Count;
            double ySD = sumY / _rawDataToCluster.Count;
            double zSD = sumZ / _rawDataToCluster.Count;

            foreach (Tile tile in _rawDataToCluster)
            {
                _normalizedDataToClutser.Add(new Tile
                {
                    X = (tile.X - xMean) / xSD,
                    Y = (tile.Y - yMean) / ySD,
                    Z = (tile.Z - zMean) / zSD
                });
            }
        }

        private void InitializeCentroids()
        {
            Random random = new Random(_numberOfClusters);

            for (int i = 0; i < _numberOfClusters; ++i)//abitrarily add a Tile to each cluster
            {
                _rawDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = i;
            }

            for (int i = _numberOfClusters; i < _rawDataToCluster.Count; i++)
            {
                _rawDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = random.Next(0, _numberOfClusters);
            }
        }

        private bool UpdateDataPointMeans()
        {
            if (EmptyCluster(_rawDataToCluster)) return false;

            var groupToComputeMeans = _rawDataToCluster.GroupBy(s => s.Cluster).OrderBy(s => s.Key);
            int clusterIndex = 0;
            double x = 0.0;
            double y = 0.0;
            double z = 0.0;
            foreach (var item in groupToComputeMeans)
            {
                foreach (var value in item)
                {
                    x += value.X;
                    y += value.Y;
                    z += value.Z;
                }
                _clusters[clusterIndex].X = x / item.Count();
                _clusters[clusterIndex].Y = y / item.Count();
                _clusters[clusterIndex].Z = z / item.Count();
                clusterIndex++;
                x = 0.0;
                y = 0.0;
                z = 0.0;
            }
            return true;
        }
        private bool EmptyCluster(IList<Tile> data)
        {
            var emptyCluster =
                data.GroupBy(s => s.Cluster).OrderBy(s => s.Key).Select(g => new { Cluster = g.Key, Count = g.Count() });

            foreach (var item in emptyCluster)
            {
                if (item.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private double EuclidianDistance(Tile tile, Tile mean)
        {
            double _diffs;
            _diffs = Math.Pow(tile.X - mean.X, 2);
            _diffs += Math.Pow(tile.Y - mean.Y, 2);
            _diffs += Math.Pow(tile.Z - mean.Z, 2);
            return Math.Sqrt(_diffs);
        }

        private bool UpdateClusterMembership()
        {
            bool changed = false;

            double[] distances = new double[_numberOfClusters];

            for (int i = 0; i < _rawDataToCluster.Count; ++i)
            {

                for (int k = 0; k < _numberOfClusters; ++k)
                    distances[k] = EuclidianDistance(_rawDataToCluster[i], _clusters[k]);

                int newClusterId = MinIndex(distances);
                if (newClusterId != _rawDataToCluster[i].Cluster)
                {
                    changed = true;
                    _rawDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = newClusterId;
                }
            }
            if (changed == false)
                return false;
            if (EmptyCluster(_rawDataToCluster)) return false;
            return true;
        }

        private int MinIndex(double[] distances)
        {
            int _indexOfMin = 0;
            double _smallDist = distances[0];
            for (int k = 0; k < distances.Length; ++k)
            {
                if (distances[k] < _smallDist)
                {
                    _smallDist = distances[k];
                    _indexOfMin = k;
                }
            }
            return _indexOfMin;
        }

        public void Cluster(IList<Tile> data, int numClusters)
        {
            bool _changed = true;
            bool _success = true;
            InitializeCentroids();

            int maxIteration = data.Count * 10;
            int _threshold = 0;
            while (_success && _changed && _threshold < maxIteration)
            {
                ++_threshold;
                _success = UpdateDataPointMeans();
                _changed = UpdateClusterMembership();
            }
        }

        public IOrderedEnumerable<IGrouping<int, Tile>> Run(IList<Tile> tiles, int numClusters)
        {
            InitializeRawData(tiles);
            //NormalizeData();
            _numberOfClusters = numClusters; //TODO calculate optimal number of clusters.

            for (int i = 0; i < _numberOfClusters; i++)
            {
                _clusters.Add(new Tile() { Cluster = i });
            }

            Cluster(_rawDataToCluster, _numberOfClusters);
            return _rawDataToCluster.GroupBy(s => s.Cluster).OrderBy(s => s.Key);
        }

    }
}
