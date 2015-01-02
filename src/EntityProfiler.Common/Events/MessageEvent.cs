namespace EntityProfiler.Common.Events {
    using System.Diagnostics;
    using Protocol;

    /// <summary>
    /// Represents event data for when a message is received
    /// </summary>
    internal class MessageEvent {
        private readonly Message _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MessageEvent(Message message) {
            this._message = message;
        }

        public Message Message {
            [DebuggerStepThrough] get { return this._message; }
        }
    }
}