namespace EntityProfiler.Common.Events {
    using System;
    using Annotations;

    internal sealed class WeakReferenceMessageSubscriber : IMessageEventSubscriber {
        private readonly WeakReference<IMessageEventSubscriber> _eventSubscriber;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public WeakReferenceMessageSubscriber([NotNull] IMessageEventSubscriber messageEventSubscriber) {
            if (messageEventSubscriber == null) {
                throw new ArgumentNullException("messageEventSubscriber");
            }

            this._eventSubscriber = new WeakReference<IMessageEventSubscriber>(messageEventSubscriber);
        }

        /// <summary>
        /// Occurs when a message is received
        /// </summary>
        /// <param name="event"></param>
        public void OnReceived(MessageEvent @event) {
            IMessageEventSubscriber instance;
            if (this._eventSubscriber.TryGetTarget(out instance)) {
                instance.OnReceived(@event);
            }
        }

        /// <summary>
        /// Occurs before a message is send
        /// </summary>
        /// <param name="event"></param>
        public void OnSending(MessageEvent @event) {
            IMessageEventSubscriber instance;
            if (this._eventSubscriber.TryGetTarget(out instance)) {
                instance.OnSending(@event);
            }
        }
    }
}