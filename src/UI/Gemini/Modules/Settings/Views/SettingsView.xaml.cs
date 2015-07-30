using System.Windows;
using System.Windows.Controls;

namespace Gemini.Modules.Settings.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
            ContentItemsControl.ScrollIntoView();
        }
    }

    public static class Extensions
    {
        public static void ScrollIntoView(
            this ItemsControl control,
            object item)
        {
            FrameworkElement framework =
                control.ItemContainerGenerator.ContainerFromItem(item)
                as FrameworkElement;
            if (framework == null) { return; }
            framework.BringIntoView();
        }
        public static void ScrollIntoView(this ItemsControl control)
        {
            int count = control.Items.Count;
            if (count == 0) { return; }
            object item = control.Items[count - 1];
            control.ScrollIntoView(item);
        }
    }
}
