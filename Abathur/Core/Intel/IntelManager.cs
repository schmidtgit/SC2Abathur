using Abathur.Constants;
using Abathur.Modules;
using NydusNetwork.Services;
using NydusNetwork.API.Protocol;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Abathur.Extensions;
using Attribute = NydusNetwork.API.Protocol.Attribute;
using NydusNetwork.Logging;
using Abathur.Repositories;
using Abathur.Model;
using Abathur.Core.Intel.Map;

namespace Abathur.Core.Intel
{
    /// <summary>
    /// The IntelManager store a representation of the current game state.
    /// It should always be updated as the first module in each game-loop to ensure all modules base decision on the newest availible information.
    /// </summary>
    public class IntelManager : IIntelManager {
        public IEnumerable<UnitTypeData> ProductionQueue{ get; set; }
        public uint GameLoop => GameConstants.GameLoop;
        public GameMap GameMap                                  { get; private set; }
        public Score CurrentScore                               { get; private set; }
        public PlayerCommon Common                              { get; private set; }
        public CaseHandler<Case,IUnit> Handler                  { get; private set; }

        private Dictionary<ulong, IntelUnit> StructuresSelf, WorkersSelf, UnitsSelf;
        private Dictionary<ulong,IntelUnit> StructureEnemy          { get; set; } = new Dictionary<ulong,IntelUnit>();
        private Dictionary<ulong,IntelUnit> WorkersEnemy            { get; set; } = new Dictionary<ulong,IntelUnit>();
        private Dictionary<ulong,IntelUnit> UnitsEnemy              { get; set; } = new Dictionary<ulong,IntelUnit>();
        private IntelColony PrimaryColony                           { get; set; }
        private ICollection<IntelColony> Colonies                   { get; set; } = new Collection<IntelColony>();
        public ICollection<UpgradeData> UpgradesSelf                { get; set; } = new Collection<UpgradeData>();

        private Dictionary<ulong,IntelUnit> MineralFields           { get; set; } = new Dictionary<ulong,IntelUnit>();
        private Dictionary<ulong,IntelUnit> VespeneGeysers          { get; set; } = new Dictionary<ulong,IntelUnit>();
        private Dictionary<ulong,IntelUnit> Destructibles           { get; set; } = new Dictionary<ulong,IntelUnit>();
        private Dictionary<ulong,IntelUnit> XelNagaTowers           { get; set; } = new Dictionary<ulong,IntelUnit>();

#if DEBUG
        private ISet<uint> IgnoredUnitTypes = new HashSet<uint>();
#endif

        IEnumerable<IColony> IIntelManager.Colonies => Colonies;
        IEnumerable<IUnit> IIntelManager.MineralFields => MineralFields.Values;
        IEnumerable<IUnit> IIntelManager.VespeneGeysers => VespeneGeysers.Values;
        IGameMap IIntelManager.GameMap => GameMap;

