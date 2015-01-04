namespace EntityProfiler.Interceptor.Core {
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure.Interception;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Actual core implementation of a profiling <see cref="DbCommandInterceptor"/> which is instantiated using DI
    /// </summary>
    internal sealed class ProxiedProfilingInterceptor: System.Data.Entity.Infrastructure.Interception.DbCommandInterceptor {
        private readonly IInterceptorLog _interceptorLog;

        public ProxiedProfilingInterceptor(IInterceptorLog interceptorLog) {
            this._interceptorLog = interceptorLog;
        }

        #region Non Query
        /// <inheritdoc/>
        public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            this._interceptorLog.OnNonQueryEnd(CreateInterceptionContext(command, interceptionContext));
        }

        /// <inheritdoc/>
        public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            this._interceptorLog.OnNonQueryBegin(command, GetDbContext(interceptionContext));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static NonQueryCommandInterceptionData CreateInterceptionContext(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            return new NonQueryCommandInterceptionData {
                DbCommand = command,
                DbContext = GetDbContext(interceptionContext),
                Error = interceptionContext.OriginalException,
                IsAsync = interceptionContext.IsAsync,
                Result = interceptionContext.Result
            };
        }
        #endregion

        #region Reader Executed / DbReader
        /// <inheritdoc/>
        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            this._interceptorLog.OnDbReaderQueryEnd(CreateInterceptionContext(command, interceptionContext));
        }

        /// <inheritdoc/>
        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            this._interceptorLog.OnDbReaderQueryBegin(command, GetDbContext(interceptionContext));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DbReaderCommandInterceptionData CreateInterceptionContext(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            return new DbReaderCommandInterceptionData {
                DbCommand = command,
                DbContext = GetDbContext(interceptionContext),
                Error = interceptionContext.OriginalException,
                IsAsync = interceptionContext.IsAsync,
                Result = interceptionContext.Result
            };
        }
        #endregion

        #region Scalar
        /// <inheritdoc/>
        public override void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            this._interceptorLog.OnScalarQueryEnd(CreateInterceptionContext(command, interceptionContext));
        }

        /// <inheritdoc/>
        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            this._interceptorLog.OnScalarQueryBegin(command, GetDbContext(interceptionContext));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ScalarCommandInterceptionData CreateInterceptionContext(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            return new ScalarCommandInterceptionData {
                DbCommand = command,
                DbContext = GetDbContext(interceptionContext),
                Error = interceptionContext.OriginalException,
                IsAsync = interceptionContext.IsAsync,
                Result = interceptionContext.Result
            };
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static DbContext GetDbContext(DbCommandInterceptionContext interceptionContext) {
            foreach (DbContext dbContext in interceptionContext.DbContexts) {
                return dbContext;
            }

            return null;
        }
    }
}