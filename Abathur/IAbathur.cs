using Abathur.Modules;
using NydusNetwork.API.Protocol;

namespace Abathur {
    public interface IAbathur {
        /// <summary>
        /// Game status received on last response from StarCraft II client.
        /// </summary>
        Status Status { get; }

        /// <summary>
        /// True if hosting - do not change mid-game.
        /// </summary>
        bool IsHosting { get; set; }

        /// <summary>
        /// Will run module game-steps in parrallel if true.
        /// Can be changed mid-game.
        /// </summary>
        bool IsParallelized { get; set; }

        /// <summary>
        /// Add this module to game loop if not already included.
        /// OnAdded() will be called on the module before next gamestep if added.
        /// </summary>
        /// <param name="module">Module to add</param>
        void AddToGameloop(IReplaceableModule module);

        /// <summary>
        /// Remove this module from game loop if included.
        /// OnRemove() will be called on the module.
        /// </summary>
        /// <param name="module">Module to add</param>
        void RemoveFromGameloop(IReplaceableModule module);

        /// <summary>
        /// Launch client, connect and call initialize on all modules.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Join game and run game loop.
        /// </summary>
        void Run();

        /// <summary>
        /// Leave game, create game (if host), send join request and start game loop.
        /// </summary>
        void Restart();

        /// <summary>
        /// Leave current game, create a new (even if not host).
        /// </summary>
        void CreateGame();
    }
}