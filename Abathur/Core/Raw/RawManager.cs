using NydusNetwork.API.Protocol;
using NydusNetwork;
using System.Collections.Generic;
using NydusNetwork.Logging;

namespace Abathur.Core.Raw
{
    public class RawManager : IRawManager {
        public Status Status => _client.Status;
        private const int TIME_OUT = 30000;
        private List<Action> commands;
        private IGameClient _client;
        private ILogger log;

        public bool Realtime    { get; set; }
        public bool IsHosting   { get; set; }

        public RawManager(IGameClient gameClient, ILogger logger) {
            commands = new List<Action>();
            _client = gameClient;
            log = logger;
        }

        public void SendRawRequest(Request r) => _client.AsyncRequest(r);

        public bool TryWaitRawRequest(Request req,out Response response) => TryWaitRawRequest(req,out response, TIME_OUT);

        public bool TryWaitRawRequest(Request req,out Response response,int timeout)
            => _client.TryWaitRequest(req,out response,timeout);

        public bool TryWaitObservationRequest(out Response response, uint gameloop) {
            do {
                if(!_client.TryWaitObservationRequest(out response,TIME_OUT))
                    return false;
            } while(Realtime && response.Observation.Observation.GameLoop == gameloop);
            return true;
        }

        public void QueueActions(params Action[] actions) {
            lock(actions)
                foreach(var a in actions)
                    commands.Add(a);
        }

        public void Step() {
            lock(commands) {
                if(commands.Count != 0) {
                    _client.AsyncRequest(new Request { Action = new RequestAction { Actions = { commands } } });
                    commands.Clear();
                }
            }
            if(!Realtime && Status != Status.Ended)
                if(!_client.TryWaitStepRequest(out var r))
                    log.LogError("RawManager: Did not receive step-response from StarCraft II client");
        }

        public void Initialize() => _client.Initialize(IsHosting);

        public bool CreateGame() {
            if(!_client.TryWaitCreateGameRequest(out var createResponse,TIME_OUT)) {
                log.LogError("RawManager: Timed out on CreateGame");
                return false;
            }

            if(createResponse.CreateGame.Error != ResponseCreateGame.Types.Error.Unset) {
                log.LogError($"RawManager: {createResponse.CreateGame.Error} : {createResponse.CreateGame.ErrorDetails}");
                return false;
            }
            return true;
        }

        public bool JoinGame() {
            _client.AsyncPingRequest();
            if(!_client.TryWaitJoinGameRequest(out var joinResponse,TIME_OUT)) {
                log.LogError("RawManager: Timed out on JoinGame");
                return false;
            } else if(joinResponse.JoinGame.Error != ResponseJoinGame.Types.Error.Unset){
                log.LogError($"RawManager: {joinResponse.JoinGame.Error} : {joinResponse.JoinGame.ErrorDetails}");
                return false;
            }
            return true;
        }

        public void Restart() {
            if(!_client.TryWaitLeaveGameRequest(out var response, TIME_OUT))
                throw new System.TimeoutException("RawManager: StarCraft II client took to long to respond.");
            if(IsHosting && !CreateGame())
                throw new System.Exception("RawManager: Unable to create game!");
            if(!JoinGame())
                throw new System.Exception("RawManager: Unable to join game!");
        }

        public bool LeaveGame() {
            if(!_client.TryWaitLeaveGameRequest(out var response,TIME_OUT))
                throw new System.TimeoutException("RawManager: StarCraft II client took to long to respond.");
            return true;
        }
    }
}
