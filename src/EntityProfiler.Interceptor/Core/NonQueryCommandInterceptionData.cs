namespace EntityProfiler.Interceptor.Core {
    using System;
    using System.Data.Common;
    using System.Data.Entity;

    struct NonQueryCommandInterceptionData : IInterceptionData<int> {
        public DbContext DbContext { get; set; }

        public Exception Error { get; set; }

        public DbCommand DbCommand { get; set; }

        public int Result { get; set; }

        public bool IsAsync { get; set; }
    }
}