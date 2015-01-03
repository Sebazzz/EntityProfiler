namespace EntityProfiler.Common.Events {
    using System.Collections.Generic;

    internal class MessageEventDispatcher {
        private readonly IEnumerable<IMessageEventSubscriber> _subscribers;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MessageEventDispatcher(IEnumerable<IMessageEventSubscriber> subscribers) {
            this._subscribers = subscribers;
        }

        public void DispatchReceived(MessageEvent messageEvent) {
            foreach (IMessageEventSubscriber subscriber in this._subscribers) {
                subscriber.OnReceived(messageEvent);
            }
        }

        public void DispatchSending(MessageEvent messageEvent) {
            foreach (IMessageEventSubscriber subscriber in this._subscribers) {
                subscriber.OnSending(messageEvent);
            }
        }
    }
}