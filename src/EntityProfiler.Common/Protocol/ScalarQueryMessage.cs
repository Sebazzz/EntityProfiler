namespace EntityProfiler.Common.Protocol {
    using System;
    using System.Globalization;
    using Annotations;

    /// <summary>
    /// Represents a scalar query that yields a single result
    /// </summary>
    public sealed class ScalarQueryMessage : QueryMessage {
        /// <summary>
        /// Gets the result of the query, converted to a string
        /// </summary>
        [CanBeNull]
        public string Result { get; set; }

        /// <summary>
        /// Gets the original .NET type name of the <see cref="Result"/>
        /// </summary>
        [CanBeNull]
        public string ResultType { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ScalarQueryMessage"/>
        /// </summary>
        public ScalarQueryMessage() {}

        /// <summary>
        /// Creates a <see cref="ScalarQueryMessage"/> object from the specified result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ScalarQueryMessage Create(object result) {
            if (result == null) {
                return new ScalarQueryMessage();
            }

            return new ScalarQueryMessage {
                                              Result = Convert.ToString(result, CultureInfo.InvariantCulture),
                                              ResultType = result.GetType().FullName
                                          };
        }
    }
}