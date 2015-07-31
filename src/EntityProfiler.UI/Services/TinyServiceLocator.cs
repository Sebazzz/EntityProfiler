namespace EntityProfiler.UI.Services {
    using TinyIoC;

    internal class TinyServiceLocator : IServiceLocator {
        private readonly TinyIoCContainer _container;

        public TinyServiceLocator(TinyIoCContainer container) {
            this._container = container;
        }

        public T GetInstance<T>() where T : class {
            return this._container.Resolve<T>();
        }
    }
}