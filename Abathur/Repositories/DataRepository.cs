using System.Collections.Generic;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;

namespace Abathur.Repositories
{
    public class DataRepository : IAbilityRepository, IBuffRepository, IUpgradeRepository, IUnitTypeRepository {
        private Dictionary<uint,AbilityData> abilityDictionary;
        private Dictionary<uint,BuffData> buffDictionary;
        private Dictionary<uint,UpgradeData> upgradeDictionary;
        private Dictionary<uint,UnitTypeData> unitTypeDictionary;
        private ILogger log;

        public DataRepository(ILogger logger, Essence essence) {
            log = logger;
            Initialize(essence);
        }

        public void Initialize(Essence essence) {
            abilityDictionary = new Dictionary<uint,AbilityData>();
            buffDictionary = new Dictionary<uint,BuffData>();
            upgradeDictionary = new Dictionary<uint,UpgradeData>();
            unitTypeDictionary = new Dictionary<uint,UnitTypeData>();
            foreach(var ability in essence.Abilities)
                abilityDictionary.Add(ability.AbilityId,ability);
            foreach(var buff in essence.Buffs)
                buffDictionary.Add(buff.BuffId,buff);
            foreach(var unitType in essence.UnitTypes)
                unitTypeDictionary.Add(unitType.UnitId,unitType);
            foreach(var upgrade in essence.Upgrades)
                upgradeDictionary.Add(upgrade.UpgradeId,upgrade);
        }

        public IEnumerable<AbilityData> Get() => abilityDictionary.Values;
        AbilityData IAbilityRepository.Get(uint id) {
            if(abilityDictionary.TryGetValue(id,out var data))
                return data;
#if DEBUG
            log.LogError($"DataRepository: Ability with ID {id} does not exist.");
#endif
            return null;
        }

        IEnumerable<BuffData> IBuffRepository.Get() => buffDictionary.Values;

        BuffData IBuffRepository.Get(uint id) {
            if(buffDictionary.TryGetValue(id,out var data))
                return data;
#if DEBUG
            log.LogError($"DataRepository: Buff with ID {id} does not exist.");
#endif
            return null;
        }

        IEnumerable<UpgradeData> IUpgradeRepository.Get() => upgradeDictionary.Values;

        UpgradeData IUpgradeRepository.Get(uint id) {
            if(upgradeDictionary.TryGetValue(id,out var data))
                return data;
#if DEBUG
            log.LogError($"DataRepository: Upgrade with ID {id} does not exist.");
#endif
            return null;
        }
        IEnumerable<UnitTypeData> IUnitTypeRepository.Get() => unitTypeDictionary.Values;
        UnitTypeData IUnitTypeRepository.Get(uint id) {
            if(unitTypeDictionary.TryGetValue(id,out var data))
                return data;
#if DEBUG
            log.LogError($"DataRepository: UnitType with ID {id} does not exist.");
#endif
            return null;
        }
    }
}
