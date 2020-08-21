using Abathur.Constants;
using Abathur.Core;
using Abathur.Extensions;
using Abathur.Model;
using NydusNetwork.API.Protocol;
using System.Collections.Generic;
using System.Linq;

namespace Abathur.Modules
{
    class AutoHarvestGather : IModule {
        private IIntelManager intelManager;
        private IRawManager rawManager;
        public AutoHarvestGather(IIntelManager intelManager, IRawManager rawManager) {
            this.intelManager = intelManager;
            this.rawManager = rawManager;
        }

        private void ColonyCheck() {
            foreach(var colony in intelManager.Colonies) {
                if(colony.Workers.Count == 0)
                    continue;
                if(intelManager.Common.IdleWorkerCount != 0)
                    IdleHarvestMinerals(colony);
                DesiredVespeneWorkerCount(colony);
            }

        }

        private void IdleHarvestMinerals(IColony colony) {
            var workers = colony.Workers.Where(w => w.Orders.Count == 0 && w.Alliance == Alliance.Self);
            if(workers.Any())
                AssignWorkersToClosestMineral(colony, workers);
        }

        private void DesiredVespeneWorkerCount(IColony colony) {
            var refineries = colony.Structures.Where(u => GameConstants.IsVespeneGeyserBuilding(u.UnitType) && u.Alliance == Alliance.Self);
            var ideal = System.Math.Min(refineries.Sum(r => r.IdealHarvesters), colony.DesiredVespeneWorkers);
            var current = refineries.Sum(r => r.AssignedHarvesters);
            if(current == ideal) return;
            if(current > ideal) {
                var workers = GetVespeneWorkers(colony,current - ideal,refineries.Select(r => r.Tag));
                AssignWorkersToClosestMineral(colony,workers);
            } else {
                var workers = GetMineralWorkers(colony,ideal - current);
                while(workers.Count != 0) {
                    var target = refineries.FirstOrDefault(v => v.IdealHarvesters > v.AssignedHarvesters);
                    HarvestGather(target.Tag,workers.Dequeue());
                    target.AssignedHarvesters++;
                }
            }
        }

        private void AssignWorkersToClosestMineral(IColony colony, IEnumerable<IUnit> units) {
            foreach(var w in units) {
                var mineral = w.GetClosest(colony.Minerals);
                HarvestGather(mineral.Tag,w);
            }
        }

        private Queue<IUnit> GetMineralWorkers(IColony colony,int count) {
            var minerals = colony.Minerals.Select(m => m.Tag);
            return new Queue<IUnit>(colony.Workers.Where(u => u.Orders.Count == 0 || minerals.Contains(u.Orders.First().TargetUnitTag)).Take(count));
        }
        private Queue<IUnit> GetVespeneWorkers(IColony colony,int count, IEnumerable<ulong> refineries) => new Queue<IUnit>(colony.Workers.Where(u => u.Orders.Count == 0 || refineries.Contains(u.Orders.First().TargetUnitTag)).Take(count));

        private void HarvestGather(ulong target,params IUnit[] workers)
            => rawManager.QueueActions(
                new Action {
                    ActionRaw = new ActionRaw {
                        UnitCommand = new ActionRawUnitCommand {
                            AbilityId = GameConstants.RaceHarvestGatherAbility,
                            TargetUnitTag = target,
                            UnitTags = { workers.Select(w => w.Tag).ToList()
                        }
                        }
                    }
                });

        void IModule.Initialize() { }
        void IModule.OnStart() {}
        void IModule.OnStep() => ColonyCheck();
        void IModule.OnGameEnded() { }
        void IModule.OnRestart() { }
    }
}
