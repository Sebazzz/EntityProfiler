using EntityProfiler.Common;
using EntityProfiler.Interceptor.Reader.Protocol;
using EntityProfiler.TinyIoC;

namespace EntityProfiler.Interceptor.Reader
{
    /// <summary>
    ///     Dependency container
    /// </summary>
    internal class DependencyFactory
    {
        /// <summary>
        ///     Configures dependencies into the dependency container
        /// </summary>
        /// <param name="container"></param>
        public static void Configure(TinyIoCContainer container)
        {
            container.Register<ITcpClientFactory, TcpClientFactory>();
            container.Register<IMessageListener, TcpMessageListener>();
            container.Register<IRestartableMessageListener, RestartableMessageListener>();

            Dependency.Configure(container);
        }
    }
}