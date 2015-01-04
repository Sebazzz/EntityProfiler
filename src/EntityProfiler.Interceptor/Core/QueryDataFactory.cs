namespace EntityProfiler.Interceptor.Core {
    using System.Data;
    using System.Data.Common;
    using Common.Protocol;

    /// <summary>
    /// Creates <see cref="Query"/> objects from a <see cref="DbCommand"/>
    /// </summary>
    internal class QueryDataFactory : IQueryDataFactory {
        public Query CreateQuery(DbCommand command) {
            Query q = new Query();
            
            q.CommandText = command.CommandText;

            AddParameters(q, command.Parameters);

            return q;
        }

        private static void AddParameters(Query query, DbParameterCollection parameters) {
            foreach (DbParameter parameter in parameters) {
                if (parameter.Direction == ParameterDirection.Input) {
                    query.Parameters[parameter.ParameterName] = parameter.Value;
                }
            }
        }
    }
}