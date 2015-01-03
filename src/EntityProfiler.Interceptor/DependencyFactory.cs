namespace EntityProfiler.Interceptor {
    using Common;
    using Protocol;
    using TinyIoC;

    /// <summary>
    /// Dependency container 
    /// </summary>
    internal class DependencyFactory {
        /// <summary>
        /// Configures dependencies into the dependency container
        /// </summary>
        /// <param name="container"></param>
        private static void Configure(TinyIoC.TinyIoCContainer container) {
            container.Register<ITcpListenerFactory, TcpListenerFactory>();
            container.Register<IMessageSink, TcpMessageSink>().AsSingleton();

            Dependency.Configure(container);
        }


        public static void Configure() {
            TinyIoC.TinyIoCContainer container = new TinyIoCContainer();
            Configure(container);
        }
    }
}
