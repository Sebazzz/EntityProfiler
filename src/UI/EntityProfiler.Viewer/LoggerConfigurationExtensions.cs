using System;
using EntityProfiler.Viewer.Modules.Output;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace EntityProfiler.Viewer
{
    public static class LoggerConfigurationExtensions
    {
        private const string DefaultOutputTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

        private const string DefaultConsoleOutputTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}";

        private const long DefaultFileSizeLimitBytes = 1L*1024*1024*1024;
        private const int DefaultRetainedFileCountLimit = 31; // A long month of logs

        /// <summary>
        ///     Writes log events to <see cref="IOutput" />.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="factory"></param>
        /// <param name="outputLogFilterProvider"></param>
        /// <param name="restrictedToMinimumLevel">
        ///     The minimum level for
        ///     events passed through the sink.
        /// </param>
        /// <param name="outputTemplate">
        ///     A message template describing the format used to write to the sink.
        ///     the default is "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".
        /// </param>
        /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration OutputModule(this LoggerSinkConfiguration sinkConfiguration,
            Func<IOutput> factory, Func<IOutputLogFilter> outputLogFilterProvider = null, LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultConsoleOutputTemplate, IFormatProvider formatProvider = null)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException("sinkConfiguration");
            if (outputTemplate == null) throw new ArgumentNullException("outputTemplate");
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return sinkConfiguration.Sink(new OutputSink(factory, formatter, outputLogFilterProvider), restrictedToMinimumLevel);
        }
    }
}