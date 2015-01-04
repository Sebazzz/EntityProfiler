namespace EntityProfiler.Interceptor.Core {
    using System.Data.Common;
    using System.Data.Entity.Infrastructure.Interception;
    using Common.Protocol;
    using Protocol;

    /// <summary>
    /// Represents the core interceptor for profiling
    /// </summary>
    public sealed class ProfilingInterceptor : System.Data.Entity.Infrastructure.Interception.DbCommandInterceptor {
        private readonly IDbCommandMessageFactory _messageFactory;
        private readonly IMessageSink _messageSink;
        
        /// <summary>
        /// Static ctor ensuring internal DI initialization
        /// </summary>
        static ProfilingInterceptor() {
            DependencyFactory.Configure();
        }
        
        /// <summary>
        /// Initializes the interceptor
        /// </summary>
        public ProfilingInterceptor() {
            this._messageFactory = DependencyFactory.GetService<IDbCommandMessageFactory>();

            this._messageSink = DependencyFactory.GetService<IMessageSink>();
            this._messageSink.Start();
        }

        /// <inheritdoc/>
        public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            this.OnExecuted(command, interceptionContext);
        }

        /// <inheritdoc/>
        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            this.OnExecuting(command, interceptionContext);
        }

        /// <inheritdoc/>
        public override void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            this.OnExecuted(command, interceptionContext);
        }

        /// <inheritdoc/>
        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            this.OnExecuting(command, interceptionContext);
        }

        /// <inheritdoc/>
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            this.OnExecuting(command, interceptionContext);
        }

        /// <inheritdoc/>
        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            this.OnExecuted(command, interceptionContext);
        }

        private void OnExecuting<TResult>(DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext) {
            this._messageFactory.BeginCreateMessage(command, interceptionContext);
        }

        private void OnExecuted<TResult>(DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext) {
            Message message = this._messageFactory.EndCreateMessage(command, interceptionContext);

            if (message == null) {
                // normally this method won't return null, but if for some reason the DbCommand instance cannot
                // be found we cannot throw exceptions here and crash the whole application
                return;
            }

            this._messageSink.DispatchMessage(message);
        }
    }
}