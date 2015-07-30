using System.ComponentModel.Composition;
using Caliburn.Micro;
using EntityProfiler.Viewer.Properties;
using Gemini;
using Gemini.Modules.Settings;
using Serilog.Events;

namespace EntityProfiler.Viewer.Modules.Output.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutputSettingsViewModel : PropertyChangedBase, ISettingsEditor
    {
        private readonly IOutputLogFilter _outputLogFilter;
        private bool _disableLog;
        private LogEventLevel _minLogLevel;

        [ImportingConstructor]
        public OutputSettingsViewModel(IOutputLogFilter outputLogFilter)
        {
            _outputLogFilter = outputLogFilter;

            DisableLog = Settings.Default.Output_DisableLog;
            MinLogLevel = Settings.Default.Output_MinLogLevel;
        }

        public string SettingsPageName
        {
            get { return EpvConstants.SettingsPages.Output; }
        }
        
        public string SettingsPagePath
        {
            get { return GeminiConstants.SettingsPaths.Environment; }
        }

        public bool DisableLog
        {
            get { return _disableLog; }
            set
            {
                if (value == _disableLog) return;
                _disableLog = value;
                NotifyOfPropertyChange();
            }
        }

        public LogEventLevel MinLogLevel
        {
            get { return _minLogLevel; }
            set
            {
                if (value == _minLogLevel) return;
                _minLogLevel = value;
                NotifyOfPropertyChange();
            }
        }

        public void ApplyChanges()
        {
            Settings.Default.Output_DisableLog = DisableLog;
            Settings.Default.Output_MinLogLevel = MinLogLevel;
            Settings.Default.Save();
            _outputLogFilter.InvalidateCache();
        }

        public void DiscardChanges()
        {
        }
    }
}