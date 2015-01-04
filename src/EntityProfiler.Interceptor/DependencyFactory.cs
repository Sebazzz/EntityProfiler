namespace EntityProfiler.Interceptor {
    using System;
    using Common;
    using Core;
    using Protocol;
    using TinyIoC;

    /// <summary>
    /// Dependency container 
    /// </summary>
    internal static class DependencyFactory {
        private static TinyIoCContainer _Container;
        
        /// <summary>
        /// Configures dependencies into the dependency container
        /// </summary>
        /// <param name="container"></param>
        private static void Configure(TinyIoC.TinyIoCContainer container) {
            container.Register<ITcpListenerFactory, TcpListenerFactory>();
            container.Register<IMessageSink, TcpMessageSink>().AsSingleton();

            container.Register<IDbCommandMessageFactory, DbCommandMessageFactory>().AsSingleton();

            Dependency.Configure(container);
        }
        public static void Configure() {
            _Container = new TinyIoCContainer();

            Configure(_Container);
        }

        /// <summary>
        /// Gets the service of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T:class {
            EnsureContainerInitialized<T>();

            return _Container.Resolve<T>();
        }

        private static void EnsureContainerInitialized<T>() where T : class {
            if (_Container == null) {
                throw new InvalidOperationException("Dependency container not initialized");
            }
        }
    }
}
