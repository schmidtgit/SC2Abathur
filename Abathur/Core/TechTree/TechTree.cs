using Abathur.Constants;
using Abathur.Repositories;
using NydusNetwork.API.Protocol;
using System.Collections.Generic;
using System.Linq;

namespace Abathur.Core.Intel {
    public class TechTree : ITechTree {
        private IIntelManager intelManager;
        private IUnitTypeRepository unitTypeRepository;
        private IUpgradeRepository upgradeRepository;
        private Dictionary<uint,uint[]> Unit_To_Producers { get; set; }
        private Dictionary<uint,uint[]> Unit_To_RequiredBuildings { get; set; }
        private Dictionary<uint,uint> Research_To_Researcher { get; set; }
        private Dictionary<uint,uint> Research_To_RequiredBuildings { get; set; }
        private Dictionary<uint,uint> Research_To_RequiredResearch { get; set; }

        public TechTree(IIntelManager intelManager, IUnitTypeRepository unitTypeRepository,IUpgradeRepository upgradeRepository, Essence essence) {
            this.intelManager = intelManager;
            this.unitTypeRepository = unitTypeRepository;
            this.upgradeRepository = upgradeRepository;
            Initialize(essence);
        }

        public UnitTypeData GetProducer(UpgradeData upgrade) {
            if(Research_To_Researcher.TryGetValue(upgrade.UpgradeId,out var id))
                return unitTypeRepository.Get(id);
            throw new System.NotImplementedException();
        }
        public UpgradeData GetRequiredResearch(UpgradeData upgrade) {
            if(Research_To_RequiredResearch.TryGetValue(upgrade.UpgradeId,out var id))
                return upgradeRepository.Get(id);
            return null;
        }
        public UnitTypeData GetRequiredBuilding(UpgradeData upgrade) {
            if(Research_To_RequiredBuildings.TryGetValue(upgrade.UpgradeId, out var id))
                return unitTypeRepository.Get(id);
            return null;
        }
        public IEnumerable<UnitTypeData> GetProducers(UnitTypeData unit) {
            if(Unit_To_Producers.TryGetValue(unit.UnitId,out var ids))
                return ids.Select(i => unitTypeRepository.Get(i));
            throw new System.NotImplementedException();
        }

        public IEnumerable<UnitTypeData> GetRequiredBuildings(UnitTypeData unit) {
            if(Unit_To_RequiredBuildings.TryGetValue(unit.UnitId,out var ids))
                return ids.Select(i => unitTypeRepository.Get(i));
            return Enumerable.Empty<UnitTypeData>();
        }

        public uint[] GetProducersID(UnitTypeData unit) {
            if(Unit_To_Producers.TryGetValue(unit.UnitId,out var ids))
                return ids;
            throw new System.NotImplementedException();
        }
        public uint GetProducer(UnitTypeData unit) {
            if(Unit_To_Producers.TryGetValue(unit.UnitId,out var ids))
                return ids[0];
            throw new System.NotImplementedException();
        }
        public uint[] GetRequiredBuildings(uint id) {
            if(Unit_To_RequiredBuildings.TryGetValue(id,out var result))
                return result;
            return new uint[0];
        }

        private void Initialize(Essence essence) {
            Unit_To_Producers = new Dictionary<uint,uint[]>();
            Unit_To_RequiredBuildings = new Dictionary<uint,uint[]>();
            Research_To_Researcher = new Dictionary<uint,uint>();
            Research_To_RequiredBuildings = new Dictionary<uint,uint>();
            Research_To_RequiredResearch = new Dictionary<uint,uint>();
            foreach(var pair in essence.UnitProducers)
                Unit_To_Producers.Add(pair.Key,pair.Values.ToArray());
            foreach(var pair in essence.UnitRequiredBuildings)
                Unit_To_RequiredBuildings.Add(pair.Key,pair.Values.ToArray());
            foreach(var pair in essence.ResearchProducer)
                Research_To_Researcher.Add(pair.Key,pair.Value);
            foreach(var pair in essence.ResearchRequiredBuildings)
                Research_To_RequiredBuildings.Add(pair.Key,pair.Value);
            foreach(var pair in essence.ResearchRequiredResearch)
                Research_To_RequiredResearch.Add(pair.Key,pair.Value);
        }

        public bool HasResearch(UpgradeData upgrade) => upgrade == null || HasResearch(upgrade.UpgradeId);

        public bool HasResearch(uint id) => id == 0 || intelManager.UpgradesSelf.Any(u => u.UpgradeId == id);

        public bool HasUnit(UnitTypeData unit) => unit == null || HasUnit(unit.UnitId);

        public bool HasUnit(uint id) {
            if(GameConstants.IsWorker(id) || GameConstants.IsLarva(id))
                return true;
            if(id == BlizzardConstants.Unit.Hatchery)
                return intelManager.StructuresSelf().Any(u => (u.UnitType == id || u.UnitType == BlizzardConstants.Unit.Lair || u.UnitType == BlizzardConstants.Unit.Hive) && u.BuildProgress == 1f);
            if(id == BlizzardConstants.Unit.Lair)
                return intelManager.StructuresSelf().Any(u => (u.UnitType == id || u.UnitType == BlizzardConstants.Unit.Hive) && u.BuildProgress == 1f);
            return intelManager.StructuresSelf().Any(u => u.UnitType == id && u.BuildProgress == 1f) || intelManager.UnitsSelf().Any(u => u.UnitType == id);
        }


        public bool HasProducer(UpgradeData upgrade) => HasUnit(GetProducer(upgrade));
        public bool HasProducer(UnitTypeData unit) {
            foreach(var id in GetProducersID(unit))
                if(HasUnit(id))
                    return true;
            return false;
        }
    }
}
