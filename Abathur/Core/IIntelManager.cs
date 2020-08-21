using System.Collections.Generic;
using NydusNetwork.API.Protocol;
using Abathur.Modules;
using NydusNetwork.Services;
using Abathur.Model;

namespace Abathur.Core {
    /// <summary>
    /// Responsible for updating representation of the current gamestate.
    /// </summary>
    public interface IIntelManager : IModule {
        /// <summary>
        /// Get interpretation of the map (creep, visibility and blocked grid)
        /// </summary>
        IGameMap GameMap { get; }
        /// <summary>
        /// Get current gameloop (updated in realtime and step mode)
        /// </summary>
        uint GameLoop { get; }
        /// <summary>
        /// Get the current score (updated each step)
        /// </summary>
        Score CurrentScore { get; }
        /// <summary>
        /// Get PlayerCommon (e.g. ressources)
        /// </summary>
        PlayerCommon Common { get; }

        /// <summary>
        /// Register handlers for events.
        /// </summary>
        CaseHandler<Case,IUnit> Handler { get; }

        /// <summary>
        /// Get all upgrades currently researched.
        /// </summary>
        ICollection<UpgradeData> UpgradesSelf { get; }

        /// <summary>
        /// Get structure controlled by participant, with given tag.
        /// </summary>
        /// <param name="tag">Unique unit tag</param>
        /// <param name="unit">The structure, if found</param>
        /// <returns>True if the structure could be found</returns>
        bool TryGetStructureSelf(ulong tag, out IUnit unit);

        /// <summary>
        /// Get all structures owned by participant.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUnit> StructuresSelf();

        /// <summary>
        /// Get unit controlled by participant, with given tag.
        /// Units does not include structures and workers.
        /// </summary>
        /// <param name="tag">Unique unit tag</param>
        /// <param name="unit">The unit, if found</param>
        /// <returns>True if the unit could be found</returns>
        bool TryGetUnitSelf(ulong tag,out IUnit unit);

        /// <summary>
        /// Get all units owned by participant.
        /// Units does not include structures and workers.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUnit> UnitsSelf();

        /// <summary>
        /// Get worker controlled by participant, with given tag.
        /// </summary>
        /// <param name="tag">Unique unit tag</param>
        /// <param name="unit">The worker, if found</param>
        /// <returns>True if the worker could be found</returns>
        bool TryGetWorkerSelf(ulong tag,out IUnit unit);

        /// <summary>
        /// Get worker, structure or unit with given tag.
        /// </summary>
        /// <param name="tag">Unique unit tag</param>
        /// <param name="unit">The unit, if found</param>
        /// <returns>True if the unit could be found</returns>
        bool TryGet(ulong tag, out IUnit unit);

        /// <summary>
        /// Get all workers owned by participant.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUnit> WorkersSelf();

        /// <summary>
        /// Return all structures controlled by the participant with one of the given unit types.
        /// </summary>
        /// <param name="types">Unique Blizzard unit type ids</param>
        /// <returns></returns>
        IEnumerable<IUnit> StructuresSelf(params uint[] types);

        /// <summary>
        /// Return all units controlled by the participant with one of the given unit types.
        /// </summary>
        /// <param name="types">Unique Blizzard unit type ids</param>
        /// <returns></returns>
        IEnumerable<IUnit> UnitsSelf(params uint[] types);

        /// <summary>
        /// Get destructible unit (neutrail)
        /// </summary>
        /// <param name="tag">Unique unit tag</param>
        /// <param name="unit">Unit if exist</param>
        /// <returns>True if destructible with tag exist</returns>
        bool TryGetDestructible(ulong tag,out IUnit unit);

        /// <summary>
        /// Get all destructibles.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUnit> Destructibles();

        /// <summary>
        /// Get all (detected) enemy structures.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUnit> StructuresEnemy();

        /// <summary>
        /// Get all (detected) enemy units (not including workers and structures).
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUnit> UnitsEnemy();

        /// <summary>
        /// Get all (detected) enemy workers.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUnit> WorkersEnemy();

        /// <summary>
        /// Get all visible (in this game loop) enemy structures.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUnit> StructuresEnemyVisible { get; }

        /// <summary>
        /// Get all visible (in this game loop) enemy units (not including workers and structures).
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUnit> UnitsEnemyVisible { get; }

        /// <summary>
        /// Get all visible (in this game loop) enemy workers.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IUnit> WorkersEnemyVisible { get; }

        /// <summary>
        /// Get primary colony - the stating location of the participant
        /// </summary>
        IColony PrimaryColony { get; }

        /// <summary>
        /// Get all colonies - (clusters of minerals, vespene and their optimal headquarter point)
        /// </summary>
        IEnumerable<IColony> Colonies { get; }

        /// <summary>
        /// Get all minerals field (excluding depleted)
        /// </summary>
        IEnumerable<IUnit> MineralFields { get; }

        /// <summary>
        /// Get all vespene geysers (including depleted)
        /// </summary>
        IEnumerable<IUnit> VespeneGeysers { get; }

        /// <summary>
        /// Get current production queue (buildings being produced and
        /// </summary>
        IEnumerable<UnitTypeData> ProductionQueue { get; set; }
    }
    /// <summary>
    /// Type of events that the IntelManager can notify listeners of.
    /// </summary>
    public enum Case { UnitAddedSelf, UnitAddedEnemy, UnitDestroyed, AddedHiddenEnemy, WorkerAddedSelf, WorkerAddedEnemy, WorkerDestroyed, StructureAddedSelf, StructureAddedEnemy, StructureDestroyed, MineralDepleted }

}