namespace EntityProfiler.Interceptor.Protocol {
    using System;
    using Common.Protocol;

    internal interface IMessageSink : IDisposable {
        /// <summary>
        /// Starts accepting connections
        /// </summary>
        void Start();

        /// <summary>
        /// Dispatches a message to listening clients
        /// </summary>
        /// <param name="message"></param>
        void DispatchMessage(Message message);
    }
}