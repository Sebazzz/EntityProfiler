using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using EntityProfiler.Viewer.Properties;
using Xceed.Wpf.AvalonDock.Controls;

namespace EntityProfiler.Viewer.Modules.Output.Views
{
    /// <summary>
    ///     Interaction logic for OutputView.xaml
    /// </summary>
    public partial class OutputView : UserControl, IOutputView
    {
        private FlowDocument _doc;
        private Paragraph _paragraph;

        public OutputView()
        {
            InitializeComponent();

            _doc = new FlowDocument();
            _paragraph = new Paragraph
            {
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12
            };
            _doc.Blocks.Add(_paragraph);

            outputText.Document = _doc;

            toggleWordWrap.IsChecked = Settings.Default.OutputWordWrap;
            ToggleWordWrap();
            toggleWordWrap.Click += (sender, args) => ToggleWordWrap();
        }

        private void ToggleWordWrap()
        {
            var isChecked = toggleWordWrap.IsChecked ?? false;
            outputText.HorizontalScrollBarVisibility = isChecked ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
            //outputText.TextWrapping = isChecked ? TextWrapping.Wrap : TextWrapping.NoWrap;
            
            Settings.Default.OutputWordWrap = isChecked;
            Settings.Default.Save();
        }

        public void ScrollToEnd()
        {
            //outputText.ScrollToEnd();
            var scrollViewer = FindScrollViewer(outputText);
            if(scrollViewer != null)
                scrollViewer.ScrollToEnd();
        }

        public void Clear()
        {
            _paragraph.Inlines.Clear();
            //outputText.Clear();
        }

        public void AppendText(string text)
        {
            //outputText.AppendText(text);
            _paragraph.Inlines.Add(new Run(text));
            ScrollToEnd();
        }

        public void SetText(string text)
        {
            //outputText.Text = text;
            _paragraph.Inlines.Add(new Run(text));
            ScrollToEnd();
        }

        public static ScrollViewer FindScrollViewer(FlowDocumentScrollViewer flowDocumentScrollViewer)
        {
            if (VisualTreeHelper.GetChildrenCount(flowDocumentScrollViewer) == 0)
            {
                return null;
            }

            // Border is the first child of first child of a ScrolldocumentViewer
            DependencyObject firstChild = VisualTreeHelper.GetChild(flowDocumentScrollViewer, 0);
            if (firstChild == null)
            {
                return null;
            }

            Decorator border = VisualTreeHelper.GetChild(firstChild, 0) as Decorator;

            if (border == null)
            {
                return null;
            }

            return border.Child as ScrollViewer;
        }
    }
}