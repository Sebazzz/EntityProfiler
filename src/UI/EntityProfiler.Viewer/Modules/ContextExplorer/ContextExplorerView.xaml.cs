using System;
using System.Windows;
using System.Windows.Controls;
using EntityProfiler.Viewer.Properties;

namespace EntityProfiler.Viewer.Modules.ContextExplorer
{
    /// <summary>
    /// Interaction logic for ObjectsExplorerView.xaml
    /// </summary>
    public partial class ContextExplorerView : UserControl
    {
        public ContextExplorerView()
        {
            InitializeComponent();

            AutoSelectedDataContextToggleButton.IsChecked = Settings.Default.Profiler_Session_AutoSelectedDataContext;
            AutoSelectedDataContextToggleButton.Checked += AutoSelectedDataContextToggleButtonOnCheckedChange;
            AutoSelectedDataContextToggleButton.Unchecked += AutoSelectedDataContextToggleButtonOnCheckedChange;
        }

        private void AutoSelectedDataContextToggleButtonOnCheckedChange(object sender, RoutedEventArgs routedEventArgs)
        {
            Settings.Default.Profiler_Session_AutoSelectedDataContext = AutoSelectedDataContextToggleButton.IsChecked ?? false;
            Settings.Default.Save();
        }
    }
}
