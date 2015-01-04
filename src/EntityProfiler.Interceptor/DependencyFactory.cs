namespace EntityProfiler.Interceptor {
    using System;
    using System.Data.Entity.Infrastructure.Interception;
    using System.Runtime.CompilerServices;
    using Common;
    using Core;
    using Protocol;
    using TinyIoC;

    /// <summary>
    /// Dependency container 
    /// </summary>
    internal static class DependencyFactory {
        private static readonly object SyncRoot = new object();
        private static TinyIoCContainer _Container;
        
        /// <summary>
        /// Configures dependencies into the dependency container
        /// </summary>
        /// <param name="container"></param>
        private static void Configure(TinyIoC.TinyIoCContainer container) {
            container.Register<ITcpListenerFactory, TcpListenerFactory>();
            container.Register<IMessageSink, TcpMessageSink>().AsSingleton();

            container.Register<IStackTraceFactory, StackTraceFactory>();
            container.Register<IStackTraceFilter, StackTraceFilter>();
            container.Register<IInterceptorLog, InterceptorLog>().AsSingleton();

            container.Register<IDbCommandInterceptor, ProxiedProfilingInterceptor>();

            container.Register<IQueryDataFactory, QueryDataFactory>();

            container.Register<IExecutionContextFactory, ExecutionContextFactory>();
            container.RegisterMultiple<IExecutionContextConstructor>(new [] { 
                // note, the order matters! if one execution context strategy cannot handle 
                // the creation it gets delegated to the next
                typeof(ThreadExecutionContextConstructor),                                              
                //typeof(HttpContextExecutionContextConstructor),
                typeof(DbContextExecutionContextConstructor),
            });

            Dependency.Configure(container);
        }
       
        public static void Configure() {
            lock (SyncRoot) {
                _Container = new TinyIoCContainer();

                Configure(_Container);
            }
        }

        /// <summary>
        /// Gets the service of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>() where T:class {
            EnsureContainerInitialized();

            return _Container.Resolve<T>();
        }

        private static void EnsureContainerInitialized() {
            if (_Container == null) {
                throw new InvalidOperationException("Dependency container not initialized");
            }
        }
    }
}
