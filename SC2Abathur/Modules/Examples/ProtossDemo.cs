using System.Collections.Generic;
using System.Linq;
using Abathur.Constants;
using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Model;
using Abathur.Modules;
using Abathur.Repositories;

namespace SC2Abathur.Modules.Examples {
    /// <summary>
    /// ProtossDemo inherits from IReplaceableModule and is therefore accessible in IoC container and can be swapped at runtime.
    /// It will also automaticly be included in Abathur if mentioned in setup.json
    /// </summary>
    public class ProtossDemo : IReplaceableModule {
        // Collection of 'colonies' (bases)
        private IEnumerable<IColony> _eStarts;
        // Abathur provided 'managers'
        private readonly IIntelManager _intelManager;
        private readonly ICombatManager _combatManager;
        private readonly IProductionManager _productionManager;
        // Squads are collection of units (for easy unit management)
        private ISquadRepository _squadRep;
        private Squad _theGang;
        // Primitive state trackers used for this demo.
        private bool _immortalCreated;
        private bool _startCalled;
        private bool _attackMode;


        /// <summary>
        /// All these varibles will be populated by Abathur
        /// </summary>
        public ProtossDemo(IIntelManager intelManager,ICombatManager combatManager,IProductionManager productionManager,ISquadRepository squadRepo) {
            _intelManager = intelManager;
            _combatManager = combatManager;
            _productionManager = productionManager;
            _squadRep = squadRepo;
        }

        /// <summary>
        /// Required to satisfy interface, but not used in demo...
        /// </summary>
        public void Initialize() { }

        /// <summary>
        /// Called on the first frame of the game.
        /// Queue our gameplan!
        /// </summary>
        public void OnStart() {
            if (_startCalled) return;
            // Find start locations (never includes our own base)
            _eStarts = _intelManager.Colonies.Where(c => c.IsStartingLocation);
            // Queue build order
            QueueImmortalRush();
            // Register a callback function for new units (not workers) added.
            _intelManager.Handler.RegisterHandler(Case.UnitAddedSelf,UnitCreationHandler);
            // Create a 'squad', a collection of units.
            _theGang = _squadRep.Create("TheGang");
            _startCalled = true;
        }

        /// <summary>
        /// Called every frame of the game, except the first.
        /// Attack everything if we are ready.
        /// </summary>
        public void OnStep() {
            // Attack if enough units
            if(_theGang.Units.Count>=10 && !_attackMode) {
                foreach (var colony in _eStarts)
                    _combatManager.AttackMove(_theGang,colony.Point, true);
                _attackMode = true;
            }

            if (_intelManager.GameLoop % 250 == 0 && _attackMode)
                AttackEverything();

            // Queue new units when the production queue is empty.
            if (!_intelManager.ProductionQueue.Any()) {
                _productionManager.QueueUnit(BlizzardConstants.Unit.Immortal);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Stalker);
            }
        }

        /// <summary>
        /// Attack everything visible!
        /// </summary>
        private void AttackEverything()
        {
            // List of all visible enemy buildings...
            var target = _intelManager.StructuresEnemyVisible.FirstOrDefault();
            if (target == null)
            {
                foreach (var colony in _eStarts)
                    _combatManager.AttackMove(_theGang, colony.Point, true);
                foreach (var colony in _intelManager.Colonies)
                    if (_theGang.Units.Any(u => u.Orders.Count == 0))
                        _combatManager.AttackMove(_theGang, colony.Point, true);
                return;
            }
            _combatManager.AttackMove(_theGang, target.Point);
        }

        /// <summary>
        /// Called by Abathur (registred in OnStart)
        /// Added new attack units to our squad. Not called for workers.
        /// </summary>
        public void UnitCreationHandler(IUnit u)
        {
            // Attack everything as soon as we get the first Immortal!
            if (u.UnitType == BlizzardConstants.Unit.Immortal)
            {
                _theGang.AddUnit(u);
                foreach(var colony in _eStarts) 
                    _combatManager.AttackMove(_theGang,colony.Point,true);
                _immortalCreated = true;
            } else {
                _theGang.AddUnit(u);
                if (_immortalCreated)
                    foreach(var colony in _eStarts) 
                        _combatManager.AttackMove(_theGang,colony.Point,true);
            }
        }

        /// <summary>
        /// Required to satisfy interface. Restart state trackers.
        /// </summary>
        public void OnGameEnded() {
            _startCalled = false;
            _immortalCreated = false;
            _attackMode = false;
        }

        /// <summary>
        /// Required to satisfy interface.
        /// </summary>
        public void OnRestart() => OnGameEnded();

        /// <summary>
        /// If added by another module mid-game (see RandomDemo)
        /// </summary>
        public void OnAdded() => OnStart();

        /// <summary>
        /// If removed mid-game.
        /// </summary>
        public void OnRemoved() => OnGameEnded();

        /// <summary>
        /// Hardcoded build-order...
        /// </summary>
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
