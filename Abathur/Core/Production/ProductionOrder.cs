using Abathur.Constants;
using Abathur.Model;
using NydusNetwork.API.Protocol;

namespace Abathur.Core.Production
{
    internal class ProductionOrder : IPosition {
        private UnitTypeData _unit;
        public UnitTypeData Unit {
            get { return _unit; }
            set {
                _unit = value;
                if(GameConstants.IsAddon(Unit.UnitId))
                    Type = BuildType.AddOn;
                else if(GameConstants.IsMorphed(Unit.UnitId))
                    Type = BuildType.Morphed;
                else if(Unit.Attributes.Contains(Attribute.Structure))
                    Type = BuildType.Structure;
                else
                    Type = BuildType.Unit;

                if(GameConstants.RequiresBarrackTechlab(_unit.UnitId))
                    RequiredAddOn = BlizzardConstants.Unit.BarracksTechLab;
                else if(GameConstants.RequiresFactoryTechlab(_unit.UnitId))
                    RequiredAddOn = BlizzardConstants.Unit.FactoryTechLab;
                else if(GameConstants.RequiresStarportTechLab(_unit.UnitId))
                    RequiredAddOn = BlizzardConstants.Unit.StarportTechLab;
            }
        }
        private UpgradeData _research;
        public UpgradeData Research {
            get { return _research; }
            set {
                _research = value;
                Type = BuildType.Research;
                if(_unit != null) throw new System.ArgumentException("Production Order must be either an Unit or Research");
            }
        }
        public Point2D Position { get; set; }
        public int Spacing { get; set; } = 1;
        public bool LowPriority { get; set; }
        public IUnit OrderedUnit { get; set; }
        public IUnit AssignedUnit { get; set; }
        public OrderStatus Status { get; set; }
        public BuildType Type { get; private set; }
        public uint RequiredAddOn { get; private set; }
        public enum BuildType { Unknown, Unit, Structure, Morphed, AddOn, Research }
        public enum OrderStatus { Queued, Commissioned, Producing, Built }
        Point2D IPosition.Point => Position;
    }
    
}
