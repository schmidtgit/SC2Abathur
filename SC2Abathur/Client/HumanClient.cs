using NydusNetwork;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;
using NydusNetwork.Model;
using System.Diagnostics;
using System.Threading;
using static SC2Abathur.Services.GameSpeedService;

namespace SC2Abathur.Client {
    /// <summary>
    /// Launches a client that does nothing.
    /// </summary>
    public class HumanClient : IClient {
        private const int TIMEOUT = 100000;
        private IGameClient _client;
        private GameSettings _settings;
        private ILogger _log;
        private GameSpeed _speed;
        private bool isHost;

        /// <summary>
        /// The human client will take on the race of the first 'opponent' in the gamesettings file.
        /// </summary>
        /// <param name="race">Race to play</param>
        public HumanClient(GameSettings gameSettings, GameSpeed speed = GameSpeed.Faster, ILogger log = null) {
            _settings = gameSettings;
            _client = new GameClient(_settings,log);
            _speed = speed;
            _log = log;
        }

        /// <inheritdoc />
        public void Initialize() => _client.Initialize(isHost);

        /// <inheritdoc />
        public void CreateGame() {
            if(!_client.TryWaitCreateGameRequest(out var response,TIMEOUT))
                _log?.LogError($"HumanClient: Timed out on CreateGame.");
            else if(response.CreateGame.Error == ResponseCreateGame.Types.Error.Unset)
                _log?.LogSuccess($"HumanClient: Created game => {_settings.GameMap}");
            else
                _log?.LogError($"HumanClient: Failed on CreateGame | {response.CreateGame.Error}");
        }

        /// <inheritdoc />
        public void JoinGame() {
            if(!_client.TryWaitJoinGameRequest(out var response,TIMEOUT))
                _log?.LogError($"HumanClient: Timed out on JoinGame.");
            else if(response.JoinGame.Error != ResponseJoinGame.Types.Error.Unset)
                _log?.LogError($"HumanClient: Failed on CreateGame | {response.CreateGame.Error}");

            var delay = MillisecondsBetweenSteps(_speed);
            var watch = new Stopwatch();
            watch.Start();
            // If step mode, keep stepping!
            if(!_settings.Realtime)
                do {
                    // Attempt to delay client so the speed match normal gamespeeds.
                    Thread.Sleep(System.Math.Max(delay - (int)watch.ElapsedMilliseconds,0));
                    watch.Restart();
                    if(!_client.TryWaitStepRequest(out response,TIMEOUT)) {
                        _log?.LogError("HumanClient: Timed out on Step Request.");
                    }
                } while(response.Status == Status.InGame);
        }

        /// <inheritdoc />
        public void SetHost(bool asHost) => isHost = asHost;
    }
}
