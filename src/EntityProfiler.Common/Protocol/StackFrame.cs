namespace EntityProfiler.Common.Protocol {
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Security;

    /// <summary>
    /// Represents a single frame in a stack trace
    /// </summary>
    /// <remarks>
    /// We use a custom class because the <see cref="System.Diagnostics.StackFrame"/> cannot be serialized by our code.
    /// </remarks>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed class StackFrame {
        private static bool SupportFileNameRetrieval = true;

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
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            if (!this.HasFileInfo) {
                return this.TypeName + "." + this.MethodName + " in " + this.FilePath + " " + this.LineNumber + ":" +
                       this.ColumnNumber;
            }

            return this.TypeName + "." + this.MethodName;
        }

        private bool HasFileInfo {
            get { return !String.IsNullOrEmpty(this.FilePath) && this.LineNumber > 0; }
        }

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
                                      FilePath = TryGetFileName(stackFrame),
                                      LineNumber = stackFrame.GetFileLineNumber(),
                                      ILOffset = stackFrame.GetILOffset(),
                                      MethodName = method.ToString(),
                                      TypeName = declaringType.FullName
                                  };
        }

        private static string TryGetFileName(System.Diagnostics.StackFrame stackFrame) {
            if (!SupportFileNameRetrieval) {
                return null;
            }

            // getting a file name may fail due to security limitations
            // so if we fail once we fail again in this app domain
            try {
                return stackFrame.GetFileName();
            }
            catch (NotSupportedException) {
                SupportFileNameRetrieval = false;
                return null;
            }
            catch (SecurityException) {
                SupportFileNameRetrieval = false;
                return null;
            }
        }
    }
}