namespace EntityProfiler.Common.Events {
    using Annotations;

    internal interface IMessageEventSubscriptionManager {
        /// <summary>
        /// Adds a subscriber that will receive message events. This class will wrap it with a <see cref="WeakReferenceMessageSubscriber"/>
        /// so the pointer is not hold on and the reference can be GC'ed
        /// </summary>
        /// <param name="messageEventSubscriber"></param>
        void AddSubscriber([NotNull] IMessageEventSubscriber messageEventSubscriber);
    }
}