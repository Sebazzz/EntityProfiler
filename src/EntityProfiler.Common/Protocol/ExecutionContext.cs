namespace EntityProfiler.Common.Protocol {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Represents a generic container for an execution context - a execution context may be a thread, HttpContext, ActiveForm
    /// </summary>
    [DebuggerDisplay("{Description} @ {Timestamp,nq}")]
    public class ExecutionContext {
        /// <summary>
        /// Gets a general text that provides a descriptive value for the execution context
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the call stack at time of execution
        /// </summary>
        public StackTrace CallStack { get; set; }

        /// <summary>
        /// Gets any generic values from the execution context
        /// </summary>
        public Dictionary<string, object> Values { get; set; }

        /// <summary>
        /// Timestamp of query execution
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ExecutionContext() {
            this.Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ExecutionContext(string description) {
            this.Timestamp = DateTime.UtcNow;
            this.Description = description;
            this.Values = new Dictionary<string, object>();
        }
    }
}