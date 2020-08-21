using Abathur.Modules;

namespace SC2Abathur.Modules {
    // Everything inheriting from IModule can be added in the Abathur setup file (use class name)
    // The Abathur Framework will handle instantiation and call events.
    public class EmptyModule : IModule {
        // Called after connection is established to the StarCraft II Client, but before a game is entered.
        public void Initialize() {}

        // Called on the first frame in each game.
        public void OnStart() {}

        // Called in every frame - except the first (use OnStart).
        // This method is called asynchronous if the framework IsParallelized is true in the setup file.
        public void OnStep() {}

        // Called when game has ended but before leaving the match.
        public void OnGameEnded() {}

        // Called before starting when starting a new game (but not the first) - can be called mid-game if a module request a restart
        public void OnRestart() {}
    }
}
