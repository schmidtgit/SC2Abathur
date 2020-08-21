using System.Collections.Generic;
using System.Linq;
using Abathur.Constants;
using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Model;
using Abathur.Modules;
using Abathur.Repositories;
using NydusNetwork.Model;

namespace SC2Abathur.Modules.Examples {
    // This demo runs best with the "AutoHarvestGather" module in the setup.json file!
    // TerranDemo inherits IReplaceableModule and is therefore accessible in IoC container and can be swapped at runtime.
    public class TerranDemo : IReplaceableModule {
        private IEnumerable<IColony> _eStarts;
        private readonly IIntelManager _intelManager;
        private readonly ICombatManager _combatManager;
        private readonly IProductionManager _productionManager;
        private ISquadRepository _squadRep;
        private Squad gang; // Dead units are automaticly removed.
        private bool _startCalled;
        private bool _done;

        private GameSettings _gameSettings;

        // Take required managers in the constructor, see FullModule for all possible managers.
        public TerranDemo(IIntelManager intelManager,ICombatManager combatManager,IProductionManager productionManager,ISquadRepository squadRepo,GameSettings gamesettings) {
            _intelManager = intelManager;
            _combatManager = combatManager;
            _productionManager = productionManager;
            _squadRep = squadRepo;
            _gameSettings = gamesettings;
        }

        public void Initialize() {}

        public void OnStart() {
            if(_startCalled)
                return;
            _productionManager.ClearBuildOrder();

            // Colonies marked with starting location are possible starting locations of the enemy, never yourself
            _eStarts = _intelManager.Colonies.Where(c => c.IsStartingLocation);
            _intelManager.Handler.RegisterHandler(Case.UnitAddedSelf,addCrook);

            QueueRaxRush();

            gang = _squadRep.Create("TheGang");
            _done = false;
            _startCalled = true;
        }

        public void OnStep() {
            if(_intelManager.GameLoop % 250 == 0 && _done)
                KillEverything();
            if(_intelManager.ProductionQueue.Any())
                return;
            if(_done)
                return;
            
            foreach(var colony in _eStarts)
                _combatManager.AttackMove(gang,colony.Point,true);
            _done = true;
            QueueThemUp();
        }

        private void QueueThemUp() {
            for(int i = 0; i < 100; i++) {
                _productionManager.QueueUnit(BlizzardConstants.Unit.Marauder);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Marine);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Marine);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Marine);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Marine);
            }
        }

        /// Find a visible enemy structure and attack it.
        private void KillEverything() {
            var target = _intelManager.StructuresEnemyVisible.FirstOrDefault();
            if(target == null) {
                foreach(var colony in _eStarts)
                    _combatManager.AttackMove(gang,colony.Point,true);
                foreach(var colony in _intelManager.Colonies)
                    if(gang.Units.Any(u => u.Orders.Count == 0))
                        _combatManager.AttackMove(gang,colony.Point,true);
                return;
            }
            _combatManager.AttackMove(gang,target.Point);
        }

        public void OnGameEnded() {}

        public void OnRestart() {
            _done = false;
            _startCalled = false;
        }

        public void OnAdded() {
            if(!_startCalled)
                OnStart();
        }

        public void OnRemoved() {
            _done = false;
            _startCalled = false;
        }

        public void addCrook(IUnit u) {
            gang.AddUnit(u);
        }

        /// Want to train yourself against a specific build order? Hardcode it and play against it.
        public void QueueRaxRush() {
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SupplyDepot,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Refinery,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Barracks,lowPriority: false,spacing: 3);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SupplyDepot,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Barracks,lowPriority: false,spacing: 3);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.BarracksTechLab,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Barracks,lowPriority: false,spacing: 3);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV,lowPriority: false);
            _productionManager.QueueTech(BlizzardConstants.Research.CombatShield,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marauder,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.BarracksReactor,lowPriority: false);
            //_productionManager.QueueUnit(BlizzardConstants.Unit.OrbitalCommand,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.BarracksReactor,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SupplyDepot,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SupplyDepot,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marauder,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Refinery,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marauder,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine,lowPriority: false);
            _productionManager.QueueTech(BlizzardConstants.Research.ConcussiveShells,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marauder,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV,lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marauder,lowPriority: false);
        }
    }
}