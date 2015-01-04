namespace EntityProfiler.Interceptor.Core {
    using Common.Protocol;
    using SysStackFrame = System.Diagnostics.StackFrame;
    using SysStackTrace = System.Diagnostics.StackTrace;

    /// <summary>
    /// Represents a factory for creating <see cref="Common.Protocol.StackTrace"/> objects. The factory will ensure irrelevant stack frames are removed from the top of the stack.
    /// </summary>
    internal class StackTraceFactory : IStackTraceFactory {
        private const bool IncludeFileInformation = true;
        private const int MethodsToSkip = 5; // rogue guess
        private readonly IStackTraceFilter _stackTraceFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public StackTraceFactory(IStackTraceFilter stackTraceFilter) {
            this._stackTraceFilter = stackTraceFilter;
        }

        /// <summary>
        /// Creates a new stack trace
        /// </summary>
        /// <returns></returns>
        public StackTrace Create() {
            SysStackFrame[] rawFrames = GetFrames();
            
            return new StackTrace(this.GetFilteredFrames(rawFrames));
        }

        /// <summary>
        /// Filters and copies the specified array of .NET stack frames
        /// </summary>
        /// <param name="stackFrames"></param>
        /// <returns></returns>
        private StackFrame[] GetFilteredFrames(SysStackFrame[] stackFrames) {
            StackFrame[] result = null;

            int resultIndex = 0;
            for (int i = 0; i < stackFrames.Length; i++) {
                SysStackFrame current = stackFrames[i];

                // postpone allocating the array until we know how big it should be
                if (result == null) {
                    // filter the top irrelevant frames from the stack
                    if (!this._stackTraceFilter.IsRelevant(current)) {
                        continue;
                    }

                    result = new StackFrame[stackFrames.Length - i];
                }

                result[resultIndex] = StackFrame.Create(stackFrames[i]);
                resultIndex ++;
            }

            return result;
        }

        /// <summary>
        /// Gets the stack frames
        /// </summary>
        /// <returns></returns>
        private static SysStackFrame[] GetFrames() {
            return new SysStackTrace(MethodsToSkip, IncludeFileInformation).GetFrames();
        }
    }
}