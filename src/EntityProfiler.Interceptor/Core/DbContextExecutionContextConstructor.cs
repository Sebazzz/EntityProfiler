namespace EntityProfiler.Interceptor.Core {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Threading;
    using Common.Protocol;
    using ExecutionContext = Common.Protocol.ExecutionContext;

    /// <summary>
    /// Creates a execution context based on a counter on the current db context
    /// </summary>
    internal sealed class DbContextExecutionContextConstructor : IExecutionContextConstructor {
        private readonly ConcurrentDictionary<Guid, List<DbContextWrapper>> _dbContexts;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DbContextExecutionContextConstructor() {
            this._dbContexts = new ConcurrentDictionary<Guid, List<DbContextWrapper>>();
        }


        /// <summary>
        /// Creates an <see cref="Common.Protocol.ExecutionContext"/> instance or returns <c>null</c>
        /// </summary>
        /// <returns></returns>
        public ExecutionContext CreateExecutionContext(DbContext dbContext) {
            if (dbContext == null) {
                return null;
            }

            ContextIdentifier contextId = this.GetContextNumber(dbContext);

            ExecutionContext ctx = new ExecutionContext(contextId, "DbContext instance #" + contextId);
            ctx.Values["ConnectionId"] = GetConnectionId(dbContext.Database.Connection);
            ctx.Values["ConnectionString"] = dbContext.Database.Connection.ConnectionString;

            return ctx;
        }

        /// <summary>
        /// Modifies an execution execution context and adds more information to i
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="ctx"></param>
        public void ModifyExistingExecutionContext(DbContext dbContext, ExecutionContext ctx) {
            if (dbContext == null) {
                return;
            }

            ctx.Values["ConnectionId"] = GetConnectionId(dbContext.Database.Connection);
            ctx.Values["ConnectionString"] = dbContext.Database.Connection.ConnectionString;
            ctx.Values["ContextId"] = this.GetContextNumber(dbContext);
        }

        private ContextIdentifier GetContextNumber(DbContext dbContext) {
            Guid connectionId = GetConnectionId(dbContext.Database.Connection);
            List<DbContextWrapper> contextsForConnection =
                this._dbContexts.GetOrAdd(connectionId, _ => new List<DbContextWrapper>());

            lock (contextsForConnection) {
                return TryAddContext(dbContext, contextsForConnection);
            }
        }

        private static ContextIdentifier TryAddContext(DbContext context, List<DbContextWrapper> contextList) {
            foreach (DbContextWrapper wrapper in contextList) {
                if (wrapper.ContainsContext(context)) {
                    return wrapper.Id;
                }
            }

            var id = ContextIdentifierFactory.Create();
            contextList.Add(new DbContextWrapper(id, context));

            return id;
        }

        private static Guid GetConnectionId(DbConnection connection) {
            SqlConnection sqlConnection = connection as SqlConnection;
            if (sqlConnection != null) {
                return sqlConnection.ClientConnectionId;
            }

            return Guid.Empty;
        }


        private struct DbContextWrapper {
            private readonly WeakReference<DbContext> _instance;

            public ContextIdentifier Id { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public DbContextWrapper(ContextIdentifier id, DbContext context) : this() {
                this.Id = id;
                this._instance = new WeakReference<DbContext>(context); 
            }

            public bool ContainsContext(DbContext instance) {
                DbContext org;
                return this._instance.TryGetTarget(out org) && ReferenceEquals(org, instance);
            }
        }
    }
}