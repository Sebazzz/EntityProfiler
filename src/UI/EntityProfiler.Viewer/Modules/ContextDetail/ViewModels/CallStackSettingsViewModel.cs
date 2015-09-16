using System.ComponentModel.Composition;
using Caliburn.Micro;
using EntityProfiler.Viewer.PresentationCore;
using EntityProfiler.Viewer.Properties;
using Gemini.Modules.Settings;

namespace EntityProfiler.Viewer.Modules.ContextDetail.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CallStackSettingsViewModel : PropertyChangedBase, ISettingsEditor
    {
        private bool _openFilesInVisualStudio;
        private bool _allowAnyVsInstance;

        public CallStackSettingsViewModel()
        {
            OpenFilesInVisualStudio = Settings.Default.Profiler_CallStack_OpenFilesInVisualStudio;
            AllowAnyVsInstance = Settings.Default.Profiler_CallStack_AllowAnyVsInstance;
        }

        public string SettingsPageName
        {
            get { return EpvConstants.SettingsPages.Profiler_CallStack; }
        }

        public string SettingsPagePath
        {
            get { return EpvConstants.SettingsPaths.Profiler; }
        }

        public bool OpenFilesInVisualStudio
        {
            get { return _openFilesInVisualStudio; }
            set
            {
                if (value == _openFilesInVisualStudio) return;
                _openFilesInVisualStudio = value;
                NotifyOfPropertyChange();
            }
        }

        public bool AllowAnyVsInstance
        {
            get { return _allowAnyVsInstance; }
            set
            {
                if (value == _allowAnyVsInstance) return;
                _allowAnyVsInstance = value;
                NotifyOfPropertyChange();
            }
        }

        public void ApplyChanges()
        {
            Settings.Default.Profiler_CallStack_OpenFilesInVisualStudio = OpenFilesInVisualStudio;
            Settings.Default.Profiler_CallStack_AllowAnyVsInstance = AllowAnyVsInstance;
            Settings.Default.Save();
        }

        public void DiscardChanges()
        {
        }
    }
}