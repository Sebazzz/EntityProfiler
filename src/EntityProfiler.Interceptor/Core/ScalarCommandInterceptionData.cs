namespace EntityProfiler.Interceptor.Core {
    using System;
    using System.Data.Common;
    using System.Data.Entity;

    struct ScalarCommandInterceptionData : IInterceptionData<object> {
        public DbContext DbContext { get; set; }

        public Exception Error { get; set; }

        public DbCommand DbCommand { get; set; }

        public object Result { get; set; }

        public bool IsAsync { get; set; }
    }
}