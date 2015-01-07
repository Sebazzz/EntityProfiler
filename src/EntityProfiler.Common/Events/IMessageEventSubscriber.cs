namespace EntityProfiler.Common.Events {
    /// <summary>
    /// Defines the interface message event listeners should implement
    /// </summary>
    public interface IMessageEventSubscriber {
        /// <summary>
        /// Occurs when a message is received
        /// </summary>
        /// <param name="event"></param>
        void OnReceived(MessageEvent @event);

        /// <summary>
        /// Occurs before a message is send
        /// </summary>
        /// <param name="event"></param>
        void OnSending(MessageEvent @event);
    }
}