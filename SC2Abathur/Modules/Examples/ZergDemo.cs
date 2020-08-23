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
    /// ZergDemo inherits from IReplaceableModule and is therefore accessible in IoC container and can be swapped at runtime.
    /// It will also automaticly be included in Abathur if mentioned in setup.json
    /// </summary>
    public class ZergDemo : IReplaceableModule
    {
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
        private bool _attackMode;
        private bool _startCalled;

        /// <summary>
        /// All these varibles will be populated by Abathur
        /// </summary>
        public ZergDemo(IIntelManager intelManager, ICombatManager combatManager, IProductionManager productionManager, ISquadRepository squadRepo)
        {
            _intelManager = intelManager;
            _combatManager = combatManager;
            _productionManager = productionManager;
            _squadRep = squadRepo;
        }

        /// <summary>
        /// Do nothing - must implement to satisfy interface.
        /// </summary>
        public void Initialize() { }

        /// <summary>
        /// Called on the first frame of every game.
        /// </summary>
        public void OnStart()
        {
            if (_startCalled)
                return;

            // Colonies marked with Starting location are possible starting locations of the enemy NOT yourself
            _eStarts = _intelManager.Colonies.Where(c => c.IsStartingLocation);

            // Queue a hardcoded all-in
            QueueOneBaseRavager();

            // Using the AutoHarvestGather module DesiredVespeneWorkers decides how many workers will be assigned to gather vespene 
            // Without AutoHarvestGather in the setup.json file (or and alternative) this has no effect
            _intelManager.PrimaryColony.DesiredVespeneWorkers = 4;

            // Creating a squad allows for treating a group of units as one.
            // The name allows other modules to look it up by name! (as IDs are generated at runtime)
            _theGang = _squadRep.Create("TheGang");

            // Include workers in our squad!
            foreach (var crook in _intelManager.UnitsSelf())
                if (crook.UnitType != BlizzardConstants.Unit.Overlord)
                    _theGang.AddUnit(crook);

            // The _intelManager.Handler allows for actions to be carried out when specific events happens.
            // Registering a handler to a Case means the handler will be called every time the event happens.
            _intelManager.Handler.RegisterHandler(Case.UnitAddedSelf, UnitCreationHandler);
            _intelManager.Handler.RegisterHandler(Case.StructureAddedSelf, s =>
            {
                if (s.UnitType == BlizzardConstants.Unit.RoachWarren)
                    // Increase vespene workers after the roach warren is added!
                    _intelManager.PrimaryColony.DesiredVespeneWorkers = 6;
            });

            // A variable needed since ZergDemo is a replaceable module relying on its on OnStart, but dont want it to be called twice
            _startCalled = true;
            _attackMode = false;
        }

        /// <summary>
        /// Called every step of the game, except the first.
        /// </summary>
        public void OnStep()
        {
            if (_intelManager.GameLoop % 250 == 0 && _attackMode)
                AttackEverything();
            if (_intelManager.ProductionQueue.Any())
                return;
            if (_attackMode)
                return;

            // Attack all colonies...
            foreach (var colony in _eStarts)
                _combatManager.AttackMove(_theGang, colony.Point, true);
            _attackMode = true;
            // Spam roaches...
            for (int i = 0; i < 100; i++)
                _productionManager.QueueUnit(BlizzardConstants.Unit.Roach);
        }

        /// <summary>
        /// Required to satisfy interface. Restart state trackers.
        /// </summary>
        public void OnGameEnded()
        {
            _startCalled = false;
            _attackMode = false;
        }

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
        {
            // UnitType is a stable id of the Type of the unit. BlizzardConstants contains static variables to make it easy to find the specific id you need.
            if (u.UnitType != BlizzardConstants.Unit.Overlord)
                _theGang.AddUnit(u);
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
        /// Hardcoded build-order...
        /// </summary>
        public void QueueOneBaseRavager()
        {
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