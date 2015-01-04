namespace EntityProfiler.Common.Protocol {
    /// <summary>
    /// Represents a strack trace included in the <see cref="ExecutionContext"/>
    /// </summary>
    public sealed class StackTrace {
        /// <summary>
        /// Gets the frames in the stack trace
        /// </summary>
        public StackFrame[] Frames { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public StackTrace() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public StackTrace(StackFrame[] frames) {
            this.Frames = frames;
        }
    }
}