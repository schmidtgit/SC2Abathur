using Abathur.Core.Combat;
using Abathur.Model;
using Abathur.Modules;
using NydusNetwork.API.Protocol;

namespace Abathur.Core {
   public interface ICombatManager : IModule {

        /// <summary>
        /// Add a micro controller to handle combat situations for a specific unit type.
        /// </summary>
        /// <param name="unitType">Unit type id</param>
        /// <param name="microController">Microcontroller</param>
        void AddMicroController(uint unitType,IMicroController microController);

       /// <summary>
       /// Order squad to move to the specified point on the map.
       /// </summary>
       /// <param name="squad">The squad being ordered to move</param>
       /// <param name="point">The point to which the squad should move</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void Move(Squad squad, Point2D point, bool queue = false);

       /// <summary>
       /// Order the unit with tag unit to move to the point indicated by point.
       /// </summary>
       /// <param name="unit">The tag of the unit being ordered to move</param>
       /// <param name="point">The point to which the unit should move</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void Move(ulong unit, Point2D point, bool queue = false);

       /// <summary>
       /// Order squad to attack-move to point.
       /// </summary>
       /// <param name="squad">The squad that should attack move</param>
       /// <param name="point">The point towards which the squad attack moves</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void AttackMove(Squad squad, Point2D point, bool queue = false);

       /// <summary>
       /// Order unit to attack-move towards point.
       /// </summary>
       /// <param name="unit">Tag of the unit being ordered to attack move</param>
       /// <param name="point">The point towards which the unit attack moves</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void AttackMove(ulong unit, Point2D point, bool queue = false);

       /// <summary>
       /// Order unit with tag sourceUnit to attack unit with tag targetUnit.
       /// </summary>
       /// <param name="sourceUnit">The tag of the unit being ordered to attack</param>
       /// <param name="targetUnit">The tag of the unit to attack</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void Attack(ulong sourceUnit, ulong targetUnit, bool queue = false);

       /// <summary>
       /// Order squad to attack unit with tag.
       /// </summary>
       /// <param name="squad">The squad being ordered to attack</param>
       /// <param name="targetUnit">The tag of the unit to attack</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void Attack(Squad squad, ulong targetUnit, bool queue = false);

       /// <summary>
       /// Make squad use ability with id abilityId on unit with tag targetUnit.
       /// </summary>
       /// <param name="abilityId">Id of the ability to use</param>
       /// <param name="squad">The squad that should use the ability</param>
       /// <param name="targetUnit">The tag of the unit to use the ability on</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void UseTargetedAbility(int abilityId, Squad squad, ulong targetUnit, bool queue = false);

       /// <summary>
       /// Make sourceUnit use ability with Id abilityId on targetUnit.
       /// </summary>
       /// <param name="abilityId">Id of ability to be used</param>
       /// <param name="sourceUnit">The unit ordered to use the ability</param>
       /// <param name="targetUnit">The unit the ability should be used on</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void UseTargetedAbility(int abilityId,ulong sourceUnit, ulong targetUnit,bool queue = false);

       
        /// <summary>
       /// Make unit with tag unit use ability with id abilityId on point.
       /// </summary>
       /// <param name="abilityId">The Id of the ability to use</param>
       /// <param name="unit">Tag of the unit that should use the ability</param>
       /// <param name="point">The point on which the ability should be cast</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void UsePointCenteredAbility(int abilityId, ulong unit, Point2D point, bool queue = false);

       /// <summary>
       /// Makee squad use ability with id abilityId on point.
       /// </summary>
       /// <param name="abilityId">Id of the ability to use</param>
       /// <param name="squad">The squad that should use the ability</param>
       /// <param name="point">The point on which the ability should be used</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void UsePointCenteredAbility(int abilityId, Squad squad, Point2D point, bool queue = false);

       /// <summary>
       /// Make unit with tag unit use targetless ability with id abilityId.
       /// </summary>
       /// <param name="abilityId">Id of ability to be used</param>
       /// <param name="unit">Tag of unit that should use the ability</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void UseTargetlessAbility(int abilityId, ulong unit, bool queue = false);

       /// <summary>
       /// Make squad use tagetless ability with id abilityID.
       /// </summary>
       /// <param name="abilityId">id of ability to use</param>
       /// <param name="squad">The squad which should use the ability</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void UseTargetlessAbility(int abilityId, Squad squad, bool queue = false);

       /// <summary>
       /// Make unit move towards point using its microcontroller or default move.
       /// </summary>
       /// <param name="unit">The unit that should move</param>
       /// <param name="point">The point towards which the unit should move</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void SmartMove(IUnit unit, Point2D point, bool queue = false);

       /// <summary>
       /// Make squad move towards point using its designated squad controller, the units individual microcontrollers or default move
       /// </summary>
       /// <param name="squad">The squad which should move</param>
       /// <param name="point">The point towards which the squad should move</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void SmartMove(Squad squad, Point2D point, bool queue = false);

       /// <summary>
       /// Make unit attack move towards point using designated microcontrollers or default to regular attack move if the unittype has no microcontroller
       /// </summary>
       /// <param name="unit">The unit that should attac move</param>
       /// <param name="point">The point towards which the unit should attack move</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void SmartAttackMove(IUnit unit, Point2D point, bool queue = false);

       /// <summary>
       /// Make squad attack move towards point using its designated squadcontroller or if none is specified the
       /// units individual microcontrollers or if the unittypes have no microcontrollers the default attack move
       /// </summary>
       /// <param name="squad">The squad that should attack move</param>
       /// <param name="point">The point towards which the squad should attack move</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void SmartAttackMove(Squad squad, Point2D point, bool queue = false);

       /// <summary>
       /// Make sourceUnit attack targetUnit using its designated microcontroller or default attack
       /// </summary>
       /// <param name="sourceUnit">The unit being ordered to attack</param>
       /// <param name="targetUnit">The tag of the unit to attack</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void SmartAttack(IUnit sourceUnit, ulong targetUnit, bool queue = false);

       /// <summary>
       /// Order squad to attack unit with tag enemy using its designated squadcontroller, the units individual microcontrollers or default attack
       /// </summary>
       /// <param name="squad">The squad being ordered to attack</param>
       /// <param name="enemy">The tag of the enemy unit to attac</param>
       /// <param name="queue">Should the order be executed after existing ones(true) or overrule previous orders(false)?</param>
        void SmartAttack(Squad squad, ulong enemy, bool queue = false);
   }
}
