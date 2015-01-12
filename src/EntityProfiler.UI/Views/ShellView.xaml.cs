namespace EntityProfiler.UI.Views {
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Xml;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Highlighting;
    using ICSharpCode.AvalonEdit.Highlighting.Xshd;
    using ViewModels;

    partial class ShellView {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void OnAvalonLoaded(object sender, RoutedEventArgs e) {
            TextEditor textEditor = (TextEditor) sender;

            using (Stream s = typeof(ShellViewModel).Assembly.GetManifestResourceStream("EntityProfiler.UI.Resources.SQL.xshd")) {
                Debug.Assert(s != null);
                using (XmlTextReader reader = new XmlTextReader(s)) {
                    textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }

            ShellViewModel vm = (ShellViewModel) textEditor.DataContext;
            Debug.Assert(vm != null);

            vm.PropertyChanged += (_, ev) => {
                if (ev.PropertyName == "SelectedQuery") {
                    SetTextEditor(vm, textEditor);
                    SetDataGrid(vm);
                }
            };

            SetTextEditor(vm, textEditor);
        }

        private void SetDataGrid(ShellViewModel vm) {
            QueryMessageViewModel query = vm.SelectedQuery;
            if (query == null) {
                return;
            }


            Record first = query.Parameters.FirstOrDefault();
            if (first == null) {
                this.ParametersTab.Content = null;
                this.ParametersTab.Visibility = Visibility.Collapsed;
                this.ParametersTab.IsSelected = false;
                return;
            }

            DataGrid dataGrid = new DataGrid();
            dataGrid.AutoGenerateColumns = false;
            dataGrid.IsReadOnly = true;

            var columns = first
                .Properties
                .Select((x, i) => new {x.Name, Index = i})
                .ToArray();

            foreach (var column in columns)
            {
                var binding = new Binding(string.Format("Properties[{0}].Value", column.Index));

                dataGrid.Columns.Add(new DataGridTextColumn {Header = column.Name, Binding = binding });
            } 

            dataGrid.ItemsSource = query.Parameters;

            this.ParametersTab.Visibility = Visibility.Visible;
            this.ParametersTab.Content = dataGrid;
        }

        private static void SetTextEditor(ShellViewModel vm, TextEditor textEditor) {
            textEditor.Text = vm.SelectedQuery != null ? vm.SelectedQuery.Model.Query.CommandText : "";
        }
    }
}
