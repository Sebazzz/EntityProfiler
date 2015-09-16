using System.Windows.Controls;

namespace EntityProfiler.Viewer.Modules.QueryTools.Views
{
    public interface IResultSetView
    {
        TextBox TextBox { get; }
        DataGrid DataGrid { get; }
    }
}