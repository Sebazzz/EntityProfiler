namespace EntityProfiler.Interceptor.Core {
    using System.Data.Entity;
    using ExecutionContext = Common.Protocol.ExecutionContext;

    /// <summary>
    /// Defines the interface for a class that can potentially create an execution context
    /// </summary>
    internal interface IExecutionContextConstructor {
        /// <summary>
        /// Creates an <see cref="Common.Protocol.ExecutionContext"/> instance or returns <c>null</c>
        /// </summary>
        /// <returns></returns>
        ExecutionContext CreateExecutionContext(DbContext dbContext);

        /// <summary>
        /// Modifies an execution execution context and adds more information to i
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="executionContext"></param>
        void ModifyExistingExecutionContext(DbContext dbContext, ExecutionContext executionContext);
    }
}