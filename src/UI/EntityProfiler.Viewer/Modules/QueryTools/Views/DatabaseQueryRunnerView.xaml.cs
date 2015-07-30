using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EntityProfiler.Viewer.Modules.QueryTools.ViewModels;
using ICSharpCode.AvalonEdit;

namespace EntityProfiler.Viewer.Modules.QueryTools.Views
{
    /// <summary>
    /// Interaction logic for DatabaseQueryRunnerView.xaml
    /// </summary>
    public partial class DatabaseQueryRunnerView : UserControl, IDatabaseQueryRunnerView
    {
        private DataTemplate _resultSetsDataGridDataTemplate;
        private DataTemplate _resultSetsTextBoxDataTemplate;
        private DataTemplate _resultSetsPropertyGridDataTemplate;

        public DatabaseQueryRunnerView()
        {
            Resources["ResultSetsDataTemplateSelector"] = new DatabaseQueryResultSetDataTemplateSelector(this);
            InitializeComponent();
        }

        public TextEditor TextEditor
        {
            get { return SqlEditor; }
        }
        
        public DataTemplate ResultSetsDataGridDataTemplate
        {
            get {
                return _resultSetsDataGridDataTemplate ??
                       (_resultSetsDataGridDataTemplate = Resources["ResultSetsDataGridDataTemplate"] as DataTemplate);
            }
        }

        public DataTemplate ResultSetsTextBoxDataTemplate
        {
            get {
                return _resultSetsTextBoxDataTemplate ??
                       (_resultSetsTextBoxDataTemplate = Resources["ResultSetsTextBoxDataTemplate"] as DataTemplate);
            }
        }

        public DataTemplate ResultSetsExceptionDataTemplate
        {
            get {
                return _resultSetsPropertyGridDataTemplate ??
                       (_resultSetsPropertyGridDataTemplate =
                           Resources["ResultSetsExceptionDataTemplate"] as DataTemplate);
            }
        }
    }
}
