namespace EntityProfiler.Interceptor.Core {
    using System;
    using System.Data.Common;
    using System.Data.Entity;

    struct DbReaderCommandInterceptionData : IInterceptionData<DbDataReader> {
        public DbContext DbContext { get; set; }

        public Exception Error { get; set; }

        public DbCommand DbCommand { get; set; }

        public DbDataReader Result { get; set; }

        public int NumberOfRows { get; set; }

        public bool IsAsync { get; set; }
    }
}