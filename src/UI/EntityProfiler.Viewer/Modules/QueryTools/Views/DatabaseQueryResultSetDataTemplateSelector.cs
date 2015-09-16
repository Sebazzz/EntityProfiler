using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace EntityProfiler.Viewer.Modules.QueryTools.Views
{
    public class DatabaseQueryResultSetDataTemplateSelector : DataTemplateSelector
    {
        private readonly DatabaseQueryRunnerView _databaseQueryRunnerView;

        public DatabaseQueryResultSetDataTemplateSelector(DatabaseQueryRunnerView databaseQueryRunnerView)
        {
            _databaseQueryRunnerView = databaseQueryRunnerView;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var executeResults = item as ResultsSetModel;
            if (executeResults != null)
            {
                if (executeResults.ExceptionDetails != null)
                {
                    return _databaseQueryRunnerView.ResultSetsExceptionDataTemplate;
                }
                if (executeResults.ResultsData is DataTable || executeResults.ResultsData is DataView)
                {
                    return _databaseQueryRunnerView.ResultSetsDataGridDataTemplate;
                }
                if (executeResults.ResultsData is string)
                {
                    return _databaseQueryRunnerView.ResultSetsTextBoxDataTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}