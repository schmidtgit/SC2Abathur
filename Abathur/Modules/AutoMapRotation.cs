using Abathur.Core;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;
using NydusNetwork.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abathur.Modules
{
    class AutoMapRotation : IModule {
        private ILogger log;
        private IAbathur abathur;
        private IRawManager rawManager;
        private IIntelManager intelManager;
        private List<string> maps;
        private GameSettings settings;
        public AutoMapRotation(IRawManager rawManager, ILogger logger, GameSettings settings, IAbathur abathur, IIntelManager intelManager) {
            this.abathur = abathur;
            this.rawManager = rawManager;
            this.settings = settings;
            this.intelManager = intelManager;
            this.log = logger;
            maps = new List<string> {
                "Flooded City",
                "Deadlock Ridge",
                "Antiga Shipyard",
                "Arctic Gates",
                "Cinder Fortress",
                "Condemned Ridge",
                "Daybreak LE",
                "Desolate Stronghold",
                "Dig Site",
                "District 10",
                "Eastwatch LE",
                "Entombed Valley",
                "Forgotten Sanctuary",
                "Green Acres",
                "Last Remnant",
                "Lava Flow",
                "Lunar Colony V",
                "Magma Core",
                "Megaton",
                "Molten Crater",
                "Ohana LE",
                "Old Estate",
                "Outpost",
                "Overgrown Facility",
                "Sand Canyon",
                "Seeds of Aiur",
                "Silent Dunes",
                "Snowy Mesa",
                "Temple of the Preservers",
                "The Bio Lab",
                "The Boneyard",
                "The Ruins of Tarsonis",
                "Traitor's Exile",
                "Tropic Shores",
                "Tyrador Keep",
                "Cloud Kingdom LE"
            };
        }

        public void Initialize() {
            if(maps != null)
                return;
            if(rawManager.TryWaitRawRequest(new Request { AvailableMaps = new RequestAvailableMaps { } },out var response))
                maps = new List<string>(response.AvailableMaps.BattlenetMapNames);
            else
                throw new TimeoutException();
        }
        private int index;
        public void OnGameEnded() {
            if(index == maps.Count)
                return;
            var mapName = maps[index++%maps.Count];
            log.LogSuccess($"Map Rotation: Next Map => {mapName}");
            settings.GameMap = mapName;
            abathur.Restart();
        }
        public void OnStart() {}
        public void OnStep() {}
        public void OnRestart() {}
    }
}
