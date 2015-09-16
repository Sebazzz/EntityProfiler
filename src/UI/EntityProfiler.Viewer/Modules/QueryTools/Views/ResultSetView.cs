using System.Windows.Controls;

namespace EntityProfiler.Viewer.Modules.QueryTools.Views
{
    public class ResultSetView : IResultSetView
    {
        public ResultSetView(TextBox textBox)
        {
            TextBox = textBox;
        }

        public ResultSetView(DataGrid dataGrid)
        {
            DataGrid = dataGrid;
        }

        public TextBox TextBox { get; private set; }
        public DataGrid DataGrid { get; private set; }
    }
}