namespace EntityProfiler.Common.Protocol {
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Represents a query that has been executed
    /// </summary>
    [DebuggerDisplay("{CommandText,nq}")]
    public sealed class Query {
        /// <summary>
        /// Gets the command text of the executed query
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// Gets an approximation of the arguments for the query 
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Query() {
            this.Parameters = new Dictionary<string, object>();
        }
    }
}