        private IUnitTypeRepository unitTypeRepository;
        private IUpgradeRepository upgradeRepository;
        private IRawManager rawManager;
        private ILogger log;
        /// <summary>
        /// Module responsible for sorting information from the StarCraft II client and notifying listeners.
        /// </summary>
        /// <param name="gameClient"></param>
        public IntelManager(IRawManager rawManager,ILogger logger,IUnitTypeRepository unitTypeRepository,IUpgradeRepository upgradeRepository, IGameMap map) {
            this.unitTypeRepository = unitTypeRepository;
            this.upgradeRepository = upgradeRepository;
            this.rawManager = rawManager;
            this.GameMap = (GameMap)map;
            log = logger;
        }
        /// <summary>
        /// This function should only be called on the first game frame.
        /// Mineral Fields, Vespene and Destructible is added once and then only updated if visible.
        /// </summary>
        /// <param name="obs">Observation from the newest ResponseObservation</param>
        private void InitialIntel(Observation obs) {
            foreach(var unit in obs.RawData.Units)
                if(GameConstants.IsMineralField(unit.UnitType)) {
                    MineralFields.Add(unit.Tag,new IntelUnit(unit));
                    GameMap.RegisterNatural(new Point { X = unit.Pos.X - 0.5f,Y = unit.Pos.Y },0.5f);
                    GameMap.RegisterStructure(new Point { X = unit.Pos.X - 0.5f,Y = unit.Pos.Y },0.5f);
                    GameMap.RegisterNatural(new Point { X = unit.Pos.X + 0.5f,Y = unit.Pos.Y },0.5f);
                    GameMap.RegisterStructure(new Point { X = unit.Pos.X + 0.5f,Y = unit.Pos.Y },0.5f);
                } else if(GameConstants.IsVepeneGeyser(unit.UnitType)) {
                    VespeneGeysers.Add(unit.Tag,new IntelUnit(unit));
                    GameMap.RegisterNatural(unit.Pos,1.5f);
                    GameMap.RegisterStructure(unit.Pos,1.5f);
                } else if(GameConstants.IsDestructible(unit.UnitType)) {
                    Destructibles.Add(unit.Tag,new IntelUnit(unit));
                    GameMap.RegisterStructure(unit.Pos,0.5f); // TODO - add actual size to destructables
                } else if(GameConstants.IsXelNagaTower(unit.UnitType)) {
                    XelNagaTowers.Add(unit.Tag,new IntelUnit(unit));
                    GameMap.RegisterStructure(unit.Pos, 0.5f);
                }
        }

        private void UpdateIntel(Observation obs) {
            GameConstants.GameLoop = obs.GameLoop;
            GameMap.CreepAndVisibility(obs);

            if(obs.PlayerCommon != null)
                Common = obs.PlayerCommon;
            if(obs.Score != null)
                CurrentScore = obs.Score;
            if(obs.RawData.Player != null)
                if(UpgradesSelf.Count != obs.RawData.Player.UpgradeIds.Count)
                    UpgradesSelf = obs.RawData.Player.UpgradeIds.Select(id => upgradeRepository.Get(id)).ToList();
            DeadUnits(obs.RawData.Event);

            if(obs.RawData != null)
                foreach(var unit in obs.RawData.Units)
                    if(unit.Alliance == Alliance.Self)
                        AddUnitSelf(unit);
                    else if(unit.Alliance == Alliance.Enemy)
                        AddUnitEnemy(unit);
                    else if(unit.DisplayType == DisplayType.Visible)
                        UpdateUnitNeutral(unit);

            DetectEnemyRace();
        }

        /// <summary>
        /// Notify listeners of dead units and remove from the index.
        /// </summary>
        /// <param name="event">Event from the newest ResponseObservation</param>
        private void DeadUnits(Event @event) {
            if(@event == null)
                return;
            foreach(var id in @event.DeadUnits) {
                if(UnitsSelf.TryGetValue(id,out var deadUnit)) {
                    UnitsSelf.Remove(id);
                    Handler.Handle(Case.UnitDestroyed,deadUnit);
                } else if(StructuresSelf.TryGetValue(id,out deadUnit)) {
                    StructuresSelf.Remove(id);
                    Handler.Handle(Case.StructureDestroyed,deadUnit);
                } else if(WorkersSelf.TryGetValue(id,out deadUnit)) {
                    WorkersSelf.Remove(id);
                    Handler.Handle(Case.WorkerDestroyed,deadUnit);
                } else if(UnitsEnemy.TryGetValue(id,out deadUnit)) {
                    UnitsEnemy.Remove(id);
                    Handler.Handle(Case.UnitAddedEnemy,deadUnit);
                } else if(StructureEnemy.TryGetValue(id,out deadUnit)) {
                    StructureEnemy.Remove(id);
                    Handler.Handle(Case.StructureAddedEnemy,deadUnit);
                } else if(WorkersEnemy.TryGetValue(id,out deadUnit)) {
                    WorkersEnemy.Remove(id);
                    Handler.Handle(Case.WorkerAddedEnemy,deadUnit);
                } else if(MineralFields.TryGetValue(id,out deadUnit)) {
                    MineralFields.Remove(id);
                    foreach(var c in Colonies)
                        if(c.Minerals.Remove(deadUnit))
                            break;
                    Handler.Handle(Case.MineralDepleted,deadUnit);
                }
            }
        }

