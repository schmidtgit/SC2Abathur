using System.Linq;
using Abathur.Constants;
using Abathur.Core;
using Abathur.Core.Intel.Map;
using Abathur.Repositories;
using NydusNetwork.API.Protocol;

namespace Abathur.Modules.Demo
{
    public class CombatManagerLiveTest : IModule
    {
        private readonly IIntelManager _intelManager;
        private readonly ICombatManager _combatManager;
        private readonly IProductionManager _productionManager;
        private ISquadRepository _squadRep;

        public CombatManagerLiveTest(IIntelManager intelManager, ICombatManager combatManager, IProductionManager productionManager, ISquadRepository squadRepo) {
            _intelManager = intelManager;
            _combatManager = combatManager;
            _productionManager = productionManager;
            _squadRep = squadRepo;
        }
        public void Initialize() {}

        public void OnStart()
        {
            for (int i = 0; i < 20; i++)
            {
                _productionManager.QueueUnit(BlizzardConstants.Unit.Marine);
            }
        }

        public void OnStep()
        {
        }

        public void OnGameEnded()
        {
        }

        public void OnRestart()
        {
        }

    }
}
