namespace EntityProfiler.Interceptor.Reader {
    using System.Threading;
    using Common.Events;

    internal class SynchronizedMessageEventSubscriber : IMessageEventSubscriber {
        private readonly SynchronizationContext _synchronizationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public SynchronizedMessageEventSubscriber() {
            this._synchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Occurs when a message is received
        /// </summary>
        /// <param name="event"></param>
        public void OnReceived(MessageEvent @event) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Occurs before a message is send
        /// </summary>
        /// <param name="event"></param>
        public void OnSending(MessageEvent @event) {
            throw new System.NotImplementedException();
        }
    }
}