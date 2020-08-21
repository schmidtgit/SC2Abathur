using NydusNetwork.API.Protocol;
using System.Linq;
using System.Collections.Generic;
using Abathur.Modules;
using Abathur.Core.Intel;
using Abathur.Constants;
using Abathur.Repositories;
using Abathur.Model;
using NydusNetwork.Logging;
using Abathur.Extensions;

namespace Abathur.Core.Production
{
    public class ProductionManager : IProductionManager {
        private ITechTree techTree;
        private List<ProductionOrder> _orders;
        private Ressources reservedRessources, currentRessources;
        private ILogger log;
        private IRawManager rawManager;
        private IIntelManager intelManager;
        private IUnitTypeRepository unitTypeRepository;
        private IUpgradeRepository upgradeRepository;

        public ProductionManager(IRawManager rawManager,IIntelManager intelManager,IUnitTypeRepository unitTypeRepository,ITechTree techTree,IUpgradeRepository upgradeRepository,ILogger logger) {
            this.rawManager = rawManager;
            this.intelManager = intelManager;
            this.unitTypeRepository = unitTypeRepository;
            this.techTree = techTree;
            this.log = logger;
            this.upgradeRepository = upgradeRepository;
        }

        private bool RefineryNeeded(IEnumerable<ProductionOrder> queue,UpgradeData tech) => tech.VespeneCost != 0 && RefineryNeeded(queue);
        private bool RefineryNeeded(IEnumerable<ProductionOrder> queue,UnitTypeData unit) => unit.VespeneCost != 0 && RefineryNeeded(queue);
        private bool RefineryNeeded(IEnumerable<ProductionOrder> queue) => !techTree.HasUnit(GameConstants.RaceRefinery) && !IsUnitQueued(queue,GameConstants.RaceRefinery);
        private bool IsQueued(IEnumerable<ProductionOrder> queue,UnitTypeData unit) => IsUnitQueued(queue,unit.UnitId);
        private bool IsQueued(IEnumerable<ProductionOrder> queue,UpgradeData upgrade) => IsResearchQueued(queue,upgrade.UpgradeId);
        private bool IsUnitQueued(IEnumerable<ProductionOrder> queue,uint id) {
            foreach(var order in queue)
                if(order.Type != ProductionOrder.BuildType.Research && order.Unit.UnitId == id)
                    return true;
            return false;
        }
        private bool IsResearchQueued(IEnumerable<ProductionOrder> queue,uint id) {
            foreach(var order in queue)
                if(order.Type == ProductionOrder.BuildType.Research && order.Research.UpgradeId == id)
                    return true;
            return false;
        }

        private List<ProductionOrder> AddRequirementsToList(List<ProductionOrder> queue,ProductionOrder order,bool canSkip) {
            var requiredUnits = new List<UnitTypeData>();
            var requiredTech = new List<UpgradeData>();
            if(order.Type == ProductionOrder.BuildType.Research) {
                var tech = techTree.GetRequiredResearch(order.Research);
                if(tech != null && !techTree.HasResearch(tech) && !IsQueued(queue,tech))
                    requiredTech.Add(tech);
                var unit = techTree.GetProducer(order.Research);
                if(unit != null && !techTree.HasUnit(unit) && !IsQueued(queue,unit))
                    requiredUnits.Add(unit);
                unit = techTree.GetRequiredBuilding(order.Research);
                if(unit != null && !techTree.HasUnit(unit) && !IsQueued(queue,unit))
                    requiredUnits.Add(unit);
            } else {
                var units = techTree.GetProducers(order.Unit);
                if(units.Any() && !units.Any(unit => techTree.HasUnit(unit) || IsQueued(queue,unit)))
                    requiredUnits.Add(units.First());
                units = techTree.GetRequiredBuildings(order.Unit);
                if(units.Any() && !units.Any(unit => techTree.HasUnit(unit) || IsQueued(queue,unit)))
                    requiredUnits.Add(units.First());
                if(order.RequiredAddOn != 0 && !(techTree.HasUnit(order.RequiredAddOn) || IsUnitQueued(queue,order.RequiredAddOn)))
                    requiredUnits.Add(unitTypeRepository.Get(order.RequiredAddOn));
            }
            if(requiredUnits.Count == 0 && requiredTech.Count == 0)
                return queue;
#if DEBUG
            log.LogWarning($"ProductionManager: The queued {order.Unit?.Name}{order.Research?.Name} have unmet requirements - adding them to queue:");
#endif
            requiredUnits.ForEach(u => {
#if DEBUG
                log.LogWarning($"\t{order.Unit?.Name}{order.Research?.Name} => {u.Name} (Structure/Unit)");
#endif
                QueueRequirementsUnit(queue,u,canSkip,"\t\t");
            });
            requiredTech.ForEach(t => {
#if DEBUG
                log.LogWarning($"\t{order.Unit?.Name}{order.Research?.Name} => {t.Name} (Research)");
#endif
                QueueRequirementsTechnology(queue,t,canSkip,"\t\t");
            });
            return queue;
        }

