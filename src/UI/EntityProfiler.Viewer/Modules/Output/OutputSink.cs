using System;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using System.ComponentModel.Composition;

namespace EntityProfiler.Viewer.Modules.Output
{
    [Export(typeof(ILogEventSink))]
    public class OutputSink : ILogEventSink
    {
        readonly ITextFormatter _textFormatter;
        readonly object _syncRoot = new object();
        private readonly Func<IOutput> _outputProvider;
        private readonly Func<IOutputLogFilter> _outputLogFilterProvider;
        private IOutput _output;
        private IOutputLogFilter _outputLogFilter;

        private IOutput Output
        {
            get { return _output ?? (_output = _outputProvider()); }
        }

        private IOutputLogFilter OutputLogFilter
        {
            get
            {
                if (_outputLogFilterProvider == null)
                    return null;
                return _outputLogFilter ?? (_outputLogFilter = _outputLogFilterProvider());
            }
        }

        public OutputSink(Func<IOutput> outputProvider, ITextFormatter textFormatter, Func<IOutputLogFilter> outputLogFilterProvider = null)
        {
            if (textFormatter == null) throw new ArgumentNullException("textFormatter");
            _textFormatter = textFormatter;
            _outputProvider = outputProvider;
            _outputLogFilterProvider = outputLogFilterProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException("logEvent");
            if(Output == null)
                return;

            var outputLogFilter = OutputLogFilter;
            if (outputLogFilter != null)
            {
                var filter = outputLogFilter.Filter(logEvent);
                if(!filter)
                    return;
            }

            var textWriter = Output.Writer;
            lock (_syncRoot)
            {
                _textFormatter.Format(logEvent, textWriter);
                textWriter.Flush();
            }
        }
    }
}