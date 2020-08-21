using System;
using System.Collections.Generic;
using Abathur.Core.Intel.Clustering;
using Abathur.Model;
using NydusNetwork.API.Protocol;

namespace Abathur.Core.Intel
{
    public class Region
    {
        public int RegionId { get; set; }
        public List<Tile> Tiles { get; private set; } = new List<Tile>();
        public IColony Colony { get; set; }
        public IList<Region> ConnectedRegions { get; set; }
        public List<Tile> Frontier { get; private set; } = new List<Tile>();
        public int MinX { get; set; } = Int32.MaxValue;
        public int MaxX { get; set; } = Int32.MinValue;
        public int MinY { get; set; } = Int32.MaxValue;
        public int MaxY { get; set; } = Int32.MinValue;

        public bool InRegion(Unit unit)
        {
            var x = unit.Pos.X;
            var y = unit.Pos.Y; 

            return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
        }

        public bool InRegion(Point2D point) {
            var x = point.X;
            var y = point.Y;

            return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
        }

        public override string ToString()
        {
            return "Region Id: " + RegionId + "\n" +
                   "Colony: " + Colony.Point + "\n" +
                   "Tiles: " + Tiles.Count + "\n" +
                   "connected Regions: " + ConnectedRegions.Count + "\n" +
                   "frontier Tiles: " + Frontier.Count + "\n" +
                   "minitmum Point: (" + MinX + "," + MinY + ")\n" +
                   "maximum Point: (" + MaxX + "," + MaxY + ")\n";
        }
    }
}
