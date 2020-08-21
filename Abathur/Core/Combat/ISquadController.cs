using NydusNetwork.API.Protocol;

namespace Abathur.Core.Combat {

    /// <summary>
    /// Used to overwrite behaviour of an entire squad.
    /// </summary>
    public interface ISquadController {

        /// <summary>
        /// Command a squad to move to location.
        /// </summary>
        /// <param name="squad">Squad to move</param>
        /// <param name="point">Desired location</param>
        /// <param name="queue">False will overwrite current order</param>
        void SmartMove(Squad squad,Point2D point,bool queue = false);

        /// <summary>
        /// Command a squad to attack unit.
        /// </summary>
        /// <param name="squad">Squad to move</param>
        /// <param name="unit">Tag of unit to attack</param>
        /// <param name="queue">False will overwrite current order</param>
        void SmartAttack(Squad squad,ulong unit,bool queue = false);

        /// <summary>
        /// Command a squad to attack location.
        /// </summary>
        /// <param name="squad">Squad to move</param>
        /// <param name="point">Desired location</param>
        /// <param name="queue">False will overwrite current order</param>
        void SmartAttack(Squad squad,Point2D point,bool queue = false);
    }
}