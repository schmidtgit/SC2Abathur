using Abathur.Modules;

namespace SC2Abathur.Modules {
    // Replaceable modules are extended IModules and provide two key features:
    // 1) Any replaceble module can be access through dependency injection in the constructor (concrete implementation)
    // 2) Replaceble modules can be added and removed from the game loop at runtime (eg. for drastic stategy change)
    public class ReplaceableModule : IReplaceableModule {
        // Called when added at runtime (not if added in the setup file)
        public void OnAdded() {}

        // Added when removed at runtime - use to deregister IntelManager handles etc.
        public void OnRemoved() {}

        // Called after connection is established to the StarCraft II Client
        // NOT called if added at runtime (as modules may be added/removed multiple times)
        public void Initialize() {}

        // Called on the first frame in each game (not called if added mid-game)
        public void OnStart() {}

        // Called in every frame - except the first (use OnStart).
        // This method is called asynchronous if the framework IsParallelized is true in the setup file.
        public void OnStep() {}

        // Called when game has ended but before leaving the match (not called if removed mid-game)
        public void OnGameEnded() {}

        // Called before starting when starting a new game (but not the first) - can be called mid-game if a module request a restart
        public void OnRestart() {}
    }
}
