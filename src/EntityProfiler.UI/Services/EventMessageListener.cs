namespace EntityProfiler.UI.Services {
    using Caliburn.Micro;
    using Common.Events;

    internal sealed class EventMessageListener : IMessageEventSubscriber {
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public EventMessageListener(IEventAggregator eventAggregator) {
            this._eventAggregator = eventAggregator;
        }


        /// <summary>
        /// Occurs when a message is received
        /// </summary>
        /// <param name="event"></param>
        public void OnReceived(MessageEvent @event) {
            this._eventAggregator.BeginPublishOnUIThread(@event);
        }

        /// <summary>
        /// Occurs before a message is send
        /// </summary>
        /// <param name="event"></param>
        public void OnSending(MessageEvent @event) {
            // we are currently not interested in these
        }
    }
}