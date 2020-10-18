using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abathur.Constants;
using Abathur.Core;
using Abathur.Core.Combat;
using Abathur.Modules.Services;
using Abathur.Repositories;

namespace Abathur.Modules
{
    class AutoExpand : IModule
    {
        private IIntelManager _intel;
        private ISquadRepository _squadRepository;
        private Squad _mainBuildings;
        private IProductionManager _productionManager;
        private int _baseAmount = 1;
        private IList<uint> _mainBuildingTypes;
        private uint workerType;
        private Squad _refineries;

        public AutoExpand(IIntelManager intel, ISquadRepository squadRepository, IProductionManager productionManager)
        {
            _intel = intel;
            _squadRepository = squadRepository;
            _productionManager = productionManager;
        }
        public void Initialize()
        {
            
        }

        public void OnStart()
        {
            _mainBuildingTypes = new List<uint>();
            switch(GameConstants.ParticipantRace) {
                case NydusNetwork.API.Protocol.Race.NoRace:
                    break;
                case NydusNetwork.API.Protocol.Race.Terran:
                    _mainBuildingTypes.Add(BlizzardConstants.Unit.CommandCenter);
                    _mainBuildingTypes.Add(BlizzardConstants.Unit.OrbitalCommand);
                    _mainBuildingTypes.Add(BlizzardConstants.Unit.PlanetaryFortress);
                    workerType = BlizzardConstants.Unit.SCV;
                    break;
                case NydusNetwork.API.Protocol.Race.Zerg:
                    _mainBuildingTypes.Add(BlizzardConstants.Unit.Hatchery);
                    _mainBuildingTypes.Add(BlizzardConstants.Unit.Lair);
                    _mainBuildingTypes.Add(BlizzardConstants.Unit.Hive);
                    workerType = BlizzardConstants.Unit.Drone;
                    break;
                case NydusNetwork.API.Protocol.Race.Protoss:
                    _mainBuildingTypes.Add(BlizzardConstants.Unit.Nexus);
                    workerType = BlizzardConstants.Unit.Probe;
                    break;
                case NydusNetwork.API.Protocol.Race.Random:
                    break;
            }


            _mainBuildings = _squadRepository.Create("MainBuildings");
            _refineries = _squadRepository.Create("Refineries");
            _mainBuildings.AddUnit(_intel.StructuresSelf().First(u => _mainBuildingTypes.Contains(u.UnitType)));

            _intel.Handler.RegisterHandler(Case.StructureAddedSelf,u => {
                if(_mainBuildingTypes.Contains(u.UnitType))
                {
                    _mainBuildings.AddUnit(u);
                    _baseAmount++;
                }
                if (GameConstants.RaceRefinery == u.UnitType)
                {
                    _refineries.AddUnit(u);
                }
            });
        }

        public void OnStep()
        {
            var workerAmount = _intel.WorkersSelf().Count();

            if (!_intel.ProductionQueue.Any() &&  workerAmount < _baseAmount*16)
            {
                for(int i = _intel.WorkersSelf().Count(); i < _baseAmount * 16; i++) {
                    _productionManager.QueueUnit(workerType);
                }
            }
            else if (!_intel.ProductionQueue.Any()) //TODO Seemingly 4 SCV's get stuck in the production queue but is never made(using AutoExpand and AutoSupply)
            {
                var curCol = _mainBuildings.Units.First();
                //TODO Use AStar distance rather than a Euclidian distance
                var nextCol = _intel.Colonies.OrderBy(c => MathServices.EuclidianDistance(curCol.Point, c.Point)).FirstOrDefault(col => col.Structures.Count == 0);
                if (nextCol!=null)
                {
                    _productionManager.QueueUnit(_mainBuildingTypes.First(),nextCol.Point,3);//TODO the placement algorithm does not take mineral spacing from HQ's into account and giving a spacing of 3 will make other objects block the HQ's
                }
            }

        }

        public void OnGameEnded()
        {
            
        }

        public void OnRestart()
        {
            
        }
    }
}
