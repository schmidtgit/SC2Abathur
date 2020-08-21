using System;
using System.Collections.Generic;
using System.Linq;
using Abathur.Constants;
using Abathur.Model;
using Abathur.Repositories;
using NydusNetwork.API.Protocol;
using Action = NydusNetwork.API.Protocol.Action;

namespace Abathur.Core.Combat {
    public class CombatManager : ICombatManager, ISquadRepository {
        public IList<Squad> Squads { get; set; } =  new List<Squad>();
        public IDictionary<uint,IMicroController> Controllers { get; set; }
        private IIntelManager intelManager;
        private IRawManager rawManager;

        public CombatManager(IIntelManager intelManager, IRawManager rawManager) {
            this.intelManager = intelManager;
            this.rawManager = rawManager;
        }
        public void Initialize() {
            Squads = new List<Squad>();
            Controllers = new Dictionary<uint,IMicroController>();
        }

        public void OnStart() {
            Squads.Clear();
            Action<IUnit> removeDead = u => {
                for(int i = 0; i < Squads.Count; i++) {
                    Squads[i].RemoveUnit(u);
                }
            };
            intelManager.Handler.RegisterHandler(Case.UnitDestroyed,removeDead);
            intelManager.Handler.RegisterHandler(Case.StructureDestroyed,removeDead);
        }

        public void OnStep() {}

        public void OnGameEnded() {}

        public void OnRestart() { Squads.Clear(); }

        /// <inheritdoc />
        public void Move(Squad squad,Point2D point,bool queue = false) {
            foreach (var unit in squad.Units)
            {
                Move(unit.Tag, point, queue);
            }
        }
        /// <inheritdoc />
        public void Move(ulong unit,Point2D point,bool queue = false) {
            var command = new ActionRawUnitCommand {
                AbilityId = BlizzardConstants.Ability.Move,
                QueueCommand = queue,
                TargetWorldSpacePos = point,
                UnitTags = { unit }
            };
            rawManager.QueueActions(new Action { ActionRaw = new ActionRaw { UnitCommand = command } });
        }
        /// <inheritdoc />
        public void AttackMove(Squad squad,Point2D point,bool queue = false) {
            foreach (var unit in squad.Units)
                AttackMove(unit.Tag, point, queue);
        }
        /// <inheritdoc />
        public void AttackMove(ulong unit,Point2D point,bool queue = false) {
            var command = new ActionRawUnitCommand {
                AbilityId = BlizzardConstants.Ability.GeneralAttack,
                QueueCommand = queue,
                TargetWorldSpacePos = point,
                UnitTags = { unit }
            };
            rawManager.QueueActions(new Action { ActionRaw = new ActionRaw { UnitCommand = command } });
        }
        /// <inheritdoc />
        public void Attack(ulong sourceUnit,ulong targetUnit,bool queue = false) {
            var command = new ActionRawUnitCommand {
                AbilityId = BlizzardConstants.Ability.GeneralAttack,
                QueueCommand = queue,
                TargetUnitTag = targetUnit,
                UnitTags = { sourceUnit }
            };
            rawManager.QueueActions(new Action { ActionRaw = new ActionRaw { UnitCommand = command } });
        }
        /// <inheritdoc />
        public void Attack(Squad squad,ulong targetUnit,bool queue = false) {
            foreach (var unit in squad.Units)
            {
                Attack(unit.Tag, targetUnit, queue);
            }
        }
        /// <inheritdoc />
        public void UseTargetedAbility(int abilityId,ulong sourceUnit,ulong targetUnit,bool queue = false) {
            var command = new ActionRawUnitCommand {
                AbilityId = abilityId,
                QueueCommand = queue,
                TargetUnitTag = targetUnit,
                UnitTags = { sourceUnit }
            };
            rawManager.QueueActions(new Action { ActionRaw = new ActionRaw { UnitCommand = command } });
        }
        /// <inheritdoc />
        public void UseTargetedAbility(int abilityId,Squad squad,ulong targetUnit,bool queue = false) {
            foreach (var unit in squad.Units)
            {
                UseTargetedAbility(abilityId, unit.Tag, targetUnit,queue);
            }
        }
        /// <inheritdoc />
        public void UsePointCenteredAbility(int abilityId,ulong unit,Point2D point,bool queue = false) {
            var command = new ActionRawUnitCommand {
                AbilityId = abilityId,
                QueueCommand = queue,
                TargetWorldSpacePos = point,
                UnitTags = { unit }
            };
            rawManager.QueueActions(new Action { ActionRaw = new ActionRaw { UnitCommand = command } });
        }
        /// <inheritdoc />
        public void UsePointCenteredAbility(int abilityId,Squad squad,Point2D point,bool queue = false) {
            foreach (var unit in squad.Units)
            {
                UsePointCenteredAbility(abilityId, unit.Tag, point, queue);
            }
        }

