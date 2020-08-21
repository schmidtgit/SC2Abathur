using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Abathur.Constants;
using Abathur.Core.Intel.Clustering;
using Abathur.Extensions;
using Abathur.Model;
using Abathur.Modules.Services;
using NydusNetwork.API.Protocol;

namespace Abathur.Core.Intel.Map
{
    public class MapAnalyser
    {
        private const double MAGIC_DISTANCE = 25;
        private const double MAX_Z_DISTANCE = 0.5;
        
        /// <summary>
        /// Given an enumerable of colonies and a List of Tiles, clusters the tiles into regions 
        /// </summary>
        /// <param name="colonies">List of all Colonies that should be attached to regions</param>
        /// <param name="graph">List of the Tiles that represent the entire map</param>
        /// <returns></returns>
        public IList<Region> GenerateRegions(IEnumerable<IColony> colonies, IList<Tile> graph)
        {
            var kMean = new KMeanAlgo();

            Console.WriteLine("running kmean-algorithm (Clustering)");
            var sw2 = new Stopwatch();
            sw2.Start();
            var pathableGroups = kMean.Run(graph,20);
            sw2.Stop();
            Console.WriteLine("Clustering took: " + sw2.Elapsed.Minutes + " mins and " + sw2.Elapsed.Seconds + " Secs.");

            var regions = new List<Region>();
            foreach (var g in pathableGroups)
            {
                var region = new Region {RegionId = g.Key, ConnectedRegions = new List<Region>()};
                foreach (var tile in g)
                {
                    region.Tiles.Add(tile);
                    if (tile.Neighbors.Any(t => t.Cluster != tile.Cluster))
                    {
                        region.Frontier.Add(tile);
                    }
                    region.MaxX = Math.Max((int)tile.X,region.MaxX);
                    region.MaxY = Math.Max((int)tile.Y,region.MaxY);
                    region.MinX = Math.Min((int)tile.X,region.MinX);
                    region.MinY = Math.Min((int)tile.Y,region.MinY);
                }

                foreach (var colony in colonies)
                {
                    if (region.InRegion(colony.Point))
                    {
                        region.Colony = colony;
                    }
                }

                regions.Add(region);
            }
            var unvisited = regions;
            var regions2 = new List<Region>();

            while (unvisited.Count!=0)
            {
                var region = unvisited.First();
                unvisited.Remove(region);

                foreach(var region1 in unvisited)
                {
                    var regionAdded = false;
                    foreach(var tile in region.Frontier) {
                        if(regionAdded)
                            break;
                        foreach(var tile1 in region1.Frontier) {
                            if(tile.HeuristicDistanceTo(tile1) <= 1) {
                                region.ConnectedRegions.Add(region1);
                                region1.ConnectedRegions.Add(region);
                                regionAdded = true;
                                break;
                            }
                        }
                    }
                }
                regions2.Add(region);
            }
            return regions2;
        }
        /// <summary>
        /// Given a map creates a list of tiles, concurrently and asyncronously connects them to a graph and returns the graph
        /// </summary>
        /// <param name="map">The Map over which there should be made a graph</param>
        /// <returns>A graph of connected Tiles</returns>
        public async Task<IList<Tile>> CreateGraph(GameMap map)
        {
            var height = map.TerrainHeight.Height;
            var width = map.NonPathable.Width;
            var graph = new List<Tile>();
            Console.WriteLine("Constructing Tiles/populating graph");
            for(int x = 0; x < width; x++)
                for(int y = 0; y < height; y++) {
                    if(!map.NonPathable.IsSet(x,y)) {
                        graph.Add(new Tile(x,height - y - 1,map.TerrainHeight.GetNumber(x,y) * 10));//TODO Hacking height to gain meaningful clustering.
                    }
                }
            Console.WriteLine("Connecting graph");
            var sw = new Stopwatch();
            sw.Start();
            //var tasks = new List<Task>();
            //int i;
            //for(i = 0; i <= graph.Count; i = i + (graph.Count / 8)) {
            //    var tenTiles = graph.Skip(i).Take(graph.Count / 8);
            //    tasks.Add(Task.Run(() => {
            //        foreach(var t in tenTiles) {
            //            t.Neighbors = GetNeighbors(t,graph,map.NonPathable);
            //        }
            //    }));
            //}

            //tasks.Add(Task.Run(() => {
            //    foreach(var t in graph.Skip(i - graph.Count / 8)) {
            //        t.Neighbors = GetNeighbors(t,graph,map.NonPathable);
            //    }
            //}));
            var tasks = graph.Select(tile =>
                Task.Run(() => tile.Neighbors = GetNeighbors(tile,graph,map.NonPathable)));

            await Task.WhenAll(tasks);
            sw.Stop();
            Console.WriteLine("Connecting the graph took: " + sw.Elapsed.Minutes + " mins and " + sw.Elapsed.Seconds +
                              " Secs... ");
            return graph;
        }
        private List<Tile> GetNeighbors(Tile tile,List<Tile> tiles,ImageDataHandler nonPathable) {
            var neighbors = new List<Tile>();
            neighbors.Add(tiles.Find(t => t.X == tile.X - 1 && t.Y == tile.Y - 1));
            neighbors.Add(tiles.Find(t => t.X == tile.X - 1 && t.Y == tile.Y));
            neighbors.Add(tiles.Find(t => t.X == tile.X - 1 && t.Y == tile.Y + 1));
            neighbors.Add(tiles.Find(t => t.X == tile.X && t.Y == tile.Y - 1));
            neighbors.Add(tiles.Find(t => t.X == tile.X && t.Y == tile.Y + 1));
            neighbors.Add(tiles.Find(t => t.X == tile.X + 1 && t.Y == tile.Y - 1));
            neighbors.Add(tiles.Find(t => t.X == tile.X + 1 && t.Y == tile.Y));
            neighbors.Add(tiles.Find(t => t.X == tile.X + 1 && t.Y == tile.Y + 1));

            neighbors.RemoveAll(i => i == null);
            return neighbors;
        }


