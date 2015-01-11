namespace EntityProfiler.Interceptor.Reader.Core {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Common.Protocol;

    /// <summary>
    /// Represents a query that contains multiple values
    /// </summary>
    public sealed class AggregateQuery : Query {
        /// <summary>
        /// Gets an approximation of the arguments for the query 
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Dictionary<string, object> Parameters { get { return base.Parameters; } set { base.Parameters = value; } }

        /// <summary>
        /// Gets the parameters by query
        /// </summary>
        public DataTable ParameterCollection { get; set; }


        /// <summary>
        /// Creates a <see cref="AggregateQuery"/> based on the given list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static AggregateQuery Create(List<QueryMessage> list) {
            if (list.Count == 0) {
                throw new ArgumentException("Expected at least one items to be given", "list");
            }

            AggregateQuery query = new AggregateQuery();
            Dictionary<string, object>[] parameters = new Dictionary<string, object>[list.Count];
            for (int i = 0; i < list.Count; i++) {
                Query q = list[i].Query;
                parameters[i] = q.Parameters;
                query.CommandText = q.CommandText;
            }

            query.ParameterCollection = new DataTable(parameters);
            return query;
        }
    }
}