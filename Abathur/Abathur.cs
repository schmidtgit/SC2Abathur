using Abathur.Constants;
using Abathur.Core;
using Abathur.Modules;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;
using NydusNetwork.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abathur {
    public class Abathur : IAbathur {
        public Status Status => rawManager.Status;
        public bool IsParallelized              { get; set; }
        public bool IsHosting                   { get; set; }
        public GameSettings Settings            { get; set; }
        public List<IModule> Modules            { get; set; }

        private List<IModule> CoreModules;
        private IRawManager rawManager;
        private ILogger log;

        private Queue<IReplaceableModule> addedModules = new Queue<IReplaceableModule>();
        private Queue<IReplaceableModule> removedModules = new Queue<IReplaceableModule>();

        public Abathur(ILogger logger,IIntelManager intelManager,ICombatManager combatManager,
            IProductionManager productionManager,IRawManager rawManager,GameSettings gameSettings) {
            this.rawManager = rawManager;
            this.log = logger;
            Settings = gameSettings;
            CoreModules = new List<IModule> {
                intelManager,
                combatManager,
                productionManager
            };
        }

        public void Initialize() {
            GameConstants.ParticipantRace = Settings.ParticipantRace;
            GameConstants.EnemyRace = Settings.Opponents.Count > 0 ? Settings.Opponents[0].Race : Race.NoRace;

            if(Modules == null) {
                Modules = new List<IModule>();
                log.LogWarning("Abathur: Running with 0 modules");
            }

            rawManager.Realtime = Settings.Realtime;
            rawManager.IsHosting = IsHosting;
            rawManager.Initialize();
            CoreModules.ForEach(c => c.Initialize());
            Modules.ForEach(m => m.Initialize());
        }

        public void CreateGame() {
            if(Status == Status.InGame)
                rawManager.LeaveGame();
            if(!rawManager.CreateGame())
                log.LogWarning("Abathur: Failed at starting game.");
        }

        public void Run() {
            if(!rawManager.JoinGame())
                return;
            GameLoop();
        }

        public void Restart() {
            rawManager.Restart();
            CoreModules.ForEach(c => c.OnRestart());
            if(IsParallelized)
                Parallel.ForEach(Modules,m => m.OnRestart());
            else
                Modules.ForEach(m => m.OnRestart());
            GameLoop();
        }

        public void GameLoop() {
            CoreModules.ForEach(c => c.OnStart());
            ChangeModules();
            if(IsParallelized)
                Parallel.ForEach(Modules,m => m.OnStart());
            else
                Modules.ForEach(m => m.OnStart());

            rawManager.Step();
            while(Status == Status.InGame) {
                CoreModules.ForEach(c => c.OnStep());
                if(IsParallelized)
                    Parallel.ForEach(Modules,m => m.OnStep());
                else
                    Modules.ForEach(m => m.OnStep());
                ChangeModules();
                rawManager.Step();
            }

            CoreModules.ForEach(c => c.OnGameEnded());
            if(IsParallelized)
                Parallel.ForEach(Modules,m => m.OnGameEnded());
            else
                Modules.ForEach(m => m.OnGameEnded());
        }

        private void ChangeModules() {
            lock(addedModules)
                while(addedModules.TryDequeue(out var module))
                    if(!Modules.Contains(module)) {
                        module.OnAdded();
                        Modules.Add(module);
                    }
#if DEBUG
                    else
                        log.LogWarning($"Abathur: (Add Module) Gameloop already contains {module.GetType().Name}");
#endif
            lock(removedModules)
                while(removedModules.TryDequeue(out var module))
                    if(Modules.Contains(module)) {
                        module.OnRemoved();
                        Modules.Remove(module);
                    }
#if DEBUG
                    else
                        log.LogWarning($"Abathur: (Remove Module) Gameloop did not contain {module.GetType().Name}");
#endif
        }

        public void AddToGameloop(IReplaceableModule module) {
            lock(addedModules)
                addedModules.Enqueue(module);
        }

        public void RemoveFromGameloop(IReplaceableModule module) {
            lock(removedModules)
                removedModules.Enqueue(module);
        }
    }
}
