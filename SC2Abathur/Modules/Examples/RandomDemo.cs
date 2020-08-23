using Abathur;
using Abathur.Constants;
using Abathur.Modules;
using NydusNetwork.Logging;
using SC2Abathur.Modules.Examples;

namespace Launcher.Modules.Examples
{
    /// <summary>
    /// This demo showcase how IReplaceableModules can be swapped at run-time.
    /// Everything that implements from IReplaceableModule is avalible using dependency injection.
    /// </summary>
    class RandomDemo : IReplaceableModule
    {
        // Need access to replace modules.
        private IAbathur _abathur;
        // Logger is optional
        private ILogger _log;
        // All these inherit from IReplaceableModule
        private ZergDemo _zergModule;
        private TerranDemo _terranModule;
        private ProtossDemo _protossModule;

        /// <summary>
        /// The varibles are automaticly populated at run-time using dependency injection.
        /// Abathur exposes itself and a number of services - everything that implements IReplaceableModule can be accessed.
        /// </summary>
        public RandomDemo(IAbathur abathur, ILogger log, TerranDemo terranModule, ProtossDemo protossModule, ZergDemo zergModule)
        {
            _terranModule = terranModule;
            _protossModule = protossModule;
            _zergModule = zergModule;
            _abathur = abathur;
            _log = log;
        }

        /// <summary>
        /// The race has not yet been detected at intialized (if random)
        /// We therefore wait for OnStart();
        /// </summary>
        public void Initialize() { }

        /// <summary>
        /// Detect the which race we are and inject the corrects strategy.
        /// </summary>
        public void OnStart()
        {
            switch (GameConstants.ParticipantRace)
            {
                case NydusNetwork.API.Protocol.Race.NoRace:
                    break;
                case NydusNetwork.API.Protocol.Race.Terran:
                    _abathur.AddToGameloop(_terranModule);
                    break;
                case NydusNetwork.API.Protocol.Race.Zerg:
                    _abathur.AddToGameloop(_zergModule);
                    break;
                case NydusNetwork.API.Protocol.Race.Protoss:
                    _abathur.AddToGameloop(_protossModule);
                    break;
                case NydusNetwork.API.Protocol.Race.Random:
                    _log.LogError("RandomDemo: Race could not be detected --- nothing was added");
                    break;
            }
        }

        /// <summary>
        /// Called every frame after the first.
        /// </summary>
        public void OnStep() { }

        /// <summary>
        /// Called when the game ends. Set race back to random.
        /// </summary>
        public void OnGameEnded() {
            GameConstants.ParticipantRace = NydusNetwork.API.Protocol.Race.Random;
        }

        /// <summary>
        /// Abathur will not automaticly remove modules added at run-time.
        /// We need to manually remove the modules we added.
        /// </summary>
        public void OnRestart()
        {
            _abathur.RemoveFromGameloop(_terranModule);
            _abathur.RemoveFromGameloop(_zergModule);
            _abathur.RemoveFromGameloop(_protossModule);
        }

        /// <summary>
        /// If we get added to Abathur during run-time, treat it as OnStart()
        /// </summary>
        public void OnAdded() => OnStart();

        /// <summary>
        /// If we get removed from Abathur, clean up.
        /// </summary>
        public void OnRemoved() => OnRestart();
    }
}