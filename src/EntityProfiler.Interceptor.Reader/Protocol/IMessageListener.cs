namespace EntityProfiler.Interceptor.Reader.Protocol {
    using System;
    using Common.Events;

    /// <summary>
    /// Represents a listener which feeds incoming messages to <see cref="IMessageEventSubscriber"/>
    /// </summary>
    public interface IMessageListener : IDisposable {
        /// <summary>
        /// Starts listening for messages - any errors will be put to the <see cref="IMessageEventSubscriber"/>
        /// </summary>
        void Start();

        /// <summary>
        /// Stops listening for messages
        /// </summary>
        void Stop();
    }
}