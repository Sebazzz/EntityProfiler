namespace EntityProfiler.Common.Events {
    using System;
    using System.Diagnostics;
    using Protocol;

    /// <summary>
    /// Represents event data for when a message is received
    /// </summary>
    public struct MessageEvent {
        private readonly Message _message;
        private readonly Exception _exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MessageEvent(Exception exception) : this() {
            this._exception = exception;
        }

        /// <summary>
        /// Gets an exception
        /// </summary>
        public Exception Exception {
            [DebuggerStepThrough] get { return this._exception; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MessageEvent(Message message) : this() {
            this._message = message;
        }

        /// <summary>
        /// Gets the message if <see cref="Exception"/> is not <c>null</c>.
        /// </summary>
        public Message Message {
            [DebuggerStepThrough] get { return this._message; }
        }
    }
}