        /// <summary>
        /// Notify listeners and add/update stored allied units.
        /// </summary>
        /// <param name="unit">Unit where Alliance == Self</param>
        private void AddUnitSelf(Unit unit) {
            if(GameConstants.IsWorker(unit.UnitType))
                if(WorkersSelf.ContainsKey(unit.Tag)) {
                    WorkersSelf[unit.Tag].DataSource = unit;
                } else {
                    var data = new IntelUnit(unit);
                    WorkersSelf.Add(unit.Tag,data);
                    Handler.Handle(Case.WorkerAddedSelf,data);
            } else if(IsBuilding(unit.UnitType)) {
                if(StructuresSelf.ContainsKey(unit.Tag)) {
                    StructuresSelf[unit.Tag].DataSource = unit;
                } else {
                    var data = new IntelUnit(unit);
                    StructuresSelf.Add(unit.Tag,data);
                    Handler.Handle(Case.StructureAddedSelf,data);
                }
            } else { // must be unit
                if(UnitsSelf.ContainsKey(unit.Tag)) {
                    UnitsSelf[unit.Tag].DataSource = unit;
                } else {
                    var data = new IntelUnit(unit);
                    UnitsSelf.Add(unit.Tag,data);
                    Handler.Handle(Case.UnitAddedSelf,data);
                }
            }
        }

        private void AddUnitEnemy(Unit unit) {
            var data = new IntelUnit(unit);
            if(GameConstants.IsWorker(unit.UnitType))
                if(WorkersEnemy.ContainsKey(unit.Tag)) {
                    WorkersEnemy[unit.Tag].DataSource = unit;
                } else {
                    WorkersEnemy.Add(unit.Tag,data);
                    Handler.Handle(Case.WorkerAddedEnemy,data);
            } else if(IsBuilding(unit.UnitType)) {
                if(StructureEnemy.ContainsKey(unit.Tag)) {
                    StructureEnemy[unit.Tag].DataSource = unit;
                } else {
                    StructureEnemy.Add(unit.Tag,data);
                    Handler.Handle(Case.StructureAddedEnemy,data);
                }
            } else { // must be unit
                if(UnitsEnemy.ContainsKey(unit.Tag)) {
                    UnitsEnemy[unit.Tag].DataSource = unit;
                } else {
                    UnitsEnemy.Add(unit.Tag,data);
                    Handler.Handle(Case.UnitAddedEnemy,data);
                }
            }
            if(data.DisplayType == DisplayType.Hidden)
                Handler.Handle(Case.AddedHiddenEnemy,data);
        }

        /// <summary>
        /// Update visible neutral units of interest.
        /// </summary>
        /// <param name="unit">Unit where DisplayeType == Visible</param>
        private void UpdateUnitNeutral(Unit unit) {
            if(GameConstants.IsMineralField(unit.UnitType))
                UpdateSnapshotNeutral(MineralFields,unit);
            else if(GameConstants.IsVepeneGeyser(unit.UnitType))
                UpdateSnapshotNeutral(VespeneGeysers,unit);
            else if(GameConstants.IsDestructible(unit.UnitType))
                UpdateSnapshotNeutral(Destructibles,unit);
            else if(GameConstants.IsXelNagaTower(unit.UnitType))
                UpdateSnapshotNeutral(XelNagaTowers,unit);
#if DEBUG    
            else if(IgnoredUnitTypes.Add(unit.UnitType))
                log.LogWarning($"IntelManager IGNORED {unit.UnitType} ({unitTypeRepository.Get(unit.UnitType).Name})");
#endif
        }

        // Method required because snapshots change ID when visible...
        private void UpdateSnapshotNeutral(Dictionary<ulong, IntelUnit> dictionary, Unit unit) {
            if(dictionary.ContainsKey(unit.Tag)) {
                dictionary[unit.Tag].DataSource = unit;
                return;
            }

            IntelUnit data = dictionary.Where(p => p.Value.Pos.X == unit.Pos.X && p.Value.Pos.Y == unit.Pos.Y).Select(p => p.Value).FirstOrDefault();
            if(data == null)
                throw new System.ArgumentNullException($"Could not find neutral unit ({unit.UnitType} at X:{unit.Pos.X} Y:{unit.Pos.Y})");
            
            dictionary.Remove(data.Tag);
            data.DataSource = unit;
            dictionary.Add(data.Tag,data);
        }

