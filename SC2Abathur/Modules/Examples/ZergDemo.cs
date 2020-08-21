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
    // ZergDemo inherits IReplaceableModule and is therefore accessible in IoC container and can be swapped at runtime.
    public class ZergDemo : IReplaceableModule {
        private IEnumerable<IColony> _eStarts;
        private bool _done;
        private readonly IIntelManager _intelManager;
        private readonly ICombatManager _combatManager;
        private readonly IProductionManager _productionManager;
        private ISquadRepository _squadRep;
        private Squad _theGang;
        private bool _startCalled;
        private GameSettings _gameSettings;

        // Take required managers in the constructor, see FullModule for all possible managers.
        public ZergDemo(IIntelManager intelManager,ICombatManager combatManager,IProductionManager productionManager,ISquadRepository squadRepo,GameSettings gamsettings) {
            _intelManager = intelManager;
            _combatManager = combatManager;
            _productionManager = productionManager;
            _squadRep = squadRepo;
            _gameSettings = gamsettings;
        }

        public void Initialize() { }

        public void OnStart() {
            if(_startCalled)
                return;
            _productionManager.ClearBuildOrder();

            // Colonies marked with Starting location are possible starting locations of the enemy NOT yourself
            _eStarts = _intelManager.Colonies.Where(c => c.IsStartingLocation);

            // Queue a hardcoded all-in
            QueueOneBaseRavager();

            // Using the AutoHarvestGather module DesiredVespeneWorkers decides how many workers will be assigned to gather vespene 
            // Without AutoHarvestGather in the setup.json file (or and alternative) this has no effect
            _intelManager.PrimaryColony.DesiredVespeneWorkers = 4;

            // Creating a squad allows for treating a group of units as one.
            _theGang = _squadRep.Create("TheGang"); // The name allows other modules to look it up by name! (as IDs are generated at runtime)

            // The _intelManager.Handler allows for actions to be carried out when specific events happens.
            // Registering a handler to a Case means the handler will be called every time the event happens.
            _intelManager.Handler.RegisterHandler(Case.UnitAddedSelf,UnitMadeHandler);
            _intelManager.Handler.RegisterHandler(Case.StructureAddedSelf,s => {
                if(s.UnitType == BlizzardConstants.Unit.RoachWarren)
                    _intelManager.PrimaryColony.DesiredVespeneWorkers = 6; // Increase vespene workers after the roach warren is added!
            });

            // A variable needed since ZergDemo is a replaceable module relying on its on OnStart, but dont want it to be called twice
            _startCalled = true;
            _done = false;
        }

        public void OnStep() {
            if(_intelManager.GameLoop % 250 == 0 && _done)
                KillEverything();
            if(_intelManager.ProductionQueue.Any())
                return;
            if(_done)
                return;

            foreach(var colony in _eStarts)
                _combatManager.AttackMove(_theGang,colony.Point,true);
            _done = true;
            QueueThemUp();
        }

        public void OnGameEnded() {}

        public void OnRestart() {
            _startCalled = false;
            _done = false;
        }

        public void OnAdded() {
            OnStart();
            foreach(var crook in _intelManager.UnitsSelf()) {
                if(crook.UnitType != BlizzardConstants.Unit.Overlord) {
                    _theGang.AddUnit(crook);
                }
            }
        }

        public void OnRemoved() {
            _startCalled = false;
            _done = false;
        }

        // Called by IntelManager, adds everything - expect overloads - to the Squad
        public void UnitMadeHandler(IUnit u) {
            // UnitType is a stable id of the Type of the unit. BlizzardConstants contains static variables to make it easy to find the specific id you need.
            if(u.UnitType != BlizzardConstants.Unit.Overlord)
                _theGang.AddUnit(u);
        }

        private void KillEverything() {
            var target = _intelManager.StructuresEnemyVisible.FirstOrDefault();
            if(target == null) {
                foreach(var colony in _eStarts)
                    _combatManager.AttackMove(_theGang,colony.Point,true);
                foreach(var colony in _intelManager.Colonies)
                    if(_theGang.Units.Any(u => u.Orders.Count == 0))
                        _combatManager.AttackMove(_theGang,colony.Point,true);
                return;
            }
            _combatManager.AttackMove(_theGang,target.Point);
        }

        private void QueueThemUp() {
            for(int i = 0; i < 100; i++) {
                _productionManager.QueueUnit(BlizzardConstants.Unit.Roach);
            }
        }

        // This is an example of a complete hardcoded build order.
        // Simply queue the units you want built, in the order you want them, using the production manager.
        // The production manager will automaticly sort out invalid build orders, and write a warning to log.
        public void QueueOneBaseRavager() {
            // QueueUnit takes the id of the unit-type you want to produce. These ids are easily accessible through BlizzardConstants
            _productionManager.QueueUnit(BlizzardConstants.Unit.Drone);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Drone);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SpawningPool);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Drone);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Extractor);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Drone);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Overlord);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Extractor);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Drone);
            _productionManager.QueueUnit(BlizzardConstants.Unit.RoachWarren);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Drone);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Drone);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Zergling);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Overlord);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Roach);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Roach);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Roach);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Roach);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Ravager);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Ravager);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Roach);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Ravager);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Overlord);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Roach);
        }
    }
}