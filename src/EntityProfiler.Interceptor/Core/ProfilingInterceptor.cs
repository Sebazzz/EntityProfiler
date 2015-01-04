namespace EntityProfiler.Interceptor.Core {
    using System.Data.Common;
    using System.Data.Entity.Infrastructure.Interception;

    /// <summary>
    /// Represents the core interceptor for profiling which is instantiated in the Entity Framework. It delegates
    /// its operations to an IOC-retrieved instance of <see cref="IDbCommandInterceptor"/>.
    /// </summary>
    public sealed class ProfilingInterceptor : System.Data.Entity.Infrastructure.Interception.DbCommandInterceptor {
        private readonly IDbCommandInterceptor _internalInstance;
        
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
            this._internalInstance = DependencyFactory.GetService<IDbCommandInterceptor>();
        }

        /// <summary>
        /// This method is called after a call to <see cref="M:System.Data.Common.DbCommand.ExecuteNonQuery"/>  or
        ///             one of its async counterparts is made. The result used by Entity Framework can be changed by setting
        ///             <see cref="P:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext`1.Result"/>.
        /// </summary>
        /// <remarks>
        /// For async operations this method is not called until after the async task has completed
        ///             or failed.
        /// </remarks>
        /// <param name="command">The command being executed.</param><param name="interceptionContext">Contextual information associated with the call.</param>
        public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            this._internalInstance.NonQueryExecuted(command, interceptionContext);
        }

        /// <summary>
        /// This method is called before a call to <see cref="M:System.Data.Common.DbCommand.ExecuteNonQuery"/> or
        ///             one of its async counterparts is made.
        /// </summary>
        /// <param name="command">The command being executed.</param><param name="interceptionContext">Contextual information associated with the call.</param>
        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            this._internalInstance.NonQueryExecuting(command, interceptionContext);
        }

        /// <summary>
        /// This method is called after a call to <see cref="M:System.Data.Common.DbCommand.ExecuteReader(System.Data.CommandBehavior)"/> or
        ///             one of its async counterparts is made. The result used by Entity Framework can be changed by setting
        ///             <see cref="P:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext`1.Result"/>.
        /// </summary>
        /// <remarks>
        /// For async operations this method is not called until after the async task has completed
        ///             or failed.
        /// </remarks>
        /// <param name="command">The command being executed.</param><param name="interceptionContext">Contextual information associated with the call.</param>
        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            this._internalInstance.ReaderExecuted(command, interceptionContext);
        }

        /// <summary>
        /// This method is called before a call to <see cref="M:System.Data.Common.DbCommand.ExecuteReader(System.Data.CommandBehavior)"/> or
        ///             one of its async counterparts is made.
        /// </summary>
        /// <param name="command">The command being executed.</param><param name="interceptionContext">Contextual information associated with the call.</param>
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            this._internalInstance.ReaderExecuting(command, interceptionContext);
        }

        /// <summary>
        /// This method is called after a call to <see cref="M:System.Data.Common.DbCommand.ExecuteScalar"/> or
        ///             one of its async counterparts is made. The result used by Entity Framework can be changed by setting
        ///             <see cref="P:System.Data.Entity.Infrastructure.Interception.DbCommandInterceptionContext`1.Result"/>.
        /// </summary>
        /// <remarks>
        /// For async operations this method is not called until after the async task has completed
        ///             or failed.
        /// </remarks>
        /// <param name="command">The command being executed.</param><param name="interceptionContext">Contextual information associated with the call.</param>
        public override void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            this._internalInstance.ScalarExecuted(command, interceptionContext);
        }

        /// <summary>
        /// This method is called before a call to <see cref="M:System.Data.Common.DbCommand.ExecuteScalar"/> or
        ///             one of its async counterparts is made.
        /// </summary>
        /// <param name="command">The command being executed.</param><param name="interceptionContext">Contextual information associated with the call.</param>
        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            this._internalInstance.ScalarExecuting(command, interceptionContext);
        }
    }
}