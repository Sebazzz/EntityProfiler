namespace EntityProfiler.Common.Protocol {
    /// <summary>
    /// Represents a message about a query which returns one or more rows.
    /// </summary>
    public sealed class DbReaderQueryMessage : QueryMessage {
        /// <summary>
        /// Gets the number of records retrieved
        /// </summary>
        public int RecordCount { get; set; }
    }
}