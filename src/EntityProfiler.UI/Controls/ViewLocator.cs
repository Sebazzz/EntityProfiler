namespace EntityProfiler.UI.Controls {
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Caliburn.Micro;

    [Export(typeof (IViewLocator))]
    public class ViewLocator : IViewLocator {
        private readonly IThemeManager _themeManager;

        [ImportingConstructor]
        public ViewLocator(IThemeManager themeManager) {
            this._themeManager = themeManager;
        }

        public UIElement GetOrCreateViewType(Type viewType) {
            var cached = IoC.GetAllInstances(viewType).OfType<UIElement>().FirstOrDefault();
            if (cached != null) {
                Caliburn.Micro.ViewLocator.InitializeComponent(cached);
                return cached;
            }

            if (viewType.IsInterface || viewType.IsAbstract || !typeof (UIElement).IsAssignableFrom(viewType)) {
                return new TextBlock {Text = string.Format("Cannot create {0}.", viewType.FullName)};
            }

            var newInstance = (UIElement) Activator.CreateInstance(viewType);
            var frameworkElement = newInstance as FrameworkElement;
            if (frameworkElement != null) {
                frameworkElement.Resources.MergedDictionaries.Add(this._themeManager.GetThemeResources());
            }

            Caliburn.Micro.ViewLocator.InitializeComponent(newInstance);
            return newInstance;
        }
    }
}