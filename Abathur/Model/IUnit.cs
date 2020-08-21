using NydusNetwork.API.Protocol;
using System.Collections.Generic;

namespace Abathur.Model {
    public interface IUnit : IPosition {
        uint FramesSinceSeen { get; }
        ulong AddOnTag { get; }
        Alliance Alliance { get; }
        int AssignedHarvesters { get; set; }
        ICollection<uint> BuffIds { get; }
        float BuildProgress { get; }
        int CargoSpaceMax { get; }
        int CargoSpaceTaken { get; }
        CloakState Cloak { get; }
        float DetectRange { get; }
        DisplayType DisplayType { get; }
        float Energy { get; }
        ulong EngagedTargetTag { get; }
        float Facing { get; }
        float Health { get; }
        float HealthMax { get; }
        int IdealHarvesters { get; }
        bool IsBlip { get; }
        bool IsBurrowed { get; }
        bool IsFlying { get; }
        bool IsOnScreen { get; }
        bool IsPowered { get; }
        bool IsSelected { get; }
        int MineralContents { get; }
        ICollection<UnitOrder> Orders { get; }
        int Owner { get; }
        ICollection<PassengerUnit> Passengers { get; }
        Point Pos { get; }
        float RadarRange { get; }
        float Radius { get; }
        float Shield { get; }
        ulong Tag { get; }
        uint UnitType { get; }
        int VespeneContents { get; }
        float WeaponCooldown { get; }
    }
}
