namespace EntityProfiler.Interceptor.Core {
    using System.Data.Common;
    using System.Data.Entity.Infrastructure.Interception;

    /// <summary>
    /// Represents the core interceptor for profiling
    /// </summary>
    public sealed class ProfilingInterceptor : System.Data.Entity.Infrastructure.Interception.DbCommandInterceptor {
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
            
        }

        /// <inheritdoc/>
        public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
        }

        /// <inheritdoc/>
        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
        }

        /// <inheritdoc/>
        public override void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
        }

        /// <inheritdoc/>
        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
        }

        /// <inheritdoc/>
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
        }

        /// <inheritdoc/>
        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
        }
    }
}