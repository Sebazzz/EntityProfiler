namespace EntityProfiler.Interceptor.Core {
    using System;
    using System.Collections.Concurrent;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Diagnostics;
    using Common.Annotations;
    using Common.Protocol;
    using Protocol;

    internal interface IInterceptorLog {
        void OnNonQueryBegin(DbCommand cmd, [CanBeNull] DbContext dbContext);
        void OnNonQueryEnd(NonQueryCommandInterceptionData data);
        void OnScalarQueryBegin(DbCommand cmd, [CanBeNull] DbContext dbContext);
        void OnScalarQueryEnd(ScalarCommandInterceptionData data);
        void OnDbReaderQueryBegin(DbCommand cmd, [CanBeNull] DbContext dbContext);
        void OnDbReaderQueryEnd(DbReaderCommandInterceptionData data);
    }

    /// <summary>
    /// Represents an internal logging back-end which profiles in interception events and sends 
    /// </summary>
    internal class InterceptorLog : IInterceptorLog {
        private readonly ConcurrentDictionary<DbCommand, InterceptionContext<NonQueryCommandInterceptionData, int>> _nonQueryContexts;
        private readonly ConcurrentDictionary<DbCommand, InterceptionContext<ScalarCommandInterceptionData, object>> _scalarQueryContexts;
        private readonly ConcurrentDictionary<DbCommand, InterceptionContext<DbReaderCommandInterceptionData, DbDataReader>> _dbReaderQueryContexts;

        private readonly IMessageSink _messageSink;
        private readonly IExecutionContextFactory _executionContextFactory;
        private readonly IQueryDataFactory _queryDataFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InterceptorLog(IMessageSink messageSink, IExecutionContextFactory executionContextFactory, IQueryDataFactory queryDataFactory) {
            this._messageSink = messageSink;
            this._messageSink.Start();

            this._executionContextFactory = executionContextFactory;
            this._queryDataFactory = queryDataFactory;

            this._nonQueryContexts = new ConcurrentDictionary<DbCommand, InterceptionContext<NonQueryCommandInterceptionData, int>>();
            this._scalarQueryContexts = new ConcurrentDictionary<DbCommand, InterceptionContext<ScalarCommandInterceptionData, object>>();
            this._dbReaderQueryContexts = new ConcurrentDictionary<DbCommand, InterceptionContext<DbReaderCommandInterceptionData, DbDataReader>>();
        }


        public void OnNonQueryBegin(DbCommand cmd, DbContext dbContext) {
            this._nonQueryContexts.TryAdd(
                cmd,
                new InterceptionContext<NonQueryCommandInterceptionData, int>(
                    this._executionContextFactory.Create(cmd.Connection, dbContext)));
        }

        public void OnNonQueryEnd(NonQueryCommandInterceptionData data) {
            InterceptionContext<NonQueryCommandInterceptionData, int> context;
            if (!this._nonQueryContexts.TryGetValue(data.DbCommand, out context)) {
                // we cannot find the context, so there is nothing to log.
                // note this shouldn't actually happen, since, even in the case of an error,
                // this method should always be called
                return;
            }

            context.TimeCounter.Stop();
            context.Data = data;

            // create query message
            NonQueryMessage qm = new NonQueryMessage();
            qm.AffectedRecords = context.Data.Result;

            this.FillUp(context, qm);
            this.PostMessage(qm);
        }

        public void OnScalarQueryBegin(DbCommand cmd, DbContext dbContext) {
            this._scalarQueryContexts.TryAdd(cmd,
                new InterceptionContext<ScalarCommandInterceptionData, object>(
                    this._executionContextFactory.Create(cmd.Connection, dbContext)));
        }

        public void OnScalarQueryEnd(ScalarCommandInterceptionData data) {
            InterceptionContext<ScalarCommandInterceptionData, object> context;
            if (!this._scalarQueryContexts.TryGetValue(data.DbCommand, out context)) {
                // we cannot find the context, so there is nothing to log.
                // note this shouldn't actually happen, since, even in the case of an error,
                // this method should always be called
                return;
            }
            context.TimeCounter.Stop();
            context.Data = data;

            // create query message
            ScalarQueryMessage qm = ScalarQueryMessage.Create(data.Result);

            this.FillUp(context, qm);
            this.PostMessage(qm);
        }

        public void OnDbReaderQueryBegin(DbCommand cmd, DbContext dbContext) {
            this._dbReaderQueryContexts.TryAdd(cmd,
                new InterceptionContext<DbReaderCommandInterceptionData, DbDataReader>(
                    this._executionContextFactory.Create(cmd.Connection, dbContext)));
        }

        public void OnDbReaderQueryEnd(DbReaderCommandInterceptionData data) {
            InterceptionContext<DbReaderCommandInterceptionData, DbDataReader> context;
            if (!this._dbReaderQueryContexts.TryGetValue(data.DbCommand, out context)) {
                // we cannot find the context, so there is nothing to log.
                // note this shouldn't actually happen, since, even in the case of an error,
                // this method should always be called
                return;
            }

            context.TimeCounter.Stop();
            context.Data = data;

            // create query message
            DbReaderQueryMessage qm = new DbReaderQueryMessage();
            qm.RecordCount = data.Result != null ? data.Result.RecordsAffected : -1;

            this.FillUp(context, qm);
            this.PostMessage(qm);
        }

        private void PostMessage(QueryMessage queryMessage) {
            this._messageSink.DispatchMessage(queryMessage);
        }

        private void FillUp<TData, TResult>(InterceptionContext<TData, TResult> context, QueryMessage message) where TData : IInterceptionData<TResult> {
            message.Error = context.Data.Error != null ? context.Data.Error.ToString() : null;
            message.Performance = CreatePerformanceData(context);
            message.Query = this._queryDataFactory.CreateQuery(context.Data.DbCommand);
            message.Context = context.ExecutionContext;
        }

        private static PerformanceData CreatePerformanceData<TData, TResult>(InterceptionContext<TData, TResult> context) where TData : IInterceptionData<TResult> {
            return new PerformanceData {
                TotalTime = context.TimeCounter.ElapsedMilliseconds
            };
        }
    }


    internal struct InterceptionContext<TData, TResult> where TData : IInterceptionData<TResult> {
        
        public TData Data { get; set; }

        public Stopwatch TimeCounter { get; set; }

        public ExecutionContext ExecutionContext { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public InterceptionContext(ExecutionContext executionContext) : this() {
            this.ExecutionContext = executionContext;

            this.TimeCounter = new Stopwatch();
            this.TimeCounter.Start();
        }
    }
}