namespace EntityProfiler.UI.Controls {
    using System;
    using System.ComponentModel.Composition;
    using System.Windows;

    [Export(typeof(IThemeManager))]
    public class ThemeManager : IThemeManager
    {
        private readonly ResourceDictionary themeResources;

        public ThemeManager()
        {
            this.themeResources = new ResourceDictionary
                                  {
                                      Source =
                                          new Uri("pack://application:,,,/Resources/DefaultTheme.xaml")
                                  };
        }

        public ResourceDictionary GetThemeResources()
        {
            return this.themeResources;
        }
    }
}