        private void QueueRequirementsTechnology(List<ProductionOrder> queue,UpgradeData tech,bool canSkip,string prefix = "\t") {
            if(RefineryNeeded(queue,tech)) {
#if DEBUG
                log.LogWarning($"{prefix}{tech.Name} => Refinery/Assimilator/Extractor ({tech.VespeneCost} vespene cost, no production)");
#endif
                QueueRequirementsUnit(queue,unitTypeRepository.Get(GameConstants.RaceRefinery),canSkip,prefix + "\t");
            }

            var requiredResearch = techTree.GetRequiredResearch(tech);
            if(requiredResearch != null && !techTree.HasResearch(requiredResearch) && !IsQueued(queue,requiredResearch)) {
#if DEBUG
                log.LogWarning($"{prefix}{tech.Name} => {requiredResearch.Name} (Required Research)");
#endif
                QueueRequirementsTechnology(queue,requiredResearch,canSkip,prefix + "\t");
            }

            var requiredProducer = techTree.GetProducer(tech);
            if(!techTree.HasUnit(requiredProducer) && !IsQueued(queue,requiredProducer)) {
#if DEBUG
                log.LogWarning($"{prefix}{tech.Name} => {requiredProducer.Name} (Producer)");
#endif
                QueueRequirementsUnit(queue,requiredProducer,canSkip,prefix + "\t");
            }

            var requiredBuilding = techTree.GetRequiredBuilding(tech);
            if(requiredBuilding != null && !techTree.HasUnit(requiredBuilding) && !IsQueued(queue,requiredBuilding)) {
#if DEBUG
                log.LogWarning($"{prefix}{tech.Name} => {requiredProducer.Name} (Required Structure)");
#endif
                QueueRequirementsUnit(queue,requiredProducer,canSkip,prefix + "\t");
            }
            queue.Add(new ProductionOrder { Research = tech,LowPriority = canSkip });
        }

        private void QueueRequirementsUnit(List<ProductionOrder> queue,UnitTypeData unit,bool canSkip,string prefix = "\t") {
            if(RefineryNeeded(queue,unit)) {
#if DEBUG
                log.LogWarning($"{prefix}{unit.Name} => Refinery/Assimilator/Extractor ({unit.VespeneCost} vespene cost, no production)");
#endif
                QueueRequirementsUnit(queue,unitTypeRepository.Get(GameConstants.RaceRefinery),canSkip,prefix + "\t");
            }

            var requiredProducers = techTree.GetProducers(unit);
            if(requiredProducers.Any() && !requiredProducers.Any(r => techTree.HasUnit(r) || IsQueued(queue,r))) {
#if DEBUG
                log.LogWarning($"{prefix}{unit.Name} => {requiredProducers.First().Name} (Producer)");
#endif
                QueueRequirementsUnit(queue,requiredProducers.First(),canSkip,prefix + "\t");
            }

            var requiredBuildings = techTree.GetRequiredBuildings(unit);
            if(requiredBuildings.Any() && !requiredBuildings.Any(r => techTree.HasUnit(r) || IsQueued(queue,r))) {
#if DEBUG
                log.LogWarning($"{prefix}{unit.Name} => {requiredBuildings.First().Name} (Producer)");
#endif
                QueueRequirementsUnit(queue,requiredBuildings.First(),canSkip,prefix + "\t");
            }

            queue.Add(new ProductionOrder { Unit = unit,LowPriority = canSkip });
        }


