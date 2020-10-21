using Abathur.Constants;
using Abathur.Extensions;
using NydusNetwork.API.Protocol;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Abathur.Core.Intel.Clustering;
using Abathur.Modules.Services;
using Color = System.Drawing.Color;
using Abathur.Repositories;
using Abathur.Model;

//TODO: Handle lift-off (Terran)
//TODO: Spine, Spore up-root (Zerg)
namespace Abathur.Core.Intel.Map {
    public class GameMap : IGameMap {
        public IAbilityRepository abilityRepository;
        private IUnitTypeRepository unitTypeRepository;
        public MapHandler BlockedGrid { get; set; }
        public MapHandler NaturalGrid { get; set; }
        public MapHandler PlacementGrid { get; set; }
        public MapHandler Explored { get; set; }
        public MapHandler Creep { get; set; }
        public MapHandler Power { get; set; }
        public MapHandler NonPathable { get; set; }
        public MapHandler TerrainHeight { get; set; }
        public IList<Tile> Graph { get; set; }
        public IList<Region> Regions { get; set; }
        private IRawManager rawManager;

        public GameMap(IAbilityRepository abilityRepository, IUnitTypeRepository unitTypeRepository, IRawManager rawManager) {
            this.abilityRepository = abilityRepository;
            this.unitTypeRepository = unitTypeRepository;
            this.rawManager = rawManager;
        }

        /*
    private void RunMapAnalyzer(IIntelManager intelManager) {

        var mapAna = new MapAnalyser();
        Task.Run(async () => {
            var sw = new Stopwatch();
            sw.Start();
            Graph = await mapAna.CreateGraph(this);
            Regions = mapAna.GenerateRegions(intelManager.Colonies, Graph);
            sw.Stop();
            Console.WriteLine("MapAnalyser took: " + sw.Elapsed.Minutes + " mins and " + sw.Elapsed.Seconds + " seconds...");
        });
        
    }
        */

        public void Initialize(ResponseGameInfo info, IIntelManager intelManager) {
            PlacementGrid = MapHandler.Instantiate(info.StartRaw.PlacementGrid);
            NonPathable = MapHandler.Instantiate(info.StartRaw.PathingGrid);
            TerrainHeight = MapHandler.Instantiate(info.StartRaw.TerrainHeight);
            BlockedGrid = MapHandler.Instantiate(PlacementGrid.Width, PlacementGrid.Height);
            NaturalGrid = MapHandler.Instantiate(PlacementGrid.Width, PlacementGrid.Height);
            Power = MapHandler.Instantiate(PlacementGrid.Width, PlacementGrid.Height);
            Creep = MapHandler.Instantiate(PlacementGrid.Width, PlacementGrid.Height);
            Explored = MapHandler.Instantiate(PlacementGrid.Width, PlacementGrid.Height);
            intelManager.Handler.RegisterHandler(Case.StructureAddedSelf, u => EventUnitAdded(u));
            intelManager.Handler.RegisterHandler(Case.StructureDestroyed, u => EventUnitDestroyed(u));
        }

        public void RegisterNatural(NydusNetwork.API.Protocol.Point point, float radius) => UpdateImageGrid(radius + 3.5f, point.X, point.Y, 1, NaturalGrid);
        public void RegisterStructure(NydusNetwork.API.Protocol.Point point, float radius) => UpdateImageGrid(radius, point.X, point.Y, 1, BlockedGrid);
        private void EventUnitDestroyed(IUnit unit) => UpdateBlockedGrid(unitTypeRepository.Get(unit.UnitType), unit.Point, 0);
        public void EventUnitAdded(IUnit unit) => UpdateBlockedGrid(unitTypeRepository.Get(unit.UnitType), unit.Point, 1);
        private float FootprintRadius(UnitTypeData structure) => abilityRepository.Get(structure.AbilityId).FootprintRadius;
        private int StructureSize(UnitTypeData structure) => (int)FootprintRadius(structure) * 2;
        private void UpdateBlockedGrid(UnitTypeData unit, Point2D position, byte value) => UpdateImageGrid(FootprintRadius(unit), position.X, position.Y, value, BlockedGrid);
        private void UpdateImageGrid(float radius, float xPosition, float yPosition, byte value, MapHandler image) {
            var x = (int)(xPosition - radius);
            var y = (int)(yPosition - radius);
            var size = radius * 2;
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    image.Set(x + i, y + j, value);
        }

        public void CreepAndVisibility(Observation obs) {
            Explored.UpdateImage(obs.RawData.MapState.Visibility);
            Creep.UpdateImage(obs.RawData.MapState.Creep);
            foreach (var ps in obs.RawData.Player.PowerSources)
                UpdatePower(ps);
        }

