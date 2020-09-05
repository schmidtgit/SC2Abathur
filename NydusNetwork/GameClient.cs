using System.Diagnostics;
using NydusNetwork.API.Protocol;
using NydusNetwork.Model;
using NydusNetwork.Services;
using NydusNetwork.Logging;
using NydusNetwork.API;
using static NydusNetwork.API.Protocol.Response;
using System;

namespace NydusNetwork {
    public class GameClient : IGameClient {
        public Status Status => _connection.Status;
        private GameSettings _settings;
        private ILogger _log;
        private SCConnection _connection;
        private bool _isHost;
        public GameClient(GameSettings settings, ILogger log = null) {
            _connection = new SCConnection(log);
            _settings = settings;
            _log = log;
        }

        public void Initialize(bool asHost) {
            _isHost = asHost;
            if(!ConnectToActiveClient()) {
#if DEBUG
                _log?.LogWarning($"NydusNetwork: Launching client at {_settings.GetUri(asHost)}");
#endif
                LaunchClient(asHost);
                ConnectToClient();
            }
        }

        public void LaunchClient(bool asHost) {
            _isHost = asHost;
            Process.Start(new ProcessStartInfo { FileName = _settings.ExecutableClientPath(),Arguments = _settings.ToArguments(_isHost),WorkingDirectory = _settings.WorkingDirectory() });
        }
        public bool ConnectToClient()
            => _connection.Connect(_settings.GetUri(_isHost));
        public bool ConnectToActiveClient()
            => _connection.Connect(_settings.GetUri(_isHost), 1);
        public void SendRequest(Request r)     => _connection.AsyncSend(r);
        public void CreateGameRequest()    => SendRequest(_settings.CreateGameRequest());
        public void JoinGameRequest()      => SendRequest(_settings.JoinGameRequest(_isHost));
        public void RestartGameRequest()   => SendRequest(ClientConstant.RequestRestartGame);
        public void LeaveGameRequest()     => SendRequest(ClientConstant.RequestLeaveGame);
        public void QuickSaveRequest()     => SendRequest(ClientConstant.RequestQuickSave);
        public void QuickLoadRequest()     => SendRequest(ClientConstant.RequestQuickLoad);
        public void QuitRequest()          => SendRequest(ClientConstant.RequestQuit);
        public void GameInfoRequest()      => SendRequest(ClientConstant.RequestGameInfo);
        public void SaveReplayRequest()    => SendRequest(ClientConstant.RequestSaveReplay);
        public void AvailableMapsRequest() => SendRequest(ClientConstant.RequestAvailableMaps);
        public void PingRequest()          => SendRequest(ClientConstant.RequestPing);
        public void ObservationRequest()   => SendRequest(ClientConstant.RequestObservation);
        public void DataRequest()          => SendRequest(ClientConstant.RequestData);
        public void StepRequest()          => SendRequest(ClientConstant.RequestStep);

        public bool TryWaitRequest(Request req,out Response r,int? wait=null) {
            if(wait.HasValue)
                return _connection.TryWaitResponse(req,out r,wait.Value);
            else
                return _connection.TryWaitResponse(req,out r);
        } 
        public bool TryWaitCreateGameRequest(out Response r, int? wait = null)      => TryWaitRequest(_settings.CreateGameRequest(), out r,wait);
        public bool TryWaitJoinGameRequest(out Response r,int? wait = null)         => TryWaitRequest(_settings.JoinGameRequest(_isHost), out r,wait);
        public bool TryWaitRestartGameRequest(out Response r,int? wait = null)      => TryWaitRequest(ClientConstant.RequestRestartGame,out r,wait);
        public bool TryWaitLeaveGameRequest(out Response r,int? wait = null)        => TryWaitRequest(ClientConstant.RequestLeaveGame,out r,wait);
        public bool TryWaitQuickSaveRequest(out Response r,int? wait = null)        => TryWaitRequest(ClientConstant.RequestQuickSave,out r,wait);
        public bool TryWaitQuickLoadRequest(out Response r,int? wait = null)        => TryWaitRequest(ClientConstant.RequestQuickLoad,out r,wait);
        public bool TryWaitQuitRequest(out Response r,int? wait = null)             => TryWaitRequest(ClientConstant.RequestQuit,out r,wait);
        public bool TryWaitGameInfoRequest(out Response r,int? wait = null)         => TryWaitRequest(ClientConstant.RequestGameInfo,out r,wait);
        public bool TryWaitSaveReplayRequest(out Response r,int? wait = null)       => TryWaitRequest(ClientConstant.RequestSaveReplay,out r,wait);
        public bool TryWaitAvailableMapsRequest(out Response r,int? wait = null)    => TryWaitRequest(ClientConstant.RequestAvailableMaps,out r,wait);
        public bool TryWaitPingRequest(out Response r,int? wait = null)             => TryWaitRequest(ClientConstant.RequestPing,out r,wait);
        public bool TryWaitObservationRequest(out Response r,int? wait = null)      => TryWaitRequest(ClientConstant.RequestObservation,out r,wait);
        public bool TryWaitStepRequest(out Response r,int? wait = null)             => TryWaitRequest(ClientConstant.RequestStep,out r,wait);
        public bool TryWaitDataRequest(out Response r,int? wait = null)             => TryWaitRequest(ClientConstant.RequestData,out r,wait);
        // Handlers
        public void RegisterHandler(ResponseOneofCase action, Action<Response> handler)
            => _connection.RegisterHandler(action, handler);
        public void DeregisterHandler(Action<Response> handler)
            => _connection.DeregisterHandler(handler);

        public void Disconnect()
            => _connection.Disconnect();
    }
}
