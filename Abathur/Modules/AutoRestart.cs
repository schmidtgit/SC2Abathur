using NydusNetwork.API.Protocol;

namespace Abathur.Modules
{
    class AutoRestart : IModule {
        private IAbathur abathur;
        public AutoRestart(IAbathur abathur) {
            this.abathur = abathur;
        }
        void IModule.OnGameEnded() => abathur.Restart();
        void IModule.Initialize() { }
        void IModule.OnStart() {}
        void IModule.OnStep() {}
        void IModule.OnRestart() {}
    }
}