        private bool IsStructure(UnitTypeData unit) => unit.Attributes.Contains(Attribute.Structure);
        private bool IsStructure(uint id) => IsStructure(unitTypeRepository.Get(id));
        private IUnit GetBuilding(ulong tag) {
            intelManager.TryGetStructureSelf(tag,out var unit);
            return unit;
        }

        private bool ReserveRessource(Ressources cost,bool reserve = false) {
            currentRessources -= cost;
            if(reserve)
                reservedRessources += cost;
            return true;
        }

        private bool ReserveRessource(UpgradeData upgrade)
            => ReserveRessource(new Ressources { Minerals = (int)upgrade.MineralCost,Vespene = (int)upgrade.VespeneCost,Supply = 0 },true);
        private bool ReserveRessource(UnitTypeData unit,bool reserve = true)
            => ReserveRessource(new Ressources { Minerals = (int)unit.MineralCost,Vespene = (int)unit.VespeneCost,Supply = (int)unit.FoodRequired },reserve);

        private Action RawCommand(uint ability,ulong tag,ulong target) =>
            new Action { ActionRaw = new ActionRaw { UnitCommand = new ActionRawUnitCommand { AbilityId = (int)ability,UnitTags = { tag },TargetUnitTag = target,QueueCommand = false } } };
        private Action RawCommand(uint ability,ulong tag,Point2D target) =>
            new Action { ActionRaw = new ActionRaw { UnitCommand = new ActionRawUnitCommand { AbilityId = (int)ability,UnitTags = { tag },TargetWorldSpacePos = target,QueueCommand = false } } };
        private Action RawCommand(uint ability,ulong target) =>
            new Action { ActionRaw = new ActionRaw { UnitCommand = new ActionRawUnitCommand { AbilityId = (int)ability,UnitTags = { target },QueueCommand = false } } };

        private IUnit GetWorker(IPosition position) {
            position = position ?? intelManager.PrimaryColony;
            var worker = position.GetClosest(intelManager.WorkersSelf().Where(o => o.Orders.Count == 0));
            if(worker != null) {
                worker.Orders.Add(new UnitOrder { }); // Add decoy order to prevent it from being used this frame.
                return worker;
            }
            var minerals = intelManager.MineralFields.Select(mf => mf.Tag);
            return worker = position.GetClosest(intelManager.WorkersSelf().Where(o => o.Orders.Count == 1 && minerals.Contains(o.Orders.First().TargetUnitTag)));
        }