        /// <summary>
        /// Set enemy race based on seen units, if not already set.
        /// </summary>
        private void DetectEnemyRace() {
            if(GameConstants.EnemyRace == Race.Random || GameConstants.EnemyRace == Race.NoRace)
                if(UnitsEnemy.Count != 0)
                    GameConstants.EnemyRace = unitTypeRepository.Get(UnitsEnemy.Values.First().UnitType).Race;
                else if(WorkersEnemy.Count != 0)
                    GameConstants.EnemyRace = unitTypeRepository.Get(WorkersEnemy.Values.First().UnitType).Race;
                else if(StructureEnemy.Count != 0)
                    GameConstants.EnemyRace = unitTypeRepository.Get(StructureEnemy.Values.First().UnitType).Race;
        }

        /// <summary>
        /// Clear all stored data.
        /// </summary>
        private void Clear() {
            StructuresSelf.Clear();
            WorkersSelf.Clear();
            UnitsSelf.Clear();
            StructureEnemy.Clear();
            WorkersEnemy.Clear();
            UnitsEnemy.Clear();
            Colonies.Clear();
            PrimaryColony = null;
            UpgradesSelf.Clear();
            MineralFields.Clear();
            VespeneGeysers.Clear();
            Destructibles.Clear();
            XelNagaTowers.Clear();
            Handler = new CaseHandler<Case,IUnit>();
        }
        
        private void RemoveWorkerFromColony(IUnit worker) {
            foreach(var colony in Colonies)
                if(colony.Workers.Remove(worker))
                    return;
        }

        private void AddWorkerSelfToColony(IUnit unit) => unit.GetClosest(Colonies).Workers.Add(unit);
        private void AddStructureSelfToColony(IUnit unit) => unit.GetClosest(Colonies).Structures.Add(unit);

        private void GenerateColonies(StartRaw startRaw) {
            Colonies = MapAnalyser.GetColonies(MineralFields.Values.Cast<IUnit>().ToList(),VespeneGeysers.Values.Cast<IUnit>().ToList(),startRaw.StartLocations);
            // Find Primary Colony
            var mainBase = StructuresSelf.First();
            var primary = (IntelColony) mainBase.Value.Pos.ConvertTo2D().GetClosest(Colonies);
            primary.Workers = WorkersSelf.Select(kvp => (IUnit) kvp.Value).ToList();
            primary.Structures = new List<IUnit> { mainBase.Value };
            PrimaryColony = primary;
        }

        private bool IsBuilding(uint unitType) => unitTypeRepository.Get(unitType).Attributes.Contains(Attribute.Structure);

        void IModule.Initialize() {
            StructuresSelf = new Dictionary<ulong, IntelUnit>();
            WorkersSelf = new Dictionary<ulong,IntelUnit>();
            UnitsSelf = new Dictionary<ulong,IntelUnit>();
            Handler = new CaseHandler<Case,IUnit>();
            Handler.RegisterHandler(Case.WorkerDestroyed,w => RemoveWorkerFromColony(w));
        }
        void IModule.OnStart() {
            if(rawManager.TryWaitRawRequest(NydusNetwork.Model.ClientConstant.RequestGameInfo,out var info,int.MaxValue))
                GameMap.Initialize(info.GameInfo, this);
            if(rawManager.TryWaitObservationRequest(out var obs, GameLoop)) {
                InitialIntel(obs.Observation.Observation);
                UpdateIntel(obs.Observation.Observation);
                if(GameConstants.ParticipantRace == Race.Random || GameConstants.ParticipantRace == Race.NoRace)
                    GameConstants.ParticipantRace = unitTypeRepository.Get(StructuresSelf.Values.FirstOrDefault().UnitType).Race;
            }
            GenerateColonies(info.GameInfo.StartRaw);
            Handler.RegisterHandler(Case.WorkerAddedSelf,u => AddWorkerSelfToColony(u));
            Handler.RegisterHandler(Case.StructureAddedSelf,u => AddStructureSelfToColony(u));
        }

