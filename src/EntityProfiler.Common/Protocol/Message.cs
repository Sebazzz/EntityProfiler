namespace EntityProfiler.Common.Protocol {
    using System;

    /// <summary>
    /// Base class for messages sent over the wire
    /// </summary>
    [Serializable]
    public class Message {
        /// <summary>
        /// Gets or sets the timestamp this message was generated
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Message() {
            this.Timestamp = DateTime.UtcNow;
        }
    }
}