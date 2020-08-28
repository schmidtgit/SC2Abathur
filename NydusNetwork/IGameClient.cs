using NydusNetwork.API.Protocol;

namespace NydusNetwork {
    public interface IGameClient {
        Status Status { get; }
        void AvailableMapsRequest();
        void CreateGameRequest();
        void DataRequest();
        void GameInfoRequest();
        void JoinGameRequest();
        void LeaveGameRequest();
        void ObservationRequest();
        void PingRequest();
        void QuickLoadRequest();
        void QuickSaveRequest();
        void QuitRequest();
        void SendRequest(Request r);
        void RestartGameRequest();
        void SaveReplayRequest();
        void StepRequest();
        bool ConnectToActiveClient();
        bool ConnectToClient();
        void Initialize(bool asHost);
        void LaunchClient(bool asHost);
        bool TryWaitAvailableMapsRequest(out Response r,int? wait = null);
        bool TryWaitCreateGameRequest(out Response r,int? wait = null);
        bool TryWaitDataRequest(out Response r,int? wait = null);
        bool TryWaitGameInfoRequest(out Response r,int? wait = null);
        bool TryWaitJoinGameRequest(out Response r,int? wait = null);
        bool TryWaitLeaveGameRequest(out Response r,int? wait = null);
        bool TryWaitObservationRequest(out Response r,int? wait = null);
        bool TryWaitPingRequest(out Response r,int? wait = null);
        bool TryWaitQuickLoadRequest(out Response r,int? wait = null);
        bool TryWaitQuickSaveRequest(out Response r,int? wait = null);
        bool TryWaitQuitRequest(out Response r,int? wait = null);
        bool TryWaitRequest(Request req,out Response r,int? wait = null);
        bool TryWaitRestartGameRequest(out Response r,int? wait = null);
        bool TryWaitSaveReplayRequest(out Response r,int? wait = null);
        bool TryWaitStepRequest(out Response r,int? wait = null);
    }
}