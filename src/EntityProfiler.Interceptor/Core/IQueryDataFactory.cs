namespace EntityProfiler.Interceptor.Core {
    using System.Data.Common;
    using Common.Protocol;

    /// <summary>
    /// Implementation for objects which create <see cref="Query"/> objects from a <see cref="DbCommand"/>
    /// </summary>
    internal interface IQueryDataFactory {
        Query CreateQuery(DbCommand command);
    }
}