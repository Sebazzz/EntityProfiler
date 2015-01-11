namespace EntityProfiler.Interceptor.Reader.Core {
    using System;
    using System.Collections.Generic;
    using Common.Annotations;
    using Common.Protocol;

    /// <summary>
    /// Represents a component which can filter out an iteration of <see cref="Message"/> and return a new iteration
    /// </summary>
    internal interface IMessageFilter {
        /// <summary>
        /// Filters the messages specified. The filter may assume that the <paramref name="messages"/> are already grouped by execution context
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        IEnumerable<Message> Filter(IEnumerable<Message> messages);
    }

    /// <summary>
    /// Contains extensions for <see cref="IEnumerable{Message}"/>
    /// </summary>
    internal static class MessageEnumerableExtensions {
        /// <summary>
        /// Filters the enumerable using the specified filter
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IEnumerable<Message> FilterWith([NotNull] this IEnumerable<Message> enumerable, [NotNull] IMessageFilter filter) {
            if (enumerable == null) {
                throw new ArgumentNullException("enumerable");
            }
            if (filter == null) {
                throw new ArgumentNullException("filter");
            }

            return filter.Filter(enumerable);
        }
    }
}