namespace EntityProfiler.Interceptor.Reader.Core {
    using Common.Protocol;

    /// <summary>
    /// Represents a reader-generated message about duplicated queries (except on parameters)
    /// </summary>
    public sealed class DuplicateQueryMessage : QueryMessage {
        /// <summary>
        /// Gets the aggregate performance data for this instance
        /// </summary>
        public new AggregatePerformanceData Performance {
            get { return (AggregatePerformanceData) base.Performance; }
            set { base.Performance = value; }
        }

        /// <summary>
        /// Gets the aggregate query stats
        /// </summary>
        public new AggregateQuery Query {
            get { return (AggregateQuery) base.Query; }
            set { base.Query = value; }
        }

        /// <summary>
        /// Gets the number of queries that were duplicated
        /// </summary>
        public int NumberOfQueries { get; set; }
    }
}
