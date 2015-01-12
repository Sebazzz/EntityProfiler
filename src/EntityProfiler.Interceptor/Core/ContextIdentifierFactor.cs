namespace EntityProfiler.Interceptor.Core {
    using System;
    using System.Threading;
    using Common.Protocol;

    internal static class ContextIdentifierFactory {
        /// <summary>
        /// Once this class is touched, this property is initialized. It gives a pretty good relative value when the app pool was started.
        /// </summary>
        private static readonly DateTime AppDomainTimestamp = DateTime.UtcNow;
        private static int _SequenceNumber = 0;

        public static ContextIdentifier Create() {
            return new ContextIdentifier(
                AppDomainTimestamp,
                Interlocked.Increment(ref _SequenceNumber));
        }
    }
}