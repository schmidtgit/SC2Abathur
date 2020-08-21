using System.Collections.Generic;

namespace Abathur.Model {
    public interface IColony : IPosition {
        uint Id                      { get; }
        bool IsStartingLocation      { get; }
        IEnumerable<IUnit> Minerals  { get; }
        IEnumerable<IUnit> Vespene   { get; }
        ICollection<IUnit> Structures{ get; }
        ICollection<IUnit> Workers   { get; }
        int DesiredVespeneWorkers    { get; set; }
    }
}
