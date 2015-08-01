using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;

namespace EntityProfiler.Viewer.Modules.ContextDetail
{
    /// <summary>
    /// Interaction logic for ContextDetailView.xaml
    /// </summary>
    public partial class ContextDetailView : UserControl, IContextDetailView
    {
        private bool _hasParameters;
        private GridLength _lastGridLength = new GridLength(1, GridUnitType.Star);

        public ContextDetailView()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                UpdateParametersLayout();
            };
        }

        public void ScrollIntoSelectedQuery()
        {
            Queries.ScrollIntoView(Queries.SelectedItem);
        }

        public bool HasParameters
        {
            get { return _hasParameters; }
            set
            {
                if (value == _hasParameters) return;
                _hasParameters = value;
                UpdateParametersLayout();
            }
        }

        private void UpdateParametersLayout()
        {
            QueryResultsGridSplitter.Visibility = _hasParameters ? Visibility.Visible : Visibility.Collapsed;
            DataGrid.Visibility = _hasParameters ? Visibility.Visible : Visibility.Collapsed;
            SetColumnDefinition(_hasParameters);
            QueryResultsGrid.ColumnDefinitions[0].Width = QueryResultsGrid.ColumnDefinitions[0].Width;
            TextEditor.TextArea.TextView.Redraw(DispatcherPriority.Background);
        }

        private void SetColumnDefinition(bool hasParameters)
        {
            var column = ParametersGridColumn;
            if (column.Width.IsStar)
            {
                _lastGridLength = column.Width;
            }
            column.Width = hasParameters ? _lastGridLength : new GridLength(0, GridUnitType.Pixel);
        }

        public static readonly DependencyProperty QueriesSummaryOpacityProperty = DependencyProperty.Register(
            "QueriesSummaryOpacity", typeof (double), typeof (ContextDetailView), new PropertyMetadata(1.0));

        public double QueriesSummaryOpacity
        {
            get { return (double) GetValue(QueriesSummaryOpacityProperty); }
            set { SetValue(QueriesSummaryOpacityProperty, value); }
        }

        public TextEditor TextEditor
        {
            get { return SqlEditor; }
        }

        public DataGrid DataGrid
        {
            get { return ParametersDataGrid; }
        }

        private void FrameworkElement_OnToolTipOpening(object sender, ToolTipEventArgs e)
        {
            QueriesSummaryOpacity = 0.4;
        }

        private void FrameworkElement_OnToolTipClosing(object sender, ToolTipEventArgs e)
        {
            QueriesSummaryOpacity = 1;
        }
    }
}
