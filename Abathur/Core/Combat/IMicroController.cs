using NydusNetwork.API.Protocol;

namespace Abathur.Core.Combat {
    /// <summary>
    /// Used to overwrite default behaviour of specific unit types.
    /// </summary>
    public interface IMicroController {
        /// <summary>
        /// Command unit to move to location.
        /// </summary>
        /// <param name="unit">Tag of unit to command</param>
        /// <param name="point">Location to move to</param>
        /// <param name="queue">False will overwrite current order</param>
        void SmartMove(ulong unit, Point2D point, bool queue = false);

        /// <summary>
        /// Command unit to attack target.
        /// </summary>
        /// <param name="unit">Tag of unit to command</param>
        /// <param name="target">Target to attack</param>
        /// <param name="queue">False will overwrite current order</param>
        void SmartAttack(ulong unit, ulong target, bool queue = false);

        /// <summary>
        /// Command unit to attack location.
        /// </summary>
        /// <param name="unit">Tag of unit to command</param>
        /// <param name="point">Location to move to</param>
        /// <param name="queue">False will overwrite current order</param>
        void SmartAttack(ulong unit, Point2D point, bool queue = false);
    }
}