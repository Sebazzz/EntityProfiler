namespace EntityProfiler.Common.Protocol {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Represents a strack trace included in the <see cref="ExecutionContext"/>
    /// </summary>
    [DebuggerDisplay("{ToString,nq}")]
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

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            if (this.Frames == null) {
                return String.Empty;
            }

            const string atWord = "at ";
            return atWord + String.Join(Environment.NewLine + " " + atWord, (IEnumerable<object>)this.Frames);
        }
    }
}