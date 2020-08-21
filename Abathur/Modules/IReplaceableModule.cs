namespace Abathur.Modules {
    public interface IReplaceableModule : IModule {
        void OnAdded();
        void OnRemoved();
    }
}
