namespace EntityProfiler.Common.Protocol {
    /// <summary>
    /// Represents performance data about a <see cref="QueryMessage"/>
    /// </summary>
    public class PerformanceData {
        /// <summary>
        /// Gets the total time in milliseconds of the query - including roundtrip time
        /// </summary>
        public long TotalTime { get; set; }
    }
}