        private static IColony ClosestColony(IEnumerable<IColony> colonies,IPosition position)
            => colonies.OrderBy(c => MinimumFastDistance(c.Minerals,position)).FirstOrDefault();
        private static double MinimumFastDistance(IEnumerable<IPosition> units,IPosition target)
            => units.Min(u => u.Point.FastDistance(target.Point));
        
        public static List<IntelColony> GetColonies(List<IUnit> minerals, List<IUnit> vespeneGeysers,IList<Point2D> startingLocations) {
            var result = ClusterMinerals(minerals);
            AddVespeneGeysers(result,vespeneGeysers);
            Merge3Mineral1VespeneGold(result);
            EnrichWithStartingLocations(result,startingLocations);
            return result;
        }

        private static List<IntelColony> ClusterMinerals(List<IUnit> minerals) {
            var result = new List<IntelColony>();
            var first = minerals.First();
            minerals.Remove(first);
            List<IUnit> grouping = new List<IUnit> { first };
            while(true) {
                var closeMinerals = minerals.Where(u => Math.Abs(u.Pos.Z - first.Pos.Z) < MAX_Z_DISTANCE && MinimumFastDistance(grouping,u) < MAGIC_DISTANCE);
                if(closeMinerals.Any()) {
                    foreach(var u in closeMinerals.ToList()) {
                        grouping.Add(u);
                        minerals.Remove(u);
                    }
                } else {
                    var x = grouping[0].Pos.X;
                    var y = grouping[0].Pos.Y;
                    result.Add(new IntelColony {
                        Id = (uint)result.Count,
                        Minerals = grouping,
                        Point = new Point2D { X = x,Y = y } //temp point
                    });
                    first = minerals.FirstOrDefault();
                    if(first == null)
                        break;
                    minerals.Remove(first);
                    grouping = new List<IUnit> { first };
                }
            }
            return result;
        }

        private static void AddVespeneGeysers(List<IntelColony> colonies, List<IUnit> vespeneGeysers) {
            foreach(var vespene in vespeneGeysers) {
                var colony = (IntelColony) ClosestColony(colonies.Where(c => Math.Abs(c.Minerals.First().Pos.Z - vespene.Pos.Z) < MAX_Z_DISTANCE),vespene);
                List<IUnit> vespeneList;
                if(colony.Vespene.Any())
                    vespeneList = colony.Vespene.ToList();
                else
                    vespeneList = new List<IUnit>();
                vespeneList.Add(vespene);
                colony.Vespene = vespeneList;
                colony.DesiredVespeneWorkers = vespeneList.Count * 3;
            }
        }

        private static void Merge3Mineral1VespeneGold(List<IntelColony> colonies) {
            var splitColonies = colonies.Where(c => c.Minerals.Count() == 3 && c.Vespene.Count() == 1).ToList();
            while(splitColonies.Count() != 0) {
                var colony = splitColonies.First();
                splitColonies.Remove(colony);
                var close = colony.GetClosest(splitColonies);
                colonies.Remove(close);
                splitColonies.Remove(close);
                colony.Minerals = colony.Minerals.Concat(close.Minerals).ToList();
                colony.Vespene = colony.Vespene.Concat(close.Vespene).ToList();
                colony.DesiredVespeneWorkers = colony.Vespene.Count() * 3;
            }
        }

        private static void EnrichWithStartingLocations(List<IntelColony> colonies, IList<Point2D> startingLocations) {
            foreach(var colony in colonies) {
                var minX = colony.Vespene.Min(v => v.Pos.X);
                var maxX = colony.Vespene.Max(v => v.Pos.X);
                var minY = colony.Vespene.Min(v => v.Pos.Y);
                var maxY = colony.Vespene.Max(v => v.Pos.Y);
                colony.Point = OptimalPosition(colony,minX,minY,maxX,maxY);
            }
            foreach(var position in startingLocations) {
                var colony = position.GetClosest(colonies);
                colony.IsStartingLocation = true;
                colony.Point = position;
            }
        }

        private static Point2D OptimalPosition(IColony colony,float minX,float minY,float maxX,float maxY) {
            minX -= 5f;
            maxX += 5f;
            minY -= 5f;
            maxY += 5f;
            var deltaX = (maxX - minX);
            var deltaY = (maxY - minY);

            List<Point2D> legalPosition = new List<Point2D>();
            for(int i = 0; i < deltaX; i++) {
                for(int j = 0; j < deltaX; j++) {
                    var point = new Point2D { X = minX + i,Y = minY + j };
                    if(LegalPosition(colony,point))
                        legalPosition.Add(point);
                }
            }
            var result = legalPosition.OrderBy(p => colony.Minerals.Sum(m => Math.Pow(p.Distance(m.Point), 2))).First();
            return result;
        }

        private const float MIN_NATURAL = 6;
        private static bool LegalPosition(IColony colony, Point2D point)
            => !colony.Minerals.Any(m => point.Distance(m.Point.X - 0.5f,m.Point.Y) < MIN_NATURAL || point.Distance(m.Point.X + 0.5f,m.Point.Y) < MIN_NATURAL);
    }
}
