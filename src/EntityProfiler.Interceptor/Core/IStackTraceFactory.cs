namespace EntityProfiler.Interceptor.Core {
    using Common.Protocol;

    internal interface IStackTraceFactory {
        /// <summary>
        /// Creates a new stack trace
        /// </summary>
        /// <returns></returns>
        StackTrace Create();
    }
}