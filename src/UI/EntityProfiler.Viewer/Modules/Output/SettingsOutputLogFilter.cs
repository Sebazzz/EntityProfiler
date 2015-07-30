using System.ComponentModel.Composition;
using EntityProfiler.Viewer.Properties;
using Serilog.Events;

namespace EntityProfiler.Viewer.Modules.Output
{
    [Export(typeof(IOutputLogFilter))]
    public class SettingsOutputLogFilter : IOutputLogFilter
    {
        private bool? _disableLog;
        private LogEventLevel? _minLogLevel;

        public bool DisableLog
        {
            get
            {
                if (!_disableLog.HasValue)
                    _disableLog = Settings.Default.Output_DisableLog;
                return _disableLog ?? false;
            }
        }

        private LogEventLevel MinLogLevel
        {
            get
            {
                if (!_minLogLevel.HasValue)
                    _minLogLevel = Settings.Default.Output_MinLogLevel;
                return _minLogLevel ?? LogEventLevel.Information;
            }
        }

        public void InvalidateCache()
        {
            _disableLog = null;
            _minLogLevel = null;
        }

        public bool Filter(LogEvent logEvent)
        {
            if(DisableLog)
                return false;

            if (logEvent.Level < MinLogLevel)
                return false;

            return true;
        }
    }
}