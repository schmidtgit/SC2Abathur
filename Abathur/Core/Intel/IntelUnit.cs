using System.Collections.Generic;
using Abathur.Constants;
using Abathur.Extensions;
using Abathur.Model;
using NydusNetwork.API.Protocol;

namespace Abathur.Core.Intel
{
    internal class IntelUnit : IUnit {
        public uint lastSeen;
        private Unit data;
        public uint FramesSinceSeen => GameConstants.GameLoop - lastSeen;
        public Unit DataSource {
            get { return data; }
            set { lastSeen = GameConstants.GameLoop;
                data = value; } }
        public IntelUnit(Unit unit) { data = unit; }

        public ulong AddOnTag => data.AddOnTag;

        public Alliance Alliance => data.Alliance;

        public int AssignedHarvesters { get => data.AssignedHarvesters; set => data.AssignedHarvesters = value; }

        public ICollection<uint> BuffIds => data.BuffIds;

        public float BuildProgress => data.BuildProgress;

        public int CargoSpaceMax => data.CargoSpaceMax;

        public int CargoSpaceTaken => data.CargoSpaceTaken;

        public CloakState Cloak => data.Cloak;

        public float DetectRange => data.DetectRange;

        public DisplayType DisplayType => FramesSinceSeen == 0 ? data.DisplayType : DisplayType.Snapshot;

        public float Energy => data.Energy;

        public ulong EngagedTargetTag => data.EngagedTargetTag;

        public float Facing => data.Facing;

        public float Health => data.Health;

        public float HealthMax => data.HealthMax;

        public int IdealHarvesters => data.IdealHarvesters;

        public bool IsBlip => data.IsBlip;

        public bool IsBurrowed => data.IsBurrowed;

        public bool IsFlying => data.IsFlying;

        public bool IsOnScreen => data.IsOnScreen;

        public bool IsPowered => data.IsPowered;

        public bool IsSelected => data.IsSelected;

        public int MineralContents => data.MineralContents;

        public ICollection<UnitOrder> Orders => data.Orders;

        public int Owner => data.Owner;
        public ICollection<PassengerUnit> Passengers => data.Passengers;

        public Point Pos => data.Pos;

        public float RadarRange => data.RadarRange;

        public float Radius => data.Radius;

        public float Shield => data.Shield;

        public ulong Tag => data.Tag;

        public uint UnitType => data.UnitType;

        public int VespeneContents => data.VespeneContents;

        public float WeaponCooldown => data.WeaponCooldown;

        public Point2D Point => Pos.ConvertTo2D();

        public override bool Equals(object obj) {
            var unit = obj as IUnit;
            if(unit == null) return false;
            return unit.Tag == this.Tag;
        }
        public override int GetHashCode() => (int)Tag;
    }
}
