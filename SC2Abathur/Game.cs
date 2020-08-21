using SC2Abathur.Client;
using NydusNetwork.Model;
using System.Threading.Tasks;

namespace Launcher {
    /// <summary>
    /// Representation of a single-client (AI vs. Blizzard AI) or multi-client (AI vs. Human or AI vs. AI) match.
    /// </summary>
    public class Game{
        /// <summary>
        /// The first player - always assumed to be host.
        /// </summary>
        IClient PlayerOne { get; set; }

        /// <summary>
        /// The second player - always assumed to be 'client'.
        /// </summary>
        IClient PlayerTwo { get; set; }

        /// <summary>
        /// Settings used for the StarCraft II client to setup the match.
        /// </summary>
        GameSettings GameSettings { get; set; }

        /// <summary>
        /// Representation of a StarCraft II match.
        /// </summary>
        /// <param name="settings">Gamesettings are assumed to support the type of match</param>
        /// <param name="PlayerOne">Host of the game</param>
        /// <param name="PlayerTwo">Optional opponent</param>
        public Game(GameSettings settings, IClient PlayerOne, IClient PlayerTwo = null) {
            GameSettings = settings;
            this.PlayerOne = PlayerOne;
            this.PlayerTwo = PlayerTwo;
        }

        /// <summary>
        /// Used to start the match.
        /// IClients are expected to handle everything from here.
        /// </summary>
        public void ExecuteMatch() {
            PlayerOne?.SetHost(true);
            PlayerTwo?.SetHost(false);

            // Launch StarCraft II clients and connect
            PlayerOne?.Initialize();
            PlayerTwo?.Initialize();

            // Let the host create the game
            PlayerOne?.CreateGame();

            // Let both run and play (asynchronous)
            var p1 = Task.Run(() => PlayerOne?.JoinGame());
            var p2 = Task.Run(() => PlayerTwo?.JoinGame());

            // Prevent main-thread from closing the application prematurely.
            Task.WaitAll(p1,p2); 
        }
    }
}
