namespace EntityProfiler.Common.Events {
    using System.Threading;

    internal sealed class SynchronizedReferenceMessageSubscriber : IMessageEventSubscriber {
        private readonly IMessageEventSubscriber _eventSubscriber;
        private readonly SynchronizationContext _synchronizationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public SynchronizedReferenceMessageSubscriber(IMessageEventSubscriber eventSubscriber) {
            this._eventSubscriber = eventSubscriber;
            this._synchronizationContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Occurs when a message is received
        /// </summary>
        /// <param name="event"></param>
        public void OnReceived(MessageEvent @event) {
            this._synchronizationContext.Post(s => ((IMessageEventSubscriber)s).OnReceived(@event), this._eventSubscriber);
        }

        /// <summary>
        /// Occurs before a message is send
        /// </summary>
        /// <param name="event"></param>
        public void OnSending(MessageEvent @event) {
            this._synchronizationContext.Post(s => ((IMessageEventSubscriber)s).OnSending(@event), this._eventSubscriber);
        }
    }
}