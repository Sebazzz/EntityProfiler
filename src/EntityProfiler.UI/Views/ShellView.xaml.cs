namespace EntityProfiler.UI.Views {
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
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
                }
            };

            SetTextEditor(vm, textEditor);
        }

        private static void SetTextEditor(ShellViewModel vm, TextEditor textEditor) {
            textEditor.Text = vm.SelectedQuery != null ? vm.SelectedQuery.Query.CommandText : "";
        }
    }
}
