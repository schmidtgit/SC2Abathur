using Abathur.Constants;
using SC2Abathur.Settings;
using NydusNetwork.API.Protocol;
using NydusNetwork.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace SC2Abathur.Settings
{
    /// <summary>
    /// Used to serialize into data directory as a templates, if the folder is empty - or in order to 'factory reset'.
    /// This class contains a lot of information vital to the framework, but is left out of the framework as it is subject to change (patches)
    /// </summary>
    public static class Defaults {
        /// <summary>
        /// Used for setting up the modules in the Abathur framework.
        /// </summary>
        public static AbathurSetup AbathurSetup => new AbathurSetup { IsParallelized = false, Modules = new List<string>{ "EmptyModule", "AutoHarvestGather", "AutoSupply" } };
        /// <summary>
        /// Stores game related settings used by the StarCraft II client.
        /// </summary>
        public static GameSettings GameSettings => new GameSettings {
                FolderPath = @"C:\Program Files (x86)\StarCraft II",
                ConnectionAddress = IPAddress.Loopback.ToString(),
                ConnectionServerPort = 8165,
                ConnectionClientPort = 8170,
                MultiplayerSharedPort = 8175,
                InterfaceOptions = new InterfaceOptions { Raw = true, Score = true},
                Fullscreen = false,
                ClientWindowWidth = 1024,
                ClientWindowHeight = 768,
                GameMap = "Cloud Kingdom LE",
                Realtime = false,
                DisableFog = false,
                ParticipantRace = Race.Random,
                Opponents = new Collection<PlayerSetup> { new PlayerSetup { Type = PlayerType.Computer, Race = Race.Random, Difficulty = Difficulty.VeryEasy } }
            };
    }
}
