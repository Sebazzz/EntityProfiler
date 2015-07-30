using System.ComponentModel.Composition;
using Caliburn.Micro;
using EntityProfiler.Viewer.Properties;
using Gemini;
using Gemini.Modules.Settings;

namespace EntityProfiler.Viewer.Modules.Connection.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConnectionSettingsViewModel : PropertyChangedBase, ISettingsEditor
    {
        private bool _autoStartWhenInitialized;
        private int _retryConnectInterval;
        private int _maxRetryConnect;

        public ConnectionSettingsViewModel()
        {
            AutoStartWhenInitialized = Settings.Default.Connection_AutoStartWhenInitialized;
            RetryConnectInterval = Settings.Default.Connection_RetryConnectInterval;
            MaxRetryConnect = Settings.Default.Connection_MaxRetryConnect;
        }

        public string SettingsPageName
        {
            get { return EpvConstants.SettingsPages.Connection; }
        }

        public string SettingsPagePath
        {
            get { return GeminiConstants.SettingsPaths.Environment; }
        }

        // start when initialized
        public bool AutoStartWhenInitialized
        {
            get { return _autoStartWhenInitialized; }
            set
            {
                if (value == _autoStartWhenInitialized) return;
                _autoStartWhenInitialized = value;
                NotifyOfPropertyChange();
            }
        }

        public int RetryConnectInterval
        {
            get { return _retryConnectInterval; }
            set
            {
                if (value == _retryConnectInterval) return;
                _retryConnectInterval = value;
                NotifyOfPropertyChange();
            }
        }

        public int MaxRetryConnect
        {
            get { return _maxRetryConnect; }
            set
            {
                if (value == _maxRetryConnect) return;
                _maxRetryConnect = value;
                NotifyOfPropertyChange();
            }
        }

        public void ApplyChanges()
        {
            Settings.Default.Connection_AutoStartWhenInitialized = AutoStartWhenInitialized;
            Settings.Default.Connection_RetryConnectInterval = RetryConnectInterval;
            Settings.Default.Connection_MaxRetryConnect = MaxRetryConnect;
            Settings.Default.Save();
        }

        public void DiscardChanges()
        {
        }
    }
}