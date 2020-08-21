using NydusNetwork.API.Protocol;
using System;
using System.Collections.ObjectModel;

namespace NydusNetwork.Model {
    public class GameSettings : ICloneable {
        // Technical Settings
        public string FolderPath { get; set; }
        public string ConnectionAddress { get; set; }
        public int ConnectionServerPort { get; set; }
        public int ConnectionClientPort { get; set; }
        public int MultiplayerSharedPort { get; set; }
        public InterfaceOptions InterfaceOptions { get; set; }
        // Client Settings
        public bool Fullscreen { get; set; }
        public int ClientWindowWidth { get; set; }
        public int ClientWindowHeight { get; set; }
        // Game Settings
        public string GameMap { get; set; }
        public bool DisableFog { get; set; }
        public bool Realtime { get; set; }
        public Race ParticipantRace { get; set; }
        public Collection<PlayerSetup> Opponents { get; set; } = new Collection<PlayerSetup>();
        public object Clone() => this.MemberwiseClone();
    }
}
