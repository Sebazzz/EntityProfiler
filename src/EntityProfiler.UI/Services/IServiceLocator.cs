namespace EntityProfiler.UI.Services {
    public interface IServiceLocator {
        T GetInstance<T>() where T : class;
    }
}