namespace EntityProfiler.Interceptor.Core {
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Linq;
    using Common.Protocol;

    /// <summary>
    /// Default implementation of <see cref="IExecutionContextFactory"/>
    /// </summary>
    internal sealed class ExecutionContextFactory : IExecutionContextFactory {
        private readonly IExecutionContextConstructor[] _strategies;
        private readonly IStackTraceFactory _stackTraceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ExecutionContextFactory(IEnumerable<IExecutionContextConstructor> strategies, IStackTraceFactory stackTraceFactory) {
            this._stackTraceFactory = stackTraceFactory;
            this._strategies = strategies.ToArray();
        }

        /// <summary>
        /// Creates an <see cref="ExecutionContext"/> from the available strategies
        /// </summary>
        /// <returns></returns>
        public ExecutionContext Create(DbConnection connection, DbContext dbContext) {
            ExecutionContext ec = null;
            foreach (IExecutionContextConstructor strategy in this._strategies) {
                bool wasEcCreated = ec != null;
                if (wasEcCreated) {
                    strategy.ModifyExistingExecutionContext(dbContext, ec);
                    continue;
                }

                ec = strategy.CreateExecutionContext(dbContext);
                if (ec != null) {
                    ec.CallStack = this._stackTraceFactory.Create();
                }
            }

            if (ec == null) {
                throw new InvalidOperationException("Unable to determine the execution context - internal error");
            }

            return ec;
        }
    }
}