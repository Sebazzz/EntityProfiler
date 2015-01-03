namespace EntityProfiler.Interceptor.Reader {
    using Protocol;

    /// <summary>
    /// Dependency container 
    /// </summary>
    internal class Dependency {
        /// <summary>
        /// Configures dependencies into the dependency container
        /// </summary>
        /// <param name="container"></param>
        public static void Configure(TinyIoC.TinyIoCContainer container) {
            container.Register<ITcpClientFactory, TcpClientFactory>();
            container.Register<IMessageListener, TcpMessageListener>();

            Common.Dependency.Configure(container);
        }
    }
}
