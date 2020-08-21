using Abathur.Modules;
using NydusNetwork.API.Protocol;

namespace Abathur.Core {
    /// <summary>
    /// Handles placement of structures and production of units.
    /// </summary>
    public interface IProductionManager : IModule {
        /// <summary>
        /// Clear everything in current production queue
        /// </summary>
        void ClearBuildOrder();

        /// <summary>
        /// Queue a unit or structure and all requirements in front of everything else.
        /// </summary>
        /// <param name="unit">UnitType to queue</param>
        /// <param name="desiredPosition">Point to place structure near or place to send unit after production</param>
        /// <param name="spacing">Required spacing around structure, only used for structures</param>
        void QueueUnitImportant(UnitTypeData unit,Point2D desiredPosition = null,int spacing = 1);

        /// <summary>
        /// Queue a unit or structure and all requirements in front of everything else.
        /// </summary>
        /// <param name="unitId">UnitType to queue by id</param>
        /// <param name="desiredPosition">Point to place structure near or place to send unit after production</param>
        /// <param name="spacing">Required spacing around structure, only used for structures</param>
        void QueueUnitImportant(uint unitId,Point2D desiredPosition = null,int spacing = 1);

        /// <summary>
        /// Queue upgrade and requirements in front of everything else.
        /// </summary>
        /// <param name="upgrade">Upgrade type to queue</param>
        void QueueTechImportant(UpgradeData upgrade);

        /// <summary>
        /// Queue upgrade and requirements in front of everything else.
        /// </summary>
        /// <param name="upgrade">Upgrade type to queue by id</param>
        void QueueTechImportant(uint upgradeId);

        /// <summary>
        /// Place production order and all requirements after all other items in the queue.
        /// </summary>
        /// <param name="unit">UnitTye to queue</param>
        /// <param name="desiredPosition">Point to place structure near or place to send unit after production</param>
        /// <param name="spacing">Required spacing around structure, only used for structures</param>
        /// <param name="lowPriority">Will reserve ressources if not currently affordable if true</param>
        void QueueUnit(UnitTypeData unit, Point2D desiredPosition = null, int spacing = 1,bool lowPriority = false);

        /// <summary>
        /// Place production order and all requirements after all other items in the queue.
        /// </summary>
        /// <param name="unitId">UnitTye to queue by id</param>
        /// <param name="desiredPosition">Point to place structure near or place to send unit after production</param>
        /// <param name="spacing">Required spacing around structure, only used for structures</param>
        /// <param name="lowPriority">Will reserve ressources if not currently affordable if true</param>
        void QueueUnit(uint unitId, Point2D desiredPosition = null, int spacing = 1,bool lowPriority = false);

        /// <summary>
        /// Queue upgrade and requirements after everything else in the queue.
        /// </summary>
        /// <param name="upgrade">Upgrade type to queue</param>
        /// <param name="lowPriority">Will reserve ressources if not currently affordable if true</param>
        void QueueTech(UpgradeData upgrade,bool lowPriority = false);

        /// <summary>
        /// Queue upgrade and requirements after everything else in the queue.
        /// </summary>
        /// <param name="upgradeId">Upgrade type to queue by id</param>
        /// <param name="lowPriority">Will reserve ressources if not currently affordable if true</param>
        void QueueTech(uint upgradeId ,bool lowPriority = false);
    }
}