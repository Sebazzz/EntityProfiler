namespace EntityProfiler.UI.Controls {
    using System;
    using System.Windows;

    public interface IViewLocator {
        UIElement GetOrCreateViewType(Type viewType);
    }
}