        private bool ProduceVespene(ProductionOrder order) {
            var colony = order.Position == null ? intelManager.PrimaryColony : order.Position.GetClosest(intelManager.Colonies);
            var refineries = colony.Structures.Where(b => GameConstants.IsVespeneGeyserBuilding(b.UnitType));
            var target = colony.Vespene.FirstOrDefault(v => !refineries.Any(r => r.Pos.X == v.Pos.X && r.Pos.Y == v.Pos.Y));
            var worker = GetWorker(target);
#if DEBUG
            if(target == null) {
                log.LogError($"ProductionManager: No available vespene geyser for {order.Unit.Name} at colony {colony.Id}");
                return false;
            }
            if(worker == null) {
                log.LogWarning($"ProductionManager: No worker found for {order.Unit.Name}");
                return false;
            }
            if(!ReserveRessource(order.Unit))
                return false;
#else
            if(target == null || worker == null || !ReserveRessource(order.Unit)) return false;
#endif
            order.Status = ProductionOrder.OrderStatus.Commissioned;
            order.AssignedUnit = worker;
            rawManager.QueueActions(RawCommand(order.Unit.AbilityId,worker.Tag,target.Tag));
            return true;
        }
        private bool ProduceBuilding(ProductionOrder order) {
            IPosition targetPosition;
            if(order.Position != null && intelManager.GameMap.ValidPlacement(order.Unit,order.Position,order.Spacing))
                targetPosition = order;
            else
                targetPosition = intelManager.GameMap.FindPlacement(order.Unit,order.Position ?? intelManager.PrimaryColony.Point,order.Spacing);
            var worker = GetWorker(targetPosition);
            if(worker == null || targetPosition == null)
                return false;
            if(!ReserveRessource(order.Unit))
                return false;
            order.Status = ProductionOrder.OrderStatus.Commissioned;
            order.AssignedUnit = worker;
            intelManager.GameMap.Reserve(order.Unit,targetPosition.Point);
            rawManager.QueueActions(RawCommand(order.Unit.AbilityId,worker.Tag,targetPosition.Point));
            return true;
        }
        private bool ProduceUnit(ProductionOrder order) {
            var ids = techTree.GetProducersID(order.Unit);
            IUnit producer = null;
            if(GameConstants.RequiresTechLab(order.Unit.UnitId))
                producer = intelManager.StructuresSelf(ids).FirstOrDefault(b => b.AddOnTag != 0 && b.Orders.Count == 0 && GameConstants.IsTechLab(GetBuilding(b.AddOnTag).UnitType));
            else
                producer = intelManager.StructuresSelf(ids).FirstOrDefault(b => b.BuildProgress == 1f && ((b.Orders.Count == 0)
                || (b.AddOnTag != 0 && GameConstants.IsReactor(GetBuilding(b.AddOnTag).UnitType) && b.Orders.Count < 2 && !GameConstants.IsReactorAbility(b.Orders.First().AbilityId))));
            if(producer == null || !ReserveRessource(order.Unit,false))
                return false;
            producer.Orders.Add(new UnitOrder()); // Fake order to prevent adding multiple units in same game loop 
            order.Status = ProductionOrder.OrderStatus.Commissioned;
            rawManager.QueueActions(RawCommand(order.Unit.AbilityId,producer.Tag));
            return true;
        }

        private bool ProducedMorphed(ProductionOrder order) {
            IUnit target = null;
            var ids = techTree.GetProducersID(order.Unit);
            if(IsStructure(ids.First()))
                target = intelManager.StructuresSelf(ids).FirstOrDefault(u => u.Orders.Count == 0);
            else
                target = intelManager.UnitsSelf(ids).FirstOrDefault(u => u.Orders.Count == 0);
            if(target == null || !ReserveRessource(order.Unit,false))
                return false;
            ;
            target.Orders.Add(new UnitOrder { });
            if(target.UnitType == BlizzardConstants.Unit.Larva)
                order.Status = ProductionOrder.OrderStatus.Commissioned;
            else {
                order.Status = ProductionOrder.OrderStatus.Producing;
                order.OrderedUnit = target;
            }
            order.AssignedUnit = target;
            rawManager.QueueActions(RawCommand(order.Unit.AbilityId,order.AssignedUnit.Tag));
            return true;
        }

        private bool ProduceAddOn(ProductionOrder order) {
            var Structure2x2 = unitTypeRepository.Get(BlizzardConstants.Unit.SupplyDepot); // Use as add-on substitute, as add-ons have a size of 8!
            var ids = techTree.GetProducersID(order.Unit);
            var targets = intelManager.StructuresSelf(ids).Where(b =>
                b.AddOnTag == 0 &&
                b.BuildProgress == 1f &&
                b.Orders.Count == 0);
            var target = targets.FirstOrDefault(b => intelManager.GameMap.ValidPlacement(Structure2x2,new Point2D { X = b.Pos.X + 2.5f,Y = b.Pos.Y - 0.5f }));

            if(target == null && targets.Count() != 0)
                return LiftOff(order,targets.First());

            if(target == null || !ReserveRessource(order.Unit,false))
                return false;
            order.Status = ProductionOrder.OrderStatus.Commissioned;
            order.AssignedUnit = target;
            rawManager.QueueActions(RawCommand(order.Unit.AbilityId,order.AssignedUnit.Tag));
            target.Orders.Add(new UnitOrder { });
            return true;
        }

