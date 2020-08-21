using Abathur.Model;
using NydusNetwork.API.Protocol;
using System.Collections.Generic;

namespace Abathur.Core.Intel {
    public class IntelColony : IColony {
        public uint Id                  { get; set; }
        public Point2D Point            { get; set; }
        public bool IsStartingLocation  { get; set; }
        public List<IUnit> Minerals     { get; set; } = new List<IUnit>();
        public List<IUnit> Vespene      { get; set; } = new List<IUnit>();
        public List<IUnit> Structures   { get; set; } = new List<IUnit>();
        public List<IUnit> Workers      { get; set; } = new List<IUnit>();
        public int DesiredVespeneWorkers{ get; set; }
        IEnumerable<IUnit> IColony.Minerals     => Minerals;
        IEnumerable<IUnit> IColony.Vespene      => Vespene;
        ICollection<IUnit> IColony.Structures   => Structures;
        ICollection<IUnit> IColony.Workers      => Workers;
    }
}
