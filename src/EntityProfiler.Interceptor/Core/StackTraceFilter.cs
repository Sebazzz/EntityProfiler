namespace EntityProfiler.Interceptor.Core {
    using System;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Represents a filter which filters out irrelevant items from the call stack. The filter will reject any Entity Framework and Entity Profiler call frames from the top of the call stack
    /// </summary>
    internal class StackTraceFilter : IStackTraceFilter {
        /// <summary>
        /// Returns a value indicating if the specified stack frame is relevant
        /// </summary>
        /// <param name="stackFrame"></param>
        /// <returns></returns>
        public bool IsRelevant(StackFrame stackFrame) {
            MethodBase method = stackFrame.GetMethod();

            return IsMethodRelevant(method);
        }

        private static bool IsMethodRelevant(MethodBase method) {
            Type declaringType = method.DeclaringType;
            if (declaringType == null) {
                return false; // I don't know why it would be null, but R# says so...
            }

            return IsAssemblyRelevant(declaringType.Assembly);
        }

        private static bool IsAssemblyRelevant(Assembly assembly) {
            string name = GetPrimaryAssemblyReference(assembly);

            return name.StartsWith("EntityFramework", StringComparison.OrdinalIgnoreCase) ||
                   name.StartsWith("EntityProfiler", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetPrimaryAssemblyReference(Assembly assembly) {
            // get the first part of the assembly, this is for most purposes enough
            string fullName = assembly.FullName;
            int dotIndex = fullName.IndexOf('.');

            return dotIndex == -1 ? fullName : fullName.Substring(0, dotIndex);
        }
    }
}