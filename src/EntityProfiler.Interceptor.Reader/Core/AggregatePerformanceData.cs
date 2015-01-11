namespace EntityProfiler.Interceptor.Reader.Core {
    using System.Collections.Generic;
    using System.Linq;
    using Common.Protocol;

    /// <summary>
    /// Represents performance data that is aggegrated from multiple queries
    /// </summary>
    public sealed class AggregatePerformanceData : PerformanceData {
        /// <summary>
        /// Gets an array of captured <see cref="PerformanceData.TotalTime"/> for multiple queries
        /// </summary>
        public long[] Times { get; set; }

        /// <summary>
        /// Creates an <see cref="AggregatePerformanceData"/> instance based on the query list
        /// </summary>
        /// <param name="aggregate"></param>
        /// <returns></returns>
        public static AggregatePerformanceData Create(List<QueryMessage> aggregate) {
            AggregatePerformanceData performanceData = new AggregatePerformanceData();
            performanceData.Times = new long[aggregate.Count];
            for (int i = 0; i < aggregate.Count; i++) {
                performanceData.Times[i] = aggregate[i].Performance.TotalTime;
            }

            performanceData.TotalTime = performanceData.Times.Sum();

            return performanceData;
        }
    }
}