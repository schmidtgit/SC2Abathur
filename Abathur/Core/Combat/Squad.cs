using System;
using System.Collections.Generic;
using Abathur.Model;

namespace Abathur.Core.Combat
{
    public class Squad {
        public ulong Id { get; set; }
        public String Name { get; set; }
        public ISet<IUnit> Units { get; set; } = new HashSet<IUnit>();
        public ISquadController SquadController { get; set; }
        
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
