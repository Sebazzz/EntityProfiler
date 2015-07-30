using Serilog.Events;

namespace EntityProfiler.Viewer.Modules.Output
{
    public interface IOutputLogFilter
    {
        void InvalidateCache();
        bool Filter(LogEvent logEvent);
    }
}