        private bool LiftOff(ProductionOrder order,IUnit target) {
            var position = intelManager.GameMap.FindPlacementWithAddOn(target.Point);
            if(position == null || !ReserveRessource(order.Unit,false))
                return false;
#if DEBUG
            log.LogWarning($"ProductionManager: Relocating {target.Tag} to X:{position.Point.X} Y:{position.Point.Y} ({order.Unit.Name})");
#endif
            order.Status = ProductionOrder.OrderStatus.Commissioned;
            order.AssignedUnit = target;
            var Structure2x2 = unitTypeRepository.Get(BlizzardConstants.Unit.SupplyDepot); // Use as add-on substitute, as add-ons have a size of 8!
            intelManager.GameMap.Reserve(order.Unit,position.Point);
            intelManager.GameMap.Reserve(Structure2x2,new Point2D { X = position.Point.X + 2.5f,Y = position.Point.Y - 0.5f });
            rawManager.QueueActions(RawCommand(order.Unit.AbilityId,order.AssignedUnit.Tag,position.Point));
            target.Orders.Add(new UnitOrder { });
            return true;
        }

        private bool ResearchUpgrade(ProductionOrder order) {
            IUnit target = intelManager.StructuresSelf(techTree.GetProducer(order.Research).UnitId).FirstOrDefault(b => b.BuildProgress == 1f && b.Orders.Count == 0);
            if(target == null || !ReserveRessource(order.Research))
                return false;
            order.Status = ProductionOrder.OrderStatus.Built; //TODO: Not built... producing
            rawManager.QueueActions(RawCommand(order.Research.AbilityId,target.Tag));
            return true;
        }

        private bool CanAfford(ProductionOrder order) {
            if(order.Unit?.FoodRequired != 0 && order.Unit?.FoodRequired > currentRessources.Supply)
                return false;
            var minerals = order.Unit == null ? order.Research.MineralCost : order.Unit.MineralCost;
            if(minerals != 0 && minerals > currentRessources.Minerals)
                return false;
            var vespene = order.Unit == null ? order.Research.VespeneCost : order.Unit.VespeneCost;
            if(vespene != 0 && vespene > currentRessources.Vespene)
                return false;
            return true;
        }

        private bool HasRequirements(ProductionOrder order) {
            if(order.RequiredAddOn != 0 && !techTree.HasUnit(order.RequiredAddOn))
                return false;
            if(order.Unit != null && !techTree.HasProducer(order.Unit))
                return false;
            if(order.Research != null && !techTree.HasProducer(order.Research))
                return false;
            var requiredBuildings = order.Unit == null ? new[] { techTree.GetProducer(order.Research) } : techTree.GetRequiredBuildings(order.Unit);
            if(requiredBuildings.Any() && !requiredBuildings.Any(b => techTree.HasUnit(b)))
                return false;
            var requiredResearch = order.Research == null ? null : techTree.GetRequiredResearch(order.Research);
            if(requiredResearch != null && !techTree.HasResearch(requiredResearch))
                return false;
            return true;
        }

        private bool Produce(ProductionOrder order) {
            if(!CanAfford(order))
                return false;
            if(!HasRequirements(order))
                return false;
            switch(order.Type) {
                case ProductionOrder.BuildType.Structure:
                    if(order.Unit.UnitId == GameConstants.RaceRefinery)
                        return ProduceVespene(order);
                    else
                        return ProduceBuilding(order);
                case ProductionOrder.BuildType.Unit:
                    return ProduceUnit(order);
                case ProductionOrder.BuildType.Morphed:
                    return ProducedMorphed(order);
                case ProductionOrder.BuildType.AddOn:
                    return ProduceAddOn(order);
                case ProductionOrder.BuildType.Research:
                    return ResearchUpgrade(order);
                default:
                    throw new System.NotImplementedException();
            }
        }

        private void GameStep() {
            currentRessources = new Ressources(intelManager.Common);
            currentRessources -= reservedRessources;
            lock(_orders) {
                int i = 0;
                while(i < _orders.Count) {
                    var order = _orders[i];
                    switch(order.Status) {
                        case ProductionOrder.OrderStatus.Queued:
                            if(!Produce(order) && !order.LowPriority)
                                currentRessources -= order;
                            break;
                        case ProductionOrder.OrderStatus.Producing:
                            if(order.OrderedUnit?.BuildProgress >= 1f - float.Epsilon)
                                order.Status = ProductionOrder.OrderStatus.Built;
                            break;
                        case ProductionOrder.OrderStatus.Commissioned: //TODO: Detect commisionned bugs - eg. dead worker
                            break;
                        case ProductionOrder.OrderStatus.Built:
                            _orders.RemoveAt(i);
                            continue;
                        default:
                            throw new System.NotImplementedException();
                    }
                    i++;
                }
            }
        }

