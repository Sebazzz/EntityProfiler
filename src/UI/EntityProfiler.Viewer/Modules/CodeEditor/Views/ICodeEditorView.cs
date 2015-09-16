using ICSharpCode.AvalonEdit;

namespace EntityProfiler.Viewer.Modules.CodeEditor.Views
{
    public interface ICodeEditorView
    {
        TextEditor TextEditor { get; } 
    }
}