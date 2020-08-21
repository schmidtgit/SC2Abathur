using System;
using System.Collections.Generic;
using System.Linq;
using Abathur.Core;
using Abathur.Core.Intel.Map;
using NydusNetwork.Model;

namespace Abathur.Modules.Demo
{
    public class MapRenderDemo : IModule
    {
        private IAbathur _abathur;
        private IIntelManager _intel;
        private GameMap _map;
        private int _iteration;
        private string _fileName;
        private bool _rendered;
        private List<String> _maps;
        private GameSettings _gameSettings;

        public MapRenderDemo(IIntelManager intel, GameSettings gameSettings, IAbathur abathur)
        {
            _intel = intel;
            _gameSettings = gameSettings;
            _abathur = abathur;
        }

        public void Initialize()
        {
            _maps = new List<string>
            {
                "Abiogenesis LE",
                "Acid Plant LE",
                "Antiga Shipyard",
                "Arctic Gates",
                "Backwater LE",
                "Blackpink LE",
                "Catalyst LE",
                "Cinder Fortress",
                "Cloud Kingdom LE",
                "Condemned Ridge",
                "Daybreak LE",
                "Deadlock Ridge",
                "Desolate Stronghold",
                "Dig Site",
                "District 10",
                "Dusty Gorge",
                "Eastwatch LE",
                "Entombed Valley",
                "Flooded City",
                "Green Acres",
                "Lava Flow",
                "Lunar Colony V",
                "Magma Core",
                "Megaton",
                "Molten Crater",
                "Neon Violet Square LE",
                "Ohana LE",
                "Outpost",
                "Overgrown Facility",
                "Sand Canyon",
                "Seeds of Aiur",
                "Shrines of Lizul",
                "Silent Dunes",
                "Temple of the Preservers",
                "The Bio Lab",
                "The Boneyard",
                "Traitor's Exile",
                "Tyrador Keep"
            };
        }

        public void OnStart()
        {
            _iteration++;
            _fileName = "MapRenders/Maprender_";
            _rendered = false;
        }

        public void OnStep() {
            if (!_rendered && _map.Regions!=null)
            {
                _map.RenderRegionsToDesktop(_fileName + _iteration);
                Console.WriteLine("rendered: " + _fileName+_iteration);
                _rendered = true;
            }

            if(_rendered && _maps.Count != 0) {
                var nextMap = _maps.First();
                _maps.Remove(nextMap);
                _gameSettings.GameMap = nextMap;
                _abathur.Restart();
            }
        }

        public void OnGameEnded() {}
        public void OnRestart(){}
    }
}
