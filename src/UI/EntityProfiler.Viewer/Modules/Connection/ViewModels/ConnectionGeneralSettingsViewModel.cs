using System.ComponentModel.Composition;
using Caliburn.Micro;
using EntityProfiler.Viewer.Properties;
using Gemini;
using Gemini.Modules.Settings;

namespace EntityProfiler.Viewer.Modules.Connection.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConnectionGeneralSettingsViewModel : PropertyChangedBase, ISettingsEditor
    {
        private bool _confirmOnCloseConnected;

        public ConnectionGeneralSettingsViewModel()
        {
            ConfirmOnCloseConnected = Settings.Default.Connection_ConfirmOnCloseConnected;
        }

        public bool ConfirmOnCloseConnected
        {
            get { return _confirmOnCloseConnected; }
            set
            {
                if (value == _confirmOnCloseConnected) return;
                _confirmOnCloseConnected = value;
                NotifyOfPropertyChange();
            }
        }

        public string SettingsPageName
        {
            get { return GeminiConstants.SettingsPages.General; }
        }

        public string SettingsPagePath
        {
            get { return GeminiConstants.SettingsPaths.Environment; }
        }

        public void ApplyChanges()
        {
            Settings.Default.Connection_ConfirmOnCloseConnected = ConfirmOnCloseConnected;
            Settings.Default.Save();
        }

        public void DiscardChanges()
        {
        }
    }
}