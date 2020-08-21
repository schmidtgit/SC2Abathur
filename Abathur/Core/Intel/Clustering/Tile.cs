using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Abathur.Core.Intel.Clustering
{
    public class Tile
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public List<Tile> Neighbors { get; set; }
        public int Cluster { get; set; }
        public double StraightLineCost { get; set; }
        public int CostToStart { get; set; }
        public double ExpectedTotalCost { get; set; }
        public bool Visited { get; set; }

        public Tile(double x, double y, double z, List<Tile> neighbors = null)
        {
            X = x;
            Y = y;
            Z = z;
            Neighbors = neighbors;
            Cluster = 0;
        }
        public Tile()
        {
            
        }

        public double HeuristicDistanceTo(Tile tile) //ignoring z
        {
            return Math.Sqrt(Math.Pow(tile.X - X, 2) + Math.Pow(tile.Y - Y, 2));
        }
        public override string ToString()
        {
            return "X = " + X + " Y = " + Y + " Z = " + Z + "\n StraightLineCost = " + StraightLineCost +
                   " CostToStart = " + CostToStart + " ExpextedTotalCost = " + ExpectedTotalCost;
        }
    }
}