        /// <inheritdoc />
        public void UseTargetlessAbility(int abilityId,ulong unit,bool queue = false) {
            var command = new ActionRawUnitCommand {
                AbilityId = abilityId,
                QueueCommand = queue,
                UnitTags = { unit }
            };
            rawManager.QueueActions(new Action { ActionRaw = new ActionRaw { UnitCommand = command } });
        }
        /// <inheritdoc />
        public void UseTargetlessAbility(int abilityId,Squad squad,bool queue = false) {
            foreach(var unit in squad.Units)
                UseTargetlessAbility(abilityId,unit.Tag,queue);
        }
        /// <inheritdoc />
        public void SmartAttackMove(IUnit unit,Point2D point,bool queue = false) {
            if(Controllers.TryGetValue(unit.UnitType,out var controller)) {
                controller.SmartAttack(unit.Tag,point,queue);
            } else {
                AttackMove(unit.Tag,point,queue);
            }
        }
        /// <inheritdoc />
        public void SmartAttackMove(Squad squad,Point2D point,bool queue = false) {
            if(squad.SquadController != null) {
                squad.SquadController.SmartAttack(squad,point,queue);
            } else {
                foreach(var unit in squad.Units) {
                        if(Controllers.TryGetValue(unit.UnitType,out var controller)) {
                            controller.SmartAttack(unit.Tag,point,queue);
                        } else {
                            AttackMove(unit.Tag,point,queue);
                        }
                }
            }
        }
        /// <inheritdoc />
        public void SmartMove(IUnit unit,Point2D point,bool queue = false) {
            if(Controllers.TryGetValue(unit.UnitType,out var controller)) {
                controller.SmartMove(unit.Tag,point,queue);
            } else {
                AttackMove(unit.Tag,point,queue);
            }
        }
        /// <inheritdoc />
        public void SmartMove(Squad squad,Point2D point,bool queue = false) {
            if(squad.SquadController != null) {
                squad.SquadController.SmartMove(squad,point,queue);
            } else {
                foreach(var unit in squad.Units) {
                        if(Controllers.TryGetValue(unit.UnitType,out var controller)) {
                            controller.SmartMove(unit.Tag,point,queue);
                        } else {
                            Move(unit.Tag,point,queue);
                        }
                    }
                
            }
        }
        /// <inheritdoc />
        public void SmartAttack(IUnit sourceUnit,ulong targetUnit,bool queue = false) {
            if(Controllers.TryGetValue(sourceUnit.UnitType,out var controller)) {
                controller.SmartAttack(sourceUnit.Tag,targetUnit,queue);
            } else {
                Attack(sourceUnit.Tag,targetUnit,queue);
            }
        }
        /// <inheritdoc />
        public void SmartAttack(Squad squad,ulong enemy,bool queue = false) {
            if(squad.SquadController != null) {
                squad.SquadController.SmartAttack(squad,enemy,queue);
            } else {
                foreach(var unit in squad.Units) {
                        if(Controllers.TryGetValue(unit.UnitType,out var controller)) {
                            controller.SmartAttack(unit.Tag,enemy,queue);
                        } else {
                            Attack(unit.Tag,enemy,queue);
                        }
                }
            }
        }

        private Squad Create(string name, ulong id) {
            var s = new Squad() {
                Id = id,
                Name = name
            };
            Squads.Add(s);
            return s;
        }

        private Squad Get(ulong id) => Squads.FirstOrDefault(u => u.Id == id);
        Squad ISquadRepository.Create(string name) => Create(name, (ulong) Squads.Count);
        Squad ISquadRepository.Create(string name, ulong id) => Create(name, id);
        IEnumerable<Squad> ISquadRepository.Get() => Squads;
        Squad ISquadRepository.Get(ulong id) => Squads.FirstOrDefault(u => u.Id == id);
        bool ISquadRepository.Remove(ulong id) => Squads.Remove(Get(id));
        void ISquadRepository.Clear() => Squads.Clear();
        Squad ISquadRepository.Get(string name) => Squads.FirstOrDefault(u => u.Name == name);

        public void AddMicroController(uint unitType,IMicroController microController) {
            if(Controllers.ContainsKey(unitType))
                Controllers[unitType] = microController;
            else
                Controllers.Add(unitType,microController);
        }
    }
}
