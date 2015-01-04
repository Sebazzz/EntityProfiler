namespace EntityProfiler.Interceptor.Core {
    using System;
    using System.Data.Common;
    using System.Data.Entity.Infrastructure.Interception;
    using Common.Protocol;

    internal interface IDbCommandMessageFactory {
        void BeginCreateMessage<TResult>(DbCommand dbCommand, DbCommandInterceptionContext<TResult> interceptionContext);
        Message EndCreateMessage<TResult>(DbCommand dbCommand, DbCommandInterceptionContext<TResult> interceptionContext);
    }

    internal sealed class DbCommandMessageFactory : IDbCommandMessageFactory {
        public void BeginCreateMessage<TResult>(DbCommand dbCommand, DbCommandInterceptionContext<TResult> interceptionContext) {
            
        }

        public Message EndCreateMessage<TResult>(DbCommand dbCommand, DbCommandInterceptionContext<TResult> interceptionContext) {
            throw new NotImplementedException();
        }
    }
}