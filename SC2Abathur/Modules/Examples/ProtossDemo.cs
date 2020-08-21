using System.Collections.Generic;
using System.Linq;
using Abathur.Constants;
using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Model;
using Abathur.Modules;
using Abathur.Repositories;

namespace SC2Abathur.Modules.Examples {
    // This demo runs best with the "AutoHarvestGather" module in the setup.json file!
    // ProtossDemo inherits IReplaceableModule and is therefore accessible in IoC container and can be swapped at runtime.
    public class ProtossDemo : IReplaceableModule {
        private IEnumerable<IColony> _eStarts;
        private bool _done;
        private readonly IIntelManager _intelManager;
        private readonly ICombatManager _combatManager;
        private readonly IProductionManager _productionManager;
        private ISquadRepository _squadRep;
        private Squad theGang;
        private bool _immortalCreated;
        private bool _startCalled;

        public ProtossDemo(IIntelManager intelManager,ICombatManager combatManager,IProductionManager productionManager,ISquadRepository squadRepo) {
            _intelManager = intelManager;
            _combatManager = combatManager;
            _productionManager = productionManager;
            _squadRep = squadRepo;
        }
        public void Initialize() { }

        public void OnStart() {
            if (_startCalled) return;
            _eStarts = _intelManager.Colonies.Where(c => c.IsStartingLocation);
            QueueImmortalRush();
            _intelManager.Handler.RegisterHandler(Case.UnitAddedSelf,HandleUnitMade);
            theGang = _squadRep.Create("TheGang");
            _startCalled = true;
        }

        public void OnStep() {
            if(theGang.Units.Count>=10 && !_done) {
                foreach (var colony in _eStarts)
                {
                    _combatManager.AttackMove(theGang,colony.Point, true);
                }
                _done = true;
                _intelManager.Handler.RegisterHandler(Case.UnitAddedSelf, HandleUnitMade);
            }

            if (theGang.Units.Count < 10 && _done)
            {
                _intelManager.Handler.DeregisterHandler(HandleUnitMade);
                _done = false;
            }

            if(!_intelManager.ProductionQueue.Any()) {
                _productionManager.QueueUnit(BlizzardConstants.Unit.Immortal);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker);
            }
        }

        public void HandleUnitMade(IUnit u)
        {
            if (u.UnitType == BlizzardConstants.Unit.Immortal)
            {
                theGang.AddUnit(u);
                foreach(var colony in _eStarts) {
                    _combatManager.AttackMove(theGang,colony.Point,true);
                }
                _immortalCreated = true;
            }
            else
            {
                theGang.AddUnit(u);
                if (_immortalCreated)
                {
                    foreach(var colony in _eStarts) {
                        _combatManager.AttackMove(theGang,colony.Point,true);
                    }
                }
            }
        }
        public void OnGameEnded() {}

        public void OnRestart() {
            _startCalled = false;
            _immortalCreated = false;
            _done = false;
        }

        public void OnAdded() {
            OnStart();
        }

        public void OnRemoved() {
            _intelManager.Handler.DeregisterHandler(HandleUnitMade);
            _startCalled = false;
            _immortalCreated = false;
            _done = false;
        }

        public void QueueImmortalRush()
        {
            _productionManager.QueueUnit(BlizzardConstants.Unit.Probe,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Probe,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Pylon,spacing: 3,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Probe,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Probe,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Gateway,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Assimilator,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Probe,spacing:3,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Pylon, spacing:3,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Probe,spacing: 3,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.CyberneticsCore,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Zealot, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Assimilator, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Gateway,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.RoboticsFacility,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Pylon,lowPriority: false,spacing: 3);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Sentry,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Gateway,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Gateway,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Pylon,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Immortal,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Pylon,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Immortal,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker,lowPriority: false);
        }
    }
}
