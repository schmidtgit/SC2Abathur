using System.Collections.Generic;
using NydusNetwork.API.Protocol;

namespace Abathur.Core {
    /// <summary>
    /// Tech tree is heavily used by the production manager.
    /// It provides a representation of the tech tree for each race.
    /// </summary>
    public interface ITechTree {
        /// <summary>
        /// Get the unit type id for a unit that can produce the given unit type.
        /// </summary>
        /// <param name="unit">Unit type to find producer for</param>
        /// <returns>Will return simplest form if multiple producers (etc. hatchery)</returns>
        uint GetProducer(UnitTypeData unit);

        /// <summary>
        /// Get the unit type for a unit that can research the given upgrade.
        /// </summary>
        /// <param name="upgrade">Upgrade to find researcher for</param>
        /// <returns>Will return unit capable of reseraching the upgrade</returns>
        UnitTypeData GetProducer(UpgradeData upgrade);

        /// <summary>
        /// Get the all unit type for a units that can produce the given unit type.
        /// </summary>
        /// <param name="unit">Unit type to find producer for</param>
        /// <returns>Can return multiple producers (etc. hatchery, lair, hive for Queen)</returns>
        IEnumerable<UnitTypeData> GetProducers(UnitTypeData unit);

        /// <summary>
        /// Get the all unit type ids for a units that can produce the given unit type.
        /// </summary>
        /// <param name="unit">Unit type to find producer for</param>
        /// <returns>Can return multiple producers (etc. hatchery, lair, hive for Queen)</returns>
        uint[] GetProducersID(UnitTypeData unit);

        /// <summary>
        /// Get structure required to research the given upgrade (not including producer).
        /// </summary>
        /// <param name="upgrade">Upgrade to find requirements for</param>
        /// <returns>Returns null if there are no requirements</returns>
        UnitTypeData GetRequiredBuilding(UpgradeData upgrade);

        /// <summary>
        /// Returns the id of required buildings for a unit type.
        /// Only one requirement must be met. Eg. Mustalisk -> Spire OR Greater Spire
        /// </summary>
        /// <param name="id">Unit type id to find requirements for</param>
        /// <returns>Id of required buildings</returns>
        uint[] GetRequiredBuildings(uint id);

        /// <summary>
        /// Returns the unit type of required buildings for a given unit type.
        /// Only one requirement must be met. Eg. Mustalisk -> Spire OR Greater Spire
        /// </summary>
        /// <param name="unit">Unit type to find requirements for</param>
        /// <returns>Units required to produce unit type</returns>
        IEnumerable<UnitTypeData> GetRequiredBuildings(UnitTypeData unit);

        /// <summary>
        /// Return research required before unlocking upgrade.
        /// </summary>
        /// <param name="upgrade">Upgrade to find requirements for</param>
        /// <returns>Null if there are no requirements</returns>
        UpgradeData GetRequiredResearch(UpgradeData upgrade);

        /// <summary>
        /// Does the participant own any units that can produce this type.
        /// </summary>
        /// <param name="unit">Unit type to find producer for</param>
        /// <returns></returns>
        bool HasProducer(UnitTypeData unit);

        /// <summary>
        /// Does the participant own any units that can research this upgrade.
        /// </summary>
        /// <param name="upgrade">Upgrade to find producer for</param>
        /// <returns></returns>
        bool HasProducer(UpgradeData upgrade);

        /// <summary>
        /// Does the participant have this research.
        /// </summary>
        /// <param name="id">Id of upgrade</param>
        /// <returns></returns>
        bool HasResearch(uint id);

        /// <summary>
        /// Does the participant have this research.
        /// </summary>
        /// <param name="upgrade">Upgrade type</param>
        /// <returns></returns>
        bool HasResearch(UpgradeData upgrade);

        /// <summary>
        /// Does the participant have a unit of this unit type (id).
        /// </summary>
        /// <param name="id">Unit type id</param>
        /// <returns></returns>
        bool HasUnit(uint id);

        /// <summary>
        /// Does the participant have a unit of this unit type.
        /// </summary>
        /// <param name="unit">Unit type</param>
        /// <returns></returns>
        bool HasUnit(UnitTypeData unit);
    }
}