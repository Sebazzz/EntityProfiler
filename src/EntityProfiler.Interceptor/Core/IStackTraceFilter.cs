namespace EntityProfiler.Interceptor.Core {
    using System.Diagnostics;

    internal interface IStackTraceFilter {
        /// <summary>
        /// Returns a value indicating if the specified stack frame is relevant
        /// </summary>
        /// <param name="stackFrame"></param>
        /// <returns></returns>
        bool IsRelevant(StackFrame stackFrame);
    }
}