        void IModule.OnStep() {
            if(rawManager.TryWaitObservationRequest(out var response, GameLoop))
                UpdateIntel(response.Observation.Observation);
        }

        void IModule.OnGameEnded() {}
        void IModule.OnRestart() => Clear();

        public bool TryGetStructureSelf(ulong tag,out IUnit unit) {
            var res = StructuresSelf.TryGetValue(tag,out var data);
            unit = data; return res;
        }

        IEnumerable<IUnit> IIntelManager.StructuresSelf() => StructuresSelf.Values;

        public bool TryGetUnitSelf(ulong tag,out IUnit unit) {
            var res = UnitsSelf.TryGetValue(tag,out var data);
            unit = data; return res;
        }

        IEnumerable<IUnit> IIntelManager.UnitsSelf() => UnitsSelf.Values;

        public bool TryGetWorkerSelf(ulong tag,out IUnit unit) {
            var res = WorkersSelf.TryGetValue(tag,out var data);
            unit = data; return res;
        }

        public bool TryGet(ulong tag, out IUnit unit)
        {
            if (UnitsSelf.TryGetValue(tag,out var u))
            {
                unit = u;
                return true;
            }
            if(StructuresSelf.TryGetValue(tag,out var building)) {
                unit = building;
                return true;
            }
            if(WorkersSelf.TryGetValue(tag,out var worker)) {
                unit = worker;
                return true;
            }
            if(StructureEnemy.TryGetValue(tag,out var buildingE)) {
                unit = buildingE;
                return true;
            }
            if(UnitsEnemy.TryGetValue(tag,out var uE)) {
                unit = uE;
                return true;
            }
            if(WorkersEnemy.TryGetValue(tag,out var workerE)) {
                unit = workerE;
                return true;
            }
            if(Destructibles.TryGetValue(tag,out var destructible)) {
                unit = destructible;
                return true;
            }
            if(MineralFields.TryGetValue(tag,out var mineral)) {
                unit = mineral;
                return true;
            }
            if(VespeneGeysers.TryGetValue(tag,out var vespene)) {
                unit = vespene;
                return true;
            }
            unit = null;
            return false;
        }

        IEnumerable<IUnit> IIntelManager.WorkersSelf() => WorkersSelf.Values;

        public bool TryGetDestructible(ulong tag,out IUnit unit) {
            var res = Destructibles.TryGetValue(tag,out var data);
            unit = data; return res;
        }

        IEnumerable<IUnit> IIntelManager.Destructibles() => Destructibles.Values;
        IEnumerable<IUnit> IIntelManager.StructuresEnemy() => StructureEnemy.Values;
        IEnumerable<IUnit> IIntelManager.UnitsEnemy() => UnitsEnemy.Values;
        IEnumerable<IUnit> IIntelManager.WorkersEnemy() => WorkersEnemy.Values;
        IEnumerable<IUnit> IIntelManager.StructuresEnemyVisible => StructureEnemy.Values.Where(u => u.lastSeen == GameLoop);
        IEnumerable<IUnit> IIntelManager.UnitsEnemyVisible => UnitsEnemy.Values.Where(u => u.lastSeen == GameLoop);
        IEnumerable<IUnit> IIntelManager.WorkersEnemyVisible => WorkersEnemy.Values.Where(u => u.lastSeen == GameLoop);
        IColony IIntelManager.PrimaryColony => PrimaryColony;
        IEnumerable<IUnit> IIntelManager.StructuresSelf(params uint[] types) => StructuresSelf.Where(b => types.Contains(b.Value.UnitType)).Select(b => b.Value);
        IEnumerable<IUnit> IIntelManager.UnitsSelf(params uint[] types) => UnitsSelf.Where(b => types.Contains(b.Value.UnitType)).Select(b => b.Value);
    }
}
