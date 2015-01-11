namespace EntityProfiler.UI.Services {
    using System.Collections.Generic;
    using Common.Protocol;
    using Interceptor.Reader.Core;

    internal static class SeedData {
        public static IEnumerable<QueryMessage> Queries() {
            yield return new DbReaderQueryMessage() {
                Context = new ExecutionContext(1, "GET /test.aspx"),
                Performance = new PerformanceData() {TotalTime = 10},
                Query = new Query() {CommandText = "SELECT ..."},
                RecordCount = 10
            };

            yield return new DbReaderQueryMessage() {
                Context = new ExecutionContext(1, "GET /test.aspx"),
                Performance = new PerformanceData() {TotalTime = 10},
                Query = new Query() {CommandText = "SELECT ..."},
                RecordCount = 10
            };

            yield return new DbReaderQueryMessage() {
                Context = new ExecutionContext(1, "GET /test.aspx"),
                Performance = new PerformanceData() {TotalTime = 10},
                Query = new Query() {CommandText = "SELECT ..."},
                RecordCount = 10
            };

            yield return new DuplicateQueryMessage {
                Context = new ExecutionContext(1, "GET /test.aspx"),
                Performance = new AggregatePerformanceData {TotalTime = 10},
                Query = new AggregateQuery() {CommandText = "SELECT ..."},
                NumberOfQueries = 6
            };

             yield return new DbReaderQueryMessage() {
                Context = new ExecutionContext(1, "GET /test.aspx"),
                Performance = new PerformanceData() {TotalTime = 10},
                Query = new Query() {CommandText = "SELECT ..."},
                RecordCount = 10
            };
        } 
    }
}