        private void MoveUnit(IUnit unit,Point2D point) {
            var command = new ActionRawUnitCommand {
                AbilityId = BlizzardConstants.Ability.Move,
                QueueCommand = true,
                TargetWorldSpacePos = point,
                UnitTags = { unit.Tag }
            };
            rawManager.QueueActions(new Action { ActionRaw = new ActionRaw { UnitCommand = command } });
        }

        private void EventUnitAdded(IUnit unit) {
            lock(_orders) {
                foreach(var o in _orders) {
                    if(o.Unit?.UnitId != unit.UnitType)
                        continue;
                    if(o.Status != ProductionOrder.OrderStatus.Commissioned)
                        continue;
                    switch(o.Type) {
                        case ProductionOrder.BuildType.AddOn:
                            o.Status = ProductionOrder.OrderStatus.Producing;
                            o.OrderedUnit = unit;
                            return;
                        case ProductionOrder.BuildType.Structure:
                            o.Status = ProductionOrder.OrderStatus.Producing;
                            o.OrderedUnit = unit;
                            reservedRessources -= o.Unit;
                            return;
                        case ProductionOrder.BuildType.Unit:
                            if(o.Position != null)
                                MoveUnit(unit,o.Position);
                            o.Status = ProductionOrder.OrderStatus.Built;
                            return;
                        case ProductionOrder.BuildType.Morphed:
                            if(!IsStructure(o.Unit) && o.Position != null)
                                MoveUnit(unit,o.Position);
                            o.Status = ProductionOrder.OrderStatus.Built;
                            return;
                        case ProductionOrder.BuildType.Unknown:
                        case ProductionOrder.BuildType.Research:
                            throw new System.FormatException("Unexpected type!");
                    }
                }
            }
        }

        private bool GetOrderOfType(List<ProductionOrder> orders,ProductionOrder order,out ProductionOrder moved) {
            if(order.Type == ProductionOrder.BuildType.Research)
                moved = orders.Where(o => o.Research?.UpgradeId == order.Research.UpgradeId).FirstOrDefault();
            else
                moved = orders.Where(o => o.Unit?.UnitId == order.Unit.UnitId).FirstOrDefault();
            return moved != null;
        }

        private void QueueOrderInFront(ProductionOrder order) {
#if DEBUG
            if(order.Unit != null && order.Unit.Race != GameConstants.ParticipantRace) {
                log.LogError($"ProductionManager: {order.Unit.Name} is a {order.Unit.Race} unit, playing as {GameConstants.ParticipantRace}!");
                return;
            }
#else
            if(order.Unit != null && order.Unit.Race != GameConstants.ParticipantRace) return;
#endif
            var requirements = new List<ProductionOrder>();
            AddRequirementsToList(requirements,order,order.LowPriority);
            lock(_orders) {
                for(int i = 0; i < requirements.Count; i++) {
                    if(GetOrderOfType(_orders,requirements[i],out var moved)) {
#if DEBUG
                        log.LogWarning($"\t(Important {order.Unit?.Name}{order.Research?.Name}) => Prioritized {moved.Unit?.Name}{order.Research?.Name} from existing queue.");
#endif
                        _orders.Remove(moved);
                        requirements[i] = moved;
                    }
                }
                var copy = _orders.ToList(); // Clone
                _orders.Clear();
                requirements.ForEach(r => _orders.Add(r));
                _orders.Add(order);
                copy.ForEach(c => _orders.Add(c));
            }
        }

        private void QueueOrder(ProductionOrder order) {
#if DEBUG
            if(order.Unit != null && order.Unit.Race != GameConstants.ParticipantRace) {
                log.LogError($"ProductionManager: {order.Unit.Name} is a {order.Unit.Race} unit, playing as {GameConstants.ParticipantRace}!");
                return;
            }
#else
            if(order.Unit != null && order.Unit.Race != GameConstants.ParticipantRace) return;
#endif
            lock(_orders) {
                AddRequirementsToList(_orders,order,order.LowPriority);
                _orders.Add(order);
            }
        }

