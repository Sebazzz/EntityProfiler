using System.ComponentModel.Composition;
using Caliburn.Micro;
using EntityProfiler.Viewer.PresentationCore;
using EntityProfiler.Viewer.Properties;
using Gemini.Modules.Settings;

namespace EntityProfiler.Viewer.Modules.CodeEditor.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SqlEditorSettingsViewModel : PropertyChangedBase, ISettingsEditor
    {
        public SqlEditorSettingsViewModel()
        {
            SqlEditorOptions = new ExtendedTextEditorOptions(Settings.Default.TextEditor_SqlEditorOptions);
        }

        public string SettingsPageName
        {
            get { return EpvConstants.SettingsPages.TextEditor_SqlEditor; }
        }

        public string SettingsPagePath
        {
            get { return EpvConstants.SettingsPaths.TextEditor; }
        }
        
        public ExtendedTextEditorOptions SqlEditorOptions { get; private set; }

        public void ResetDefault()
        {
            SqlEditorOptions = new ExtendedTextEditorOptions();
            NotifyOfPropertyChange(()=> SqlEditorOptions);
        }

        public void ApplyChanges()
        {
            Settings.Default.TextEditor_SqlEditorOptions.CopyFrom(SqlEditorOptions);
            Settings.Default.Save();
        }

        public void DiscardChanges()
        {
        }
    }
}