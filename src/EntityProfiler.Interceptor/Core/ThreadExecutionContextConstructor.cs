namespace EntityProfiler.Interceptor.Core {
    using System.Data.Common;
    using System.Data.Entity;
    using System.Threading;
    using ExecutionContext = Common.Protocol.ExecutionContext;

    /// <summary>
    /// Creates a <see cref="ExecutionContext"/> from the current Thread ID. Note this fails to produce
    /// reliable results when using Async queries.
    /// </summary>
    internal sealed class ThreadExecutionContextConstructor : IExecutionContextConstructor {
        /// <summary>
        /// Creates an <see cref="ExecutionContext"/> instance or returns <c>null</c>
        /// </summary>
        /// <returns></returns>
        public ExecutionContext CreateExecutionContext(DbContext dbContext) {
            Thread currentThread = Thread.CurrentThread;

            return new ExecutionContext("Thread #" + currentThread.ManagedThreadId);
        }

        /// <summary>
        /// Modifies an execution execution context and adds more information to it
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="executionContext"></param>
        public void ModifyExistingExecutionContext(DbContext dbContext, ExecutionContext executionContext) {
            Thread currentThread = Thread.CurrentThread;
            executionContext.Values["ThreadId"] = currentThread.ManagedThreadId;
        }
    }
}