        //TODO: Make smarter.
        private void UpdatePower(PowerSource source) {
            var p = source.Pos.ConvertTo2D();
            var r = source.Radius;
            var startX = MathF.Round(p.X - r);
            var startY = MathF.Round(p.Y - r);
            var size = (int)(r * 2);
            var mx = (int)startX + size;
            var my = (int)startY + size;
            for (int x = (int)startX; x < mx; x++)
                for (int y = (int)startY; y < my; y++)
                    if (MathServices.EuclidianDistance(p, x, y) <= r)
                        Power.Set(x, y, 1);
        }

        private bool ValidPlacement(int xPosition, int yPosition, int size, bool requiresCreep = false, bool requiresPower = false, bool avoidCreep = true, bool avoidNatural = false) {
            var mx = xPosition + size;
            var my = yPosition + size;
            for (int x = xPosition; x < mx; x++) {
                for (int y = yPosition; y < my; y++) {
                    if (BlockedGrid.IsSet(x, y))
                        return false;
                    if (!PlacementGrid.IsSet(x, y))
                        return false;
                    if (requiresCreep && !Creep.IsSet(x, y))
                        return false;
                    if (avoidCreep && Creep.IsSet(x, y))
                        return false;
                    if (requiresPower && !Power.IsSet(x, y))
                        return false;
                    if (avoidNatural && NaturalGrid.IsSet(x, y))
                        return false;
                }
            }
            return true;
        }

        private IPosition FindPlacement(float xPosition, float yPosition, UnitTypeData structure, float spacing = 0.0f) {
            var r = FootprintRadius(structure) + spacing;
            var d = (int)(r * 2);
            AlignCoordinates(d, ref xPosition, ref yPosition);

            bool rCreep = structure.Race == Race.Zerg && structure.UnitId != BlizzardConstants.Unit.Hatchery;
            bool rPower = structure.Race == Race.Protoss && structure.UnitId != BlizzardConstants.Unit.Nexus && structure.UnitId != BlizzardConstants.Unit.Pylon;
            bool aCreep = structure.Race != Race.Zerg;
            bool aNaturals = GameConstants.IsHeadquarter(structure.UnitId);

            if (ValidPlacement((int)(xPosition - r), (int)(yPosition - r), d, rCreep, rPower, aCreep, aNaturals))
                return new Position(xPosition, yPosition);

            bool b = true;
            for (int count = 1; count < 50; count++) {
                // Move vertical
                for (int i = 0; i < count; i++) {
                    if (b) yPosition++;
                    else yPosition--;
                    if (ValidPlacement((int)(xPosition - r), (int)(yPosition - r), d, rCreep, rPower, aCreep, aNaturals))
                        return new Position(xPosition, yPosition);
                }
                // Move horizontal
                for (int i = 0; i < count; i++) {
                    if (b) xPosition--;
                    else xPosition++;
                    if (ValidPlacement((int)(xPosition - r), (int)(yPosition - r), d, rCreep, rPower, aCreep, aNaturals))
                        return new Position(xPosition, yPosition);
                }
                // Change direction
                b = !b;
            }
            return null;
        }

        private IPosition FindPlacementWithAddOn(float x, float y) {
            float r1 = 1.5f, r2 = 1.0f;
            int d1 = 3, d2 = 2;
            AlignCoordinates(d1, ref x, ref y);
            var offX = x + 2.5f - r2;
            var offY = y - 0.5 - r2;
            if (ValidPlacement((int)(x - r1), (int)(y - r1), d1) && ValidPlacement((int)(offX), (int)(offY), d2))
                return new Position(x, y);

            bool b = true;
            for (int count = 1; count < 50; count++) {
                // Move vertical
                for (int i = 0; i < count; i++) {
                    if (b) { y++; offY++; } else { y--; offY--; }
                    if (ValidPlacement((int)(x - r1), (int)(y - r1), d1) && ValidPlacement((int)(offX), (int)(offY), d2))
                        return new Position(x, y);
                }
                // Move horizontal
                for (int i = 0; i < count; i++) {
                    if (b) { x--; offX--; } else { x++; offX++; }
                    if (ValidPlacement((int)(x - r1), (int)(y - r1), d1) && ValidPlacement((int)(offX), (int)(offY), d2))
                        return new Position(x, y);
                }
                // Change direction
                b = !b;
            }
            return null;
        }

