namespace EntityProfiler.Common.Events {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Annotations;

    internal class MessageEventDispatcher : IMessageEventSubscriptionManager {
        private readonly ConcurrentBag<IMessageEventSubscriber> _subscribers;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public MessageEventDispatcher(IEnumerable<IMessageEventSubscriber> subscribers) {
            this._subscribers = new ConcurrentBag<IMessageEventSubscriber>(subscribers);
        }

        /// <summary>
        /// Adds a subscriber that will receive message events. This class will wrap it with a <see cref="WeakReferenceMessageSubscriber"/>
        /// so the pointer is not hold on and the reference can be GC'ed
        /// </summary>
        /// <param name="messageEventSubscriber"></param>
        public void AddSubscriber([NotNull] IMessageEventSubscriber messageEventSubscriber) {
            if (messageEventSubscriber == null) {
                throw new ArgumentNullException("messageEventSubscriber");
            }

            WeakReferenceMessageSubscriber refSubscriber = new WeakReferenceMessageSubscriber(messageEventSubscriber);
            this._subscribers.Add(refSubscriber);
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