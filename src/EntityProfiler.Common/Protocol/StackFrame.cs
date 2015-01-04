namespace EntityProfiler.Common.Protocol {
    using System;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// Represents a single frame in a stack trace
    /// </summary>
    /// <remarks>
    /// We use a custom class because the <see cref="System.Diagnostics.StackFrame"/> cannot be serialized by our code.
    /// </remarks>
    public sealed class StackFrame {
        /// <summary>
        /// Gets the method name (without signature)
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets the line number - if available. If the line number is not available, <c>0</c> is returned.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets the column number - if available. If the column number is not available, <c>0</c> is returned.
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Gets the MSIL offset from the start of the method
        /// </summary>
        public int ILOffset { get; set; }

        /// <summary>
        /// Gets the full type name (without assembly) 
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Gets the file path - if available. If the file path is not available <c>null</c> is returned.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Creates a <see cref="StackTrace"/> from the .NET system <see cref="System.Diagnostics.StackFrame"/>
        /// </summary>
        /// <param name="stackFrame"></param>
        /// <returns></returns>
        public static StackFrame Create(System.Diagnostics.StackFrame stackFrame) {
            MethodBase method = stackFrame.GetMethod();
            Type declaringType = method.DeclaringType;
            Debug.Assert(declaringType != null);

            return new StackFrame {
                                      ColumnNumber = stackFrame.GetFileColumnNumber(),
                                      FilePath = stackFrame.GetFileName(),
                                      LineNumber = stackFrame.GetFileLineNumber(),
                                      ILOffset = stackFrame.GetILOffset(),
                                      MethodName = method.ToString(),
                                      TypeName = declaringType.FullName
                                  };
        }
    }
}