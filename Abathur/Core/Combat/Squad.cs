using System;
using System.Collections.Generic;
using System.Linq;
using Abathur.Model;
using NydusNetwork.API.Protocol;

namespace Abathur.Core.Combat
{
    public class Squad : IPosition {
        public ulong Id { get; set; }
        public String Name { get; set; }
        public ISet<IUnit> Units { get; set; } = new HashSet<IUnit>();
        public ISquadController SquadController { get; set; }

        Point2D IPosition.Point { get {
                var count = Units.Count;
                if (count == 0)
                    return new Point2D { X = float.NaN, Y = float.NaN };
                var p = Units.Aggregate((0.0f, 0.0f), (sum, unit) => (sum.Item1 + unit.Point.X, sum.Item2 + unit.Point.Y));
                return new Point2D { X = p.Item1 / count, Y = p.Item2 / count};
            }
        }

        public bool AddUnit(IUnit unit)
        {
            return Units.Add(unit);
        }

        public int AddUnits(IEnumerable<IUnit> units)
        {
            var added = 0;
            foreach (var unit in units)
            {
                if (Units.Add(unit))
                {
                    added++;
                }
            }
            return added;
        }

        public bool RemoveUnit(IUnit unit)
        {
            return Units.Remove(unit);
        }

        public int RemoveUnits(IEnumerable<IUnit> units)
        {
            var removed = 0;
            foreach (var unit in units)
            {
                if (Units.Remove(unit))
                {
                    removed++;
                }
            }
            return removed;
        }

        public override string ToString()
        {
            var s = Name + ":\n";
            foreach (var unit in Units)
            {
                s = s + unit + "\n";
            }
            return s;
        }
    }
}