        public void RenderImageToDesktop(string filename) {
            var height = PlacementGrid.Height;
            var width = PlacementGrid.Width;
            var bm = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (BlockedGrid.IsSet(x, y))
                        bm.SetPixel(x, height - y - 1, Color.Red);
                    else if (Creep.IsSet(x, y))
                        bm.SetPixel(x, height - y - 1, Color.Purple);
                    else if (Explored.IsSet(x, y))
                        if (PlacementGrid.IsSet(x, y))
                            bm.SetPixel(x, height - y - 1, Color.LightBlue);
                        else
                            bm.SetPixel(x, height - y - 1, Color.DarkBlue);
                    else if (PlacementGrid.IsSet(x, y))
                        if (NaturalGrid.IsSet(x, y))
                            bm.SetPixel(x, height - y - 1, Color.Pink);
                        else
                            bm.SetPixel(x, height - y - 1, Color.Green);
                    else
                        bm.SetPixel(x, height - y - 1, Color.Black);
            bm.Save($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{filename}.BMP");
        }

        private void AlignCoordinates(int diameter, ref float x, ref float y) {
            if (diameter % 2 == 0) {
                if ((int)(x * 10f) % 2 == 1)
                    x += 0.5f;
                if ((int)(y * 10f) % 2 == 1)
                    y += 0.5f;
            } else {
                if ((int)(x * 10f) % 2 == 0)
                    x += 0.5f;
                if ((int)(y * 10f) % 2 == 0)
                    y += 0.5f;
            }
        }

        public void RenderRegionsToDesktop(string filename) {
            var height = NonPathable.Height;
            var width = NonPathable.Width;
            var bm = new Bitmap(width, height);
            var ran = new Random();

            foreach (var r in Regions) {
                var color = Color.FromArgb(ran.Next(256), ran.Next(256), ran.Next(256));
                foreach (var value in r.Tiles) {
                    bm.SetPixel((int)value.X, (int)value.Y, color);
                }
                for (int i = r.MinX; i <= r.MaxX; i++) {
                    for (int j = r.MinY; j <= r.MaxY; j++) {
                        if (i == r.MinX || i == r.MaxX || j == r.MinY || j == r.MaxY) {
                            bm.SetPixel(i, j, color);
                        }
                    }
                }
            }
            bm.Save($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{filename}.BMP");
        }

        private void Clear() {
            NonPathable = null;
            Graph = null;
            Regions = null;
            BlockedGrid = null;
            Creep = null;
            Explored = null;
            PlacementGrid = null;
            Power = null;
            TerrainHeight = null;
        }

        //IEnumerable<Region> IGameMap.Regions => Regions;

        IPosition IGameMap.FindPlacementWithAddOn(Point2D point) => FindPlacementWithAddOn(point.X, point.Y);
        IPosition IGameMap.FindPlacement(UnitTypeData structure, Point2D point, int spacing) => FindPlacement(point.X, point.Y, structure, spacing);
        bool IGameMap.ValidPlacement(UnitTypeData structure, Point2D point, int spacing) { //TODO: Improve to avoid code duplication
            var r = FootprintRadius(structure) + spacing;
            var d = (int)(r * 2);
            var x = point.X;
            var y = point.Y;
            AlignCoordinates(d, ref x, ref y);
            bool rCreep = structure.Race == Race.Zerg && structure.UnitId != BlizzardConstants.Unit.Hatchery;
            bool rPower = structure.Race == Race.Protoss && structure.UnitId != BlizzardConstants.Unit.Nexus && structure.UnitId != BlizzardConstants.Unit.Pylon;
            bool aCreep = structure.Race != Race.Zerg;
            return ValidPlacement((int)(x - r), (int)(y - r), d, rCreep, rPower, aCreep);
        }

        void IGameMap.Reserve(UnitTypeData structure, Point2D position) => UpdateBlockedGrid(structure, position, 1);

        // Improve and expose
        private int GetShortestGroundDistance(Point2D start, Point2D end) {
            var startTile = Graph.FirstOrDefault(t => Math.Abs(t.X - start.X) <= 1.0 && Math.Abs(t.Y - start.Y) <= 1.0);
            var endTile = Graph.FirstOrDefault(t => Math.Abs(t.X - end.X) <= 1.0 && Math.Abs(t.Y - end.Y) <= 1.0);
            if (startTile == null || endTile == null) {
                Console.WriteLine("start or end position could not be found on graph");
                return Int32.MaxValue;
            }
            return GetShortestDistanceAstar(startTile, endTile);
        }
        private int GetShortestDistanceAstar(Tile start, Tile end) {
            foreach (var tile in Graph) {
                tile.StraightLineCost = tile.HeuristicDistanceTo(end);
                tile.CostToStart = 0;
                tile.Visited = false;
                tile.ExpectedTotalCost = tile.StraightLineCost;
            }
            start.ExpectedTotalCost = start.StraightLineCost;
            return AStar(start, end);
        }
        private int AStar(Tile start, Tile end) {
            var prioqueue = new List<Tile>();
            prioqueue.Add(start);

            do {
                prioqueue = prioqueue.OrderBy(x => x.CostToStart + x.StraightLineCost).ToList();
                var next = prioqueue.First();
                prioqueue.Remove(next);
                foreach (Tile child in next.Neighbors) {
                    if (child.Visited)
                        continue;

                    child.CostToStart = child.CostToStart + next.CostToStart + 1;
                    if (!prioqueue.Contains(child)) {
                        prioqueue.Add(child);
                    }
                    if (next == end) {
                        return next.CostToStart + 1;
                    }

                }
                next.ExpectedTotalCost = next.CostToStart + next.StraightLineCost;
                next.Visited = true;

            } while (prioqueue.Any());
            if (prioqueue.Count <= 0) {
                Console.WriteLine("no path available");
            }
            return Int32.MaxValue;
        }
    }
}
