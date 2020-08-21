using Abathur.Constants;
using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Model;
using Abathur.Repositories;
using NydusNetwork.API.Protocol;
using System.Linq;

namespace Abathur.Modules
{
    class CrazySupply : IModule {
        private IIntelManager intelManager;
        private IProductionManager productionManager;
        private ICombatManager combatManager;
        private ISquadRepository rep;
        private Squad depots;
        public CrazySupply(IIntelManager intelManager, IProductionManager productionManager, ICombatManager combatManager, ISquadRepository squadRepository) {
            this.intelManager = intelManager;
            this.productionManager = productionManager;
            this.combatManager = combatManager;
            rep = squadRepository;
        }
        void IModule.OnStep() {
            if(intelManager.ProductionQueue.Where(u => u.UnitId == GameConstants.RaceSupply).Count() > 3)
                return;
            productionManager.QueueUnit(GameConstants.RaceSupply, null, 0);
            combatManager.UseTargetlessAbility(BlizzardConstants.Ability.SupplyDepotLower,depots);
        }

        private void Handle(IUnit unit) {
            if(unit.UnitType == BlizzardConstants.Unit.SupplyDepot)
                depots.AddUnit(unit);
        }
        void IModule.Initialize() { }
        void IModule.OnStart() {
            depots = rep.Create("MASTER DEPOTS <3");
            intelManager.Handler.RegisterHandler(Case.StructureAddedSelf,u => Handle(u));
        }
        void IModule.OnGameEnded() { }
        void IModule.OnRestart() { }
    }
}
