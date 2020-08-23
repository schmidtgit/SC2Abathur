using System.Collections.Generic;
using System.Linq;
using Abathur.Constants;
using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Model;
using Abathur.Modules;
using Abathur.Repositories;

namespace SC2Abathur.Modules.Examples
{
    /// <summary>
    /// TerranDemo inherits from IReplaceableModule and is therefore accessible in IoC container and can be swapped at runtime.
    /// It will also automaticly be included in Abathur if mentioned in setup.json
    /// </summary>
    public class TerranDemo : IReplaceableModule
    {
        // Collection of 'colonies' (bases)
        private IEnumerable<IColony> _eStarts;
        // Abathur provided 'managers'
        private readonly IIntelManager _intelManager;
        private readonly ICombatManager _combatManager;
        private readonly IProductionManager _productionManager;
        // Squads are collection of units (for easy unit management)
        private ISquadRepository _squadRep;
        private Squad gang;
        // Primitive state trackers used for this demo.
        private bool _startCalled;
        private bool _attackMode;


        /// <summary>
        /// Abathur will populate all these parameters...
        /// </summary>
        public TerranDemo(IIntelManager intelManager,
            ICombatManager combatManager,
            IProductionManager productionManager,
            ISquadRepository squadRepo)
        {
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
        public void OnStart()
        {
            if (_startCalled) return;
            _productionManager.ClearBuildOrder();

            // Colonies marked with starting location are possible starting locations of the enemy, never yourself
            _eStarts = _intelManager.Colonies.Where(c => c.IsStartingLocation);
            // Register callback function with Abathur -- this will call addCrook everytime we build a unit.
            _intelManager.Handler.RegisterHandler(Case.UnitAddedSelf, UnitCreationHandler);

            // Queue our build orders.
            QueueRaxRush();

            // Create a 'squad' (collection of units)
            gang = _squadRep.Create("TheGang");

            // Set primitive state trackers...
            _attackMode = false;
            _startCalled = true;
        }

        /// <summary>
        /// Called every frame of the game, except the first.
        /// Attack everything if we are ready.
        /// </summary>
        public void OnStep()
        {
            // Queue new units when the production queue is empty - and start attacking.
            if (!_intelManager.ProductionQueue.Any())
            {
                _productionManager.QueueUnit(BlizzardConstants.Unit.Marauder);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Marine);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Marine);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Marine);
                _productionManager.QueueUnit(BlizzardConstants.Unit.Marine);
                _attackMode = true;
                foreach (var colony in _eStarts)
                    _combatManager.AttackMove(gang, colony.Point, true);
            }

            // Once in a while attack everything you see.
            if (_intelManager.GameLoop % 250 == 0 && _attackMode)
                AttackEverything();
        }

        /// <summary>
        /// Attack everything visible!
        /// </summary>
        private void AttackEverything()
        {
            var target = _intelManager.StructuresEnemyVisible.FirstOrDefault();
            if (target == null)
            {
                foreach (var colony in _eStarts)
                    _combatManager.AttackMove(gang, colony.Point, true);
                foreach (var colony in _intelManager.Colonies)
                    if (gang.Units.Any(u => u.Orders.Count == 0))
                        _combatManager.AttackMove(gang, colony.Point, true);
                return;
            }
            _combatManager.AttackMove(gang, target.Point);
        }

        /// <summary>
        /// Required to satisfy interface. Restart state trackers.
        /// </summary>
        public void OnGameEnded()
            => _startCalled = false;

        /// <summary>
        /// Required to satisfy interface.
        /// </summary>
        public void OnRestart() { }

        /// <summary>
        /// If added by another module mid-game (see RandomDemo)
        /// </summary>
        public void OnAdded() => OnStart();

        /// <summary>
        /// If removed mid-game.
        /// </summary>
        public void OnRemoved() => OnGameEnded();

        /// <summary>
        /// Called by Abathur (registred in OnStart)
        /// Added new attack units to our squad. Not called for workers.
        /// </summary>
        public void UnitCreationHandler(IUnit u)
            => gang.AddUnit(u);

        /// <summary>
        /// Hardcoded build-order...
        /// </summary>
        public void QueueRaxRush()
        {
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SupplyDepot, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Refinery, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Barracks, lowPriority: false, spacing: 3);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SupplyDepot, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Barracks, lowPriority: false, spacing: 3);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.BarracksTechLab, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Barracks, lowPriority: false, spacing: 3);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV, lowPriority: false);
            _productionManager.QueueTech(BlizzardConstants.Research.CombatShield, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marauder, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.BarracksReactor, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.BarracksReactor, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SupplyDepot, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SupplyDepot, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marauder, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Refinery, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marauder, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine, lowPriority: false);
            _productionManager.QueueTech(BlizzardConstants.Research.ConcussiveShells, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marine, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marauder, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.SCV, lowPriority: false);
            _productionManager.QueueUnit(BlizzardConstants.Unit.Marauder, lowPriority: false);
        }
    }
}