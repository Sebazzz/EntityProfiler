namespace EntityProfiler.Interceptor.Reader.Protocol {
    using System;

    internal interface IMessageListener : IDisposable {
        void Start();
        void Stop();
    }
}