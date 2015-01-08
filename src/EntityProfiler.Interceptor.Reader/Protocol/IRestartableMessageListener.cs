namespace EntityProfiler.Interceptor.Reader.Protocol {
    using System;
    using Common.Events;

    /// <summary>
    /// Represents an <see cref="IMessageListener"/> which can be restarted
    /// </summary>
    public interface IRestartableMessageListener : IMessageListener {
        /// <summary>
        ///     Restarts the underlying message listener
        /// </summary>
        void Restart();
    }

    internal sealed class RestartableMessageListener : IRestartableMessageListener {
        private IMessageListener _messageListener;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public RestartableMessageListener(IMessageListener messageListener) {
            this._messageListener = messageListener;
        }

        public void Restart() {
            this._messageListener.Stop();
            this._messageListener.Dispose();

            this._messageListener = this._messageListener.Clone();
            this._messageListener.Start();
        }

        /// <summary>
        ///     Starts listening for messages - any errors will be put to the <see cref="IMessageEventSubscriber" />
        /// </summary>
        public void Start() {
            this._messageListener.Start();
        }

        /// <summary>
        ///     Stops listening for messages
        /// </summary>
        public void Stop() {
            this._messageListener.Stop();
        }

        /// <summary>
        ///     Creates a new instance based on the current
        /// </summary>
        public IMessageListener Clone() {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            this._messageListener.Dispose();
        }
    }
}