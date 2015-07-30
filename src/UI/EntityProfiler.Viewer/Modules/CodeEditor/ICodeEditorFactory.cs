using System;
using EntityProfiler.Viewer.Modules.CodeEditor.ViewModels;
using EntityProfiler.Viewer.Modules.CodeEditor.Views;

namespace EntityProfiler.Viewer.Modules.CodeEditor
{
    public interface ICodeEditorFactory
    {
        void OpenDocumentView(CodeEditorViewModel viewModel);
        CodeEditorViewModel Open(string fullPath, Action<ICodeEditorView> viewCallback = null);
        CodeEditorViewModel Open(string text, string fileName, Action<ICodeEditorView> viewCallback = null);
    }
}