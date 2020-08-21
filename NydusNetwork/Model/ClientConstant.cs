using NydusNetwork.API.Protocol;

namespace NydusNetwork.Model {
    public class ClientConstant {
        public const string Address             = "-listen";
        public const string Port                = "-port";
        public const string Fullscreen          = "-displaymode";
        public const string WindowWidth         = "-windowwidth";
        public const string WindowHeight        = "-windowheight";
        public const string WindowVertical      = "-windowx";
        public const string WindowHorizontal    = "-windowy";
        public static readonly Request RequestRestartGame   = new Request { RestartGame =   new RequestRestartGame() };
        public static readonly Request RequestLeaveGame     = new Request { LeaveGame =     new RequestLeaveGame() };
        public static readonly Request RequestQuickSave     = new Request { QuickSave =     new RequestQuickSave() };
        public static readonly Request RequestQuickLoad     = new Request { QuickLoad =     new RequestQuickLoad() };
        public static readonly Request RequestQuit          = new Request { Quit =          new RequestQuit() };
        public static readonly Request RequestGameInfo      = new Request { GameInfo =      new RequestGameInfo() };
        public static readonly Request RequestSaveReplay    = new Request { SaveReplay =    new RequestSaveReplay() };
        public static readonly Request RequestAvailableMaps = new Request { AvailableMaps = new RequestAvailableMaps() };
        public static readonly Request RequestPing          = new Request { Ping =          new RequestPing() };
        public static readonly Request RequestObservation   = new Request { Observation =   new RequestObservation() };
        public static readonly Request RequestStep          = new Request { Step =          new RequestStep { } };
        public static readonly Request RequestData          = new Request { Data =          new RequestData { AbilityId = true, BuffId = true, UnitTypeId = true, UpgradeId = true} };
    }
}
