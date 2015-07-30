using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using EntityProfiler.Viewer.Properties;

namespace EntityProfiler.Viewer.Modules.Output.Views
{
    /// <summary>
    ///     Interaction logic for OutputView.xaml
    /// </summary>
    public partial class OutputView : UserControl, IOutputView
    {
        public OutputView()
        {
            InitializeComponent();
            toggleWordWrap.IsChecked = Settings.Default.OutputWordWrap;
            ToggleWordWrap();
            toggleWordWrap.Click += (sender, args) => ToggleWordWrap();
        }

        private void ToggleWordWrap()
        {
            var isChecked = toggleWordWrap.IsChecked ?? false;
            outputText.HorizontalScrollBarVisibility = isChecked ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
            outputText.TextWrapping = isChecked ? TextWrapping.Wrap : TextWrapping.NoWrap;

            Settings.Default.OutputWordWrap = isChecked;
            Settings.Default.Save();
        }

        public void ScrollToEnd()
        {
            outputText.ScrollToEnd();
        }

        public void Clear()
        {
            //outputText.Document.Blocks.Clear();
            outputText.Clear();
        }

        public void AppendText(string text)
        {
            outputText.AppendText(text);
            ScrollToEnd();
        }

        public void SetText(string text)
        {
            outputText.Text = text;
            //outputText.Document.Blocks.Add(new Paragraph(new Run(text)));
            ScrollToEnd();
        }
    }
}