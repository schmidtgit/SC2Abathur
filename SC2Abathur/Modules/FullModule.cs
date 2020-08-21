using Abathur.Core;
using Abathur.Modules;
using Abathur.Repositories;
using NydusNetwork.Logging;
using NydusNetwork.Model;

namespace SC2Abathur.Modules {
    // Everything inheriting from IModule can be added in the Abathur setup file (use class name)
    // The constructor can take a series of interfaces due to dependency injection, allowing access to core functionality
    public class FullModule : IModule {
        // The ILogger provides logging options.
        // The framework can be provided with a file logger to save the log, console logger to show at runtime
        // or a MultiLogger do archive both or do slient 'void' logging.
        private ILogger log;

        // The IntelManager provide a sorted representation of the current game state.
        // Get self or enemy workers, structures (any unit with structure attribute) and units (anything else...)
        // It also provides a handler for getting events, eg. on EnemyUnitAdded (seen for the first time)
        // IUnits are updated each frame by the IntelManager, it is save to keep a reference to them.
        private IIntelManager intelManager;

        // CombatManager is responsible for sending commands to units and can handle 'Squads' (collection of Units)
        // Squads are also save to keep a refenrence to as they dead units are automaticly removed.
        // Squads can be named so they can be utilized by multiple modules.
        private ICombatManager combatManager;

        // The production manager will let you produce units and research upgrades.
        // It is possible to provide a suggested location (production manager will overrule if blocked).
        // Request will always be attempted satisfied, eg. queing a BarracksTechLab will queue Barracks and Supply Depot if none exist.
        // The production manager will log warnings everytime it had to correct something in order to satisfy the queue.
        private IProductionManager productionManager;

        // RawManager is used internally by the Abathur Framework to queue up commands.
        // It is provided here to allow developers access to "RawCommands" (protobuf request sent immediately) 
        private IRawManager rawManager;

        // Query Abilities from DataRequest (cached)
        private IAbilityRepository abilityRepository;

        // Query UnitType from DataRequest (cached and morphed unit cost manipulated to actual cost)
        private IUnitTypeRepository unitTypeRepository;

        // Query Upgrades from DataRequest (cached)
        private IUpgradeRepository upgradeRepository;

        // Query Buffs from DataRequest (cached)
        private IBuffRepository buffRepository;

        // Create and access squads (syncronized across modules and programming languages)
        private ISquadRepository squadRepository;

        // Find valid locations etc.
        private IGameMap gameMap;

        // Ask 'what requires what' questions. Current research is stored in IntelManager
        private ITechTree techTree;

        // Used to setup the game - only provided here to allow for map and race swapping between games.
        private GameSettings gameSettings;
        
        /// IModules can take any number of these in the constructor and in any order.
        /// It is all handled using Dependency Injection.
        public FullModule(ILogger log, IIntelManager intelManager,ICombatManager combatManager,
            IProductionManager productionManager, IRawManager rawManager, IAbilityRepository abilityRepository,
            IUnitTypeRepository unitTypeRepository, IUpgradeRepository upgradeRepository, IBuffRepository buffRepository,
            ISquadRepository squadRepository, IGameMap gameMap, ITechTree techTree, GameSettings gameSettings) {
            this.log = log;
            this.intelManager = intelManager;
            this.combatManager = combatManager;
            this.productionManager = productionManager;
            this.rawManager = rawManager;
            this.abilityRepository = abilityRepository;
            this.unitTypeRepository = unitTypeRepository;
            this.upgradeRepository = upgradeRepository;
            this.buffRepository = buffRepository;
            this.squadRepository = squadRepository;
            this.gameMap = gameMap;
            this.techTree = techTree;
        }

        // Called after connection is established to the StarCraft II Client, but before a game is entered.
        public void Initialize() { }

        // Called on the first frame in each game.
        public void OnStart() { }

        // Called in every frame - except the first (use OnStart).
        // This method is called asynchronous if the framework IsParallelized is true in the setup file.
        public void OnStep() { }

        // Called when game has ended but before leaving the match.
        public void OnGameEnded() { }

        // Called before starting when starting a new game (but not the first) - can be called mid-game if a module request a restart
        public void OnRestart() { }
    }
}
