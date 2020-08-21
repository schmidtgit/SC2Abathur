using Abathur.Constants;
using Abathur.Core;
using Abathur.Extensions;
using NydusNetwork.Logging;
using System.Linq;

namespace Abathur.Modules
{
    class Debug : IModule {
        private IProductionManager productionManager;
        private IIntelManager intelManager;
        private ILogger log;

        private bool colonyInfo = true;

        public Debug(IProductionManager productionManager,IIntelManager intelManager,ILogger logger) {
            this.productionManager = productionManager;
            this.intelManager = intelManager;
            this.log = logger;
        }
        public void OnGameEnded() { }

        public void OnStart() {
            if(colonyInfo)
                PrintColonyInformation();
        }

        public void OnStep() { }
        public void Initialize() { }
        public void OnRestart() { }

        private void PrintColonyInformation() {
            log.LogSuccess("Colony debug information:");
            foreach(var c in intelManager.Colonies) {
                var minerals = c.Minerals.Count();
                var vespene = c.Vespene.Count();
                if(c.Id == intelManager.PrimaryColony.Id)
                    log.LogSuccess($"\t{pid(c.Id)} | {c.Vespene.Count()} vespenes and {c.Minerals.Count()} minerals [PRIMARY]");
                else if(c.IsStartingLocation)
                    log.LogSuccess($"\t{pid(c.Id)} | {c.Vespene.Count()} vespenes and {c.Minerals.Count()} minerals [STARTING]");
                else if(vespene == 2 && minerals == 8)
                    log.LogSuccess($"\t{pid(c.Id)} | {c.Vespene.Count()} vespenes and {c.Minerals.Count()} minerals");
                else if(vespene == 2 && minerals == 6)
                    log.LogSuccess($"\t{pid(c.Id)} | {c.Vespene.Count()} vespenes and {c.Minerals.Count()} minerals [GOLD]");
                else
                    log.LogError($"\t{pid(c.Id)} |  {c.Vespene.Count()} vespenes and {c.Minerals.Count()} minerals [ERROR]");
            }
            if(intelManager.MineralFields.Count() != intelManager.Colonies.Sum(c => c.Minerals.Count()))
                log.LogError($"\tMINERALS | {intelManager.MineralFields.Count()} total,  {intelManager.Colonies.Sum(c => c.Minerals.Count())} in colonies");
            if(intelManager.VespeneGeysers.Count() != intelManager.Colonies.Sum(c => c.Vespene.Count()))
                log.LogError($"\tVESPENE  | {intelManager.VespeneGeysers.Count()} total,  {intelManager.Colonies.Sum(c => c.Vespene.Count())} in colonies");
        }

        private string pid(uint id) {
            if(id > 9)
                return $"{id}";
            else
                return $"{id} ";
        }
    }
}
