namespace EntityProfiler.Interceptor.Core {
    using System.Data.Common;
    using System.Data.Entity;
    using ExecutionContext = Common.Protocol.ExecutionContext;

    /// <summary>
    /// Defines the interface for creating an execution context
    /// </summary>
    internal interface IExecutionContextFactory {
        /// <summary>
        /// Creates an <see cref="Common.Protocol.ExecutionContext"/> from the available strategies
        /// </summary>
        /// <returns></returns>
        ExecutionContext Create(DbConnection connection, DbContext dbContext);
    }
}