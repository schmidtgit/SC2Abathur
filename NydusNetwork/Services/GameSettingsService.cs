using System;
using System.Text;
using NydusNetwork.API.Protocol;
using NydusNetwork.Model;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;

namespace NydusNetwork.Services {
    public static class GameSettingsService {
        public static string ToArguments(this GameSettings gs, bool isHost) {
            var sb = new StringBuilder();
            sb.Append($"{ClientConstant.Address} {gs.ConnectionAddress} ");

            if(isHost)
                sb.Append($"{ClientConstant.Port} {gs.ConnectionServerPort} ");
            else
                sb.Append($"{ClientConstant.Port} {gs.ConnectionClientPort} ");

            if(gs.Fullscreen)
                sb.Append($"{ClientConstant.Fullscreen} 1 ");
            else
                sb.Append($"{ClientConstant.Fullscreen} 0 {ClientConstant.WindowHeight} {gs.ClientWindowHeight} {ClientConstant.WindowVertical} {gs.ClientWindowWidth} ");

            if(!isHost)
                sb.Append($"{ClientConstant.WindowVertical} {gs.ClientWindowWidth} ");
            return sb.ToString();
        }

        public static bool IsMultiplayer(this GameSettings gs) => gs.Opponents.Any(c => c.Type == PlayerType.Participant);

        public static PortSet ServerPort(this GameSettings gs)
            => new PortSet { GamePort = gs.MultiplayerSharedPort + 1, BasePort = gs.MultiplayerSharedPort + 2 };
            

        public static Collection<PortSet> ClientPorts(this GameSettings gs)
            => new Collection<PortSet> { new PortSet{ GamePort = gs.ConnectionClientPort + 1, BasePort = gs.ConnectionServerPort + 1} };
        
        public static Request JoinGameRequest(this GameSettings gs, bool isHost) {
            if(gs.IsMultiplayer())
                return new Request {
                    JoinGame = new RequestJoinGame {
                        Race = isHost ? gs.ParticipantRace : gs.Opponents.First(o => o.Type == PlayerType.Participant).Race,
                        Options = gs.InterfaceOptions,
                        SharedPort = gs.MultiplayerSharedPort,
                        ServerPorts = gs.ServerPort(),
                        ClientPorts = { gs.ClientPorts() }
                    }
                };
            else
                return new Request {
                    JoinGame = new RequestJoinGame {
                        Race = gs.ParticipantRace,
                        Options = gs.InterfaceOptions
                    }
                };
        }

        public static Request CreateGameRequest(this GameSettings gs) {
            var r = new Request {
                CreateGame = new RequestCreateGame {
                    DisableFog = gs.DisableFog,
                    Realtime = gs.Realtime,
                    PlayerSetup = { gs.Opponents,new PlayerSetup { Type = PlayerType.Participant,Race = gs.ParticipantRace } }
                }
            };
            if(File.Exists(gs.GameMap) || File.Exists($"{gs.FolderPath}\\Maps\\{gs.GameMap}"))
                r.CreateGame.LocalMap = new LocalMap { MapPath = gs.GameMap };
            else
                r.CreateGame.BattlenetMapName = gs.GameMap;
            return r;
        }

        public static Uri GetUri(this GameSettings gs,bool IsHost)
            => IsHost ? new Uri($"ws://{gs.ConnectionAddress}:{gs.ConnectionServerPort}/sc2api") : new Uri($"ws://{gs.ConnectionAddress}:{gs.ConnectionClientPort}/sc2api");

        public static string WorkingDirectory(this GameSettings gs) => $"{gs.FolderPath}\\Support";

        public static string ExecutableClientPath(this GameSettings gs) {
            if(System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
                return Directory.GetDirectories(gs.FolderPath + @"\Versions\",@"Base*")[0] + @"\SC2.app";
            else
                return Directory.GetDirectories(gs.FolderPath + @"\Versions\",@"Base*")[0] + @"\SC2.exe";
        } 
    }
}
