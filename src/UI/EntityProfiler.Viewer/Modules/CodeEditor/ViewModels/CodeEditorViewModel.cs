using System;
using System.ComponentModel.Composition;
using System.IO;
using EntityProfiler.Viewer.Modules.CodeEditor.Views;
using Gemini.Framework;

namespace EntityProfiler.Viewer.Modules.CodeEditor.ViewModels
{
    [Export(typeof(CodeEditorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
#pragma warning disable 659
    public class CodeEditorViewModel : Document
#pragma warning restore 659
    {
        private readonly LanguageDefinitionManager _languageDefinitionManager;
        private string _originalText;
        private string _path;
        private string _fileName;
        private bool _isDirty;
        private ICodeEditorView _view;
        private Action<ICodeEditorView> _viewCallback;
        private readonly Guid _instanceId;

        [ImportingConstructor]
        public CodeEditorViewModel(LanguageDefinitionManager languageDefinitionManager)
        {
            _instanceId = Guid.NewGuid();
            _languageDefinitionManager = languageDefinitionManager;
        }

        public string Path
        {
            get { return _path; }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (value == _isDirty)
                    return;

                _isDirty = value;
                NotifyOfPropertyChange(() => IsDirty);
                UpdateDisplayName();
            }
        }

        public override bool ShouldReopenOnStart
        {
            get { return false; }
        }

        public override void CanClose(System.Action<bool> callback)
        {
            callback(!IsDirty);
        }

        public void New(string name)
        {
            _fileName = name;
            UpdateDisplayName();
        }

        public void Open(string fullPath, Action<ICodeEditorView> viewCallback = null)
        {
            _path = fullPath;
            _fileName = System.IO.Path.GetFileName(_path);
            _viewCallback = viewCallback;
            UpdateDisplayName();

            UpdateTextEditor(fullPath);
        }

        public void Open(string text, string newFileName, Action<ICodeEditorView> viewCallback = null)
        {
            _originalText = text;
            _path = newFileName;
            _fileName = System.IO.Path.GetFileName(_path);
            _viewCallback = viewCallback;
            UpdateDisplayName();

            UpdateTextEditor(newFileName);
        }

        public override void SaveState(BinaryWriter writer)
        {
            writer.Write(_path);
        }

        public override void LoadState(BinaryReader reader)
        {
            Open(reader.ReadString());
        }

        private void UpdateDisplayName()
        {
            DisplayName = (IsDirty) ? _fileName + "*" : _fileName;
        }

        protected override void OnViewLoaded(object view)
        {
            _view = (ICodeEditorView)view;

            UpdateTextEditor();

            _view.TextEditor.TextChanged += delegate
            {
                IsDirty = string.Compare(_originalText, _view.TextEditor.Text, StringComparison.InvariantCulture) != 0;
            };
        }

        private void UpdateTextEditor(string newPath = null)
        {
            if (_view == null)
                return;

            if (newPath != null && newPath.Equals(_path))
            {
                if (_viewCallback != null)
                    _viewCallback(_view);
                return;
            }

            if (_path != null && File.Exists(_path))
            {
                using (var stream = File.OpenText(_path))
                {
                    _originalText = stream.ReadToEnd();
                }
            }

            _view.TextEditor.Text = _originalText;

            if (!string.IsNullOrEmpty(_fileName))
            {
                var fileExtension = System.IO.Path.GetExtension(_fileName).ToLower();
                var languageDefinition = _languageDefinitionManager.GetDefinitionByExtension(fileExtension);

                SetLanguage(languageDefinition);
            }

            if (_viewCallback != null)
                _viewCallback(_view);
        }

        public override bool Equals(object obj)
        {
            var other = obj as CodeEditorViewModel;
            if (other == null)
                return false;
            if (!string.IsNullOrEmpty(_path) && !string.IsNullOrEmpty(other._path))
                return string.Compare(_path, other._path, StringComparison.InvariantCulture) == 0;
            return _instanceId == other._instanceId;
        }

        public void Save()
        {
            var newText = _view.TextEditor.Text;
            File.WriteAllText(_path, newText);
            _originalText = newText;

            IsDirty = false;
        }

        private void SetLanguage(ILanguageDefinition languageDefinition)
        {
            if (languageDefinition == null)
                _view.TextEditor.SyntaxHighlighting = null;
            else
                _view.TextEditor.SyntaxHighlighting = languageDefinition.SyntaxHighlighting;
        }
    }
}