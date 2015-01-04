namespace EntityProfiler.Common.Protocol {
    using Annotations;

    /// <summary>
    /// Represents a message about query execution. It is never directly used and acts as a base class.
    /// </summary>
    public abstract class QueryMessage : Message {
        /// <summary>
        /// Gets the context the query was executed in
        /// </summary>
        public ExecutionContext Context { get; set; }

        /// <summary>
        /// Gets the query that was executed
        /// </summary>
        public Query Query { get; set; }

        /// <summary>
        /// Gets the string representation of any error that occurred
        /// </summary>
        [CanBeNull]
        public string Error { get; set; }

        /// <summary>
        /// Gets performance information
        /// </summary>
        public PerformanceData Performance { get; set; }
    }
}