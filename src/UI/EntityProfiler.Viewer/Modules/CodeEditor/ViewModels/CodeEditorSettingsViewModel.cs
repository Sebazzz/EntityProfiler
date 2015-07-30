using System.ComponentModel.Composition;
using Caliburn.Micro;
using EntityProfiler.Viewer.PresentationCore;
using EntityProfiler.Viewer.Properties;
using Gemini.Modules.Settings;

namespace EntityProfiler.Viewer.Modules.CodeEditor.ViewModels
{
    [Export(typeof(ISettingsEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CodeEditorSettingsViewModel : PropertyChangedBase, ISettingsEditor
    {
        public CodeEditorSettingsViewModel()
        {
            CodeEditorOptions = new ExtendedTextEditorOptions(Settings.Default.TextEditor_CodeEditorOptions);
        }

        public string SettingsPageName
        {
            get { return EpvConstants.SettingsPages.TextEditor_CodeEditor; }
        }

        public string SettingsPagePath
        {
            get { return EpvConstants.SettingsPaths.TextEditor; }
        }

        public ExtendedTextEditorOptions CodeEditorOptions { get; private set; }

        public void ResetDefault()
        {
            CodeEditorOptions = new ExtendedTextEditorOptions();
            NotifyOfPropertyChange(() => CodeEditorOptions);
        }

        public void ApplyChanges()
        {
            Settings.Default.TextEditor_CodeEditorOptions.CopyFrom(CodeEditorOptions);
            Settings.Default.Save();
        }

        public void DiscardChanges()
        {
        }
    }
}