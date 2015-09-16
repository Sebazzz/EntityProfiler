namespace EntityProfiler.Viewer.Modules.ContextDetail
{
    public interface IContextDetailView : ISqlEditorView, IParametersDataGridView
    {
        bool HasParameters { get; set; }
        void ScrollIntoSelectedQuery();
    }
}