namespace EntityProfiler.Interceptor.Core {
    using System;
    using System.Data.Common;
    using System.Data.Entity;

    /// <summary>
    /// Defines the interface for an immediate struct holding interception concept. This allows us to use a different method of interception later while keeping the same back-end.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    internal interface IInterceptionData<out TResult> {
        /// <remarks>We assume only one DbContext</remarks>
        DbContext DbContext { get; }

        Exception Error { get; }

        DbCommand DbCommand { get; }

        TResult Result { get; }

        bool IsAsync { get; }
    }
}
