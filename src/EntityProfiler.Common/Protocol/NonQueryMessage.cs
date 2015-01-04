namespace EntityProfiler.Common.Protocol {
    /// <summary>
    /// Represents a UPDATE/DELETE query that yields a number of affected records
    /// </summary>
    public sealed class NonQueryMessage : QueryMessage {
        /// <summary>
        /// Gets the number of affected records
        /// </summary>
        public int AffectedRecords { get; set; }
    }
}