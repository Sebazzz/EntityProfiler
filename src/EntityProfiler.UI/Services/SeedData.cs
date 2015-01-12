namespace EntityProfiler.UI.Services {
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using Common.Protocol;
    using Common.Utils;
    using Interceptor.Reader.Core;
    using ViewModels;

    internal static class SeedData {
        public static IEnumerable<DataContextViewModel> DataContexts() {
            yield return new DataContextViewModel() {
                Identifier = new ContextIdentifier(DateTime.UtcNow, 1),
                Description = "GET /Test/123.aspx"
            };

            yield return new DataContextViewModel() {
                Identifier = new ContextIdentifier(DateTime.UtcNow, 2),
                Description = "POST /Test/123.aspx"
            };
        }

        public static IEnumerable<QueryMessageViewModel> Queries() {
            foreach (SmartEnumerable<QueryMessage>.Entry message in QueriesInternal().AsSmartEnumerable()) {
                yield return new QueryMessageViewModel() {
                    Index = message.Index,
                    Model = message.Value
                };
            }
        }

        private static IEnumerable<QueryMessage> QueriesInternal() {
            ContextIdentifier ctx = new ContextIdentifier(DateTime.UtcNow, 1);

            yield return new DbReaderQueryMessage() {
                Context = new ExecutionContext(ctx, "GET /test.aspx"),
                Performance = new PerformanceData() {TotalTime = 10},
                Query = new Query() {CommandText = "SELECT ..."},
                RecordCount = 10
            };

            yield return new DbReaderQueryMessage() {
                Context = new ExecutionContext(ctx, "GET /test.aspx"),
                Performance = new PerformanceData() {TotalTime = 10},
                Query = new Query() {CommandText = "SELECT ..."},
                RecordCount = 10
            };

            yield return new DbReaderQueryMessage() {
                Context = new ExecutionContext(ctx, "GET /test.aspx"),
                Performance = new PerformanceData() {TotalTime = 10},
                Query = new Query() {CommandText = "SELECT ..."},
                RecordCount = 10
            };

            yield return new DuplicateQueryMessage {
                Context = new ExecutionContext(ctx, "GET /test.aspx"),
                Performance = new AggregatePerformanceData {TotalTime = 10},
                Query = new AggregateQuery() {CommandText = "SELECT ..."},
                NumberOfQueries = 6
            };

             yield return new DbReaderQueryMessage() {
                Context = new ExecutionContext(ctx, "GET /test.aspx"),
                Performance = new PerformanceData() {TotalTime = 10},
                Query = new Query() {CommandText = "SELECT ..."},
                RecordCount = 10
            };
        } 
    }
}