        private void QueueUnitImportant(UnitTypeData unit,Point2D position,int spacing) => QueueOrderInFront(new ProductionOrder { Unit = unit,Position = position,LowPriority = false,Spacing = spacing });
        private void QueueTechImportant(UpgradeData upgrade) => QueueOrderInFront(new ProductionOrder { Research = upgrade,LowPriority = false });
        private void QueueUnit(UnitTypeData unit,Point2D position,int spacing,bool lowPriority) => QueueOrder(new ProductionOrder { Unit = unit,Position = position,LowPriority = lowPriority,Spacing = spacing });
        private void QueueTech(UpgradeData upgrade,bool lowPriority) => QueueOrder(new ProductionOrder { Research = upgrade,LowPriority = lowPriority });

        private void OnStructureDestroyed(IUnit unit) {
            if(!_orders.Any(o => o.Unit != null && (techTree.GetProducersID(o.Unit).Contains(unit.UnitType) || techTree.GetRequiredBuildings(o.Unit.UnitId).Contains(unit.UnitType))))
                return;
#if DEBUG
            log?.LogWarning($"ProductionerManager: Lost {unit.UnitType} - requeuing to prevent deadlock.");
#endif
            lock(_orders) {
                var ordersCopy = _orders.ToList();
                _orders.Clear();
                foreach(var o in ordersCopy)
                    QueueOrder(o);
            }
        }


        private void Clear() {
            lock(_orders)
                _orders.Clear();
            reservedRessources = new Ressources();
        }

        void IModule.Initialize() {
            _orders = new List<ProductionOrder>();
            intelManager.ProductionQueue = _orders.Where(o => o.Type != ProductionOrder.BuildType.Research).Select(o => o.Unit);
            reservedRessources = new Ressources();
        }

        void IModule.OnStart() {
            intelManager.Handler.RegisterHandler(Case.StructureAddedSelf,u => EventUnitAdded(u));
            intelManager.Handler.RegisterHandler(Case.UnitAddedSelf,u => EventUnitAdded(u));
            intelManager.Handler.RegisterHandler(Case.WorkerAddedSelf,u => EventUnitAdded(u));
            intelManager.Handler.RegisterHandler(Case.StructureDestroyed,u => OnStructureDestroyed(u));
        }

        void IModule.OnStep() => GameStep();
        void IModule.OnGameEnded() => Clear();
        void IModule.OnRestart() => Clear();
        void IProductionManager.ClearBuildOrder() => Clear();

        void IProductionManager.QueueTech(UpgradeData upgrade,bool lowPriority)
            => QueueTech(upgrade,lowPriority);

        void IProductionManager.QueueTech(uint upgradeId,bool lowPriority)
            => QueueTech(upgradeRepository.Get(upgradeId),lowPriority);

        void IProductionManager.QueueUnit(UnitTypeData unit,Point2D desiredPosition,int spacing,bool lowPriority)
            => QueueUnit(unit,desiredPosition,spacing,lowPriority);

        void IProductionManager.QueueUnit(uint unitId,Point2D desiredPosition,int spacing,bool lowPriority)
            => QueueUnit(unitTypeRepository.Get(unitId),desiredPosition,spacing,lowPriority);

        void IProductionManager.QueueUnitImportant(UnitTypeData unit,Point2D desiredPosition,int spacing)
            => QueueUnitImportant(unit,desiredPosition,spacing);

        void IProductionManager.QueueUnitImportant(uint unitId,Point2D desiredPosition,int spacing)
            => QueueUnitImportant(unitTypeRepository.Get(unitId),desiredPosition,spacing);

        void IProductionManager.QueueTechImportant(UpgradeData upgrade)
            => QueueTechImportant(upgrade);

        void IProductionManager.QueueTechImportant(uint upgradeId)
            => QueueTechImportant(upgradeRepository.Get(upgradeId));

        enum ProductionStatus { InsufficientMinerals, InsufficientVespene, InsufficientSupply, NeedTech, Ready }
    }
}
