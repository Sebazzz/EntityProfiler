using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using EntityProfiler.Viewer.Modules.CodeEditor.Views;
using Gemini.Framework.Services;

namespace EntityProfiler.Viewer.Modules.CodeEditor.ViewModels
{
    [Export(typeof(ICodeEditorFactory))]
    public class CodeEditorFactory : ICodeEditorFactory
    {
        private static readonly IDictionary<string, CodeEditorViewModel> _editors = 
            new Dictionary<string, CodeEditorViewModel>(); 

        private readonly IShell _shell;

        [ImportingConstructor]
        public CodeEditorFactory(IShell shell)
        {
            _shell = shell;
        }
        
        public virtual void OpenDocumentView(CodeEditorViewModel viewModel)
        {
            _shell.OpenDocument(viewModel);
        }
        
        public CodeEditorViewModel Open(string fullPath, Action<ICodeEditorView> viewCallback = null)
        {
            var codeEditorViewModel = Create(fullPath);
            codeEditorViewModel.Open(fullPath, viewCallback);
            OpenDocumentView(codeEditorViewModel);
            return codeEditorViewModel;
        }

        public CodeEditorViewModel Open(string text, string fileName, Action<ICodeEditorView> viewCallback = null)
        {
            var codeEditorViewModel = Create(fileName);
            codeEditorViewModel.Open(text, fileName, viewCallback);
            OpenDocumentView(codeEditorViewModel);
            return codeEditorViewModel;
        }

        protected virtual CodeEditorViewModel Create(string pathOrName)
        {
            if (_editors.ContainsKey(pathOrName))
                return _editors[pathOrName];

            var editorViewModel = IoC.Get<CodeEditorViewModel>();
            _editors.Add(pathOrName, editorViewModel);
            return editorViewModel;
        }
    }
}