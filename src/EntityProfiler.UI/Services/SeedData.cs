namespace EntityProfiler.UI.Services {
    using System.Collections.Generic;
    using Common.Protocol;

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

            yield return new DbReaderQueryMessage() {
                Context = new ExecutionContext(1, "GET /test.aspx"),
                Performance = new PerformanceData() {TotalTime = 10},
                Query = new Query() {CommandText = "SELECT ..."},
                RecordCount = 10
            };
        } 
    }
}