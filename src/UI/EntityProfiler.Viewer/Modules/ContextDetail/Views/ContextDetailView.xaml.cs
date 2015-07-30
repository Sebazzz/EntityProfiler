using System.Windows.Controls;
using ICSharpCode.AvalonEdit;

namespace EntityProfiler.Viewer.Modules.ContextDetail
{
    /// <summary>
    /// Interaction logic for ContextDetailView.xaml
    /// </summary>
    public partial class ContextDetailView : UserControl, IContextDetailView
    {
        public ContextDetailView()
        {
            InitializeComponent();
        }

        public TextEditor TextEditor
        {
            get { return SqlEditor; }
        }

        public DataGrid DataGrid
        {
            get { return ParametersDataGrid; }
        }
    }
}
