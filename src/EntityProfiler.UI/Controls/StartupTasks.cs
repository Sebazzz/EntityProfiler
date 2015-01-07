namespace EntityProfiler.UI.Services {
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Caliburn.Micro;
    using Controls;
    using MahApps.Metro.Controls;
    using ViewLocator = Caliburn.Micro.ViewLocator;

    public delegate void StartupTask();
    public class StartupTasks
    {
        private readonly IServiceLocator _serviceLocator;

        [ImportingConstructor]
        public StartupTasks(IServiceLocator serviceLocator)
        {
            this._serviceLocator = serviceLocator;
        }

        [Export(typeof(StartupTask))]
        public void ApplyBindingScopeOverride()
        {
            var getNamedElements = BindingScope.GetNamedElements;
            BindingScope.GetNamedElements = o =>
                                            {
                                                var metroWindow = o as MetroWindow;
                                                if (metroWindow == null)
                                                {
                                                    return getNamedElements(o);
                                                }

                                                var list = new List<FrameworkElement>(getNamedElements(o));
                                                var type = o.GetType();
                                                var fields =
                                                    o.GetType()
                                                        .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                                                        .Where(f => f.DeclaringType == type);
                                                var flyouts =
                                                    fields.Where(f => f.FieldType == typeof(FlyoutsControl))
                                                        .Select(f => f.GetValue(o))
                                                        .Cast<FlyoutsControl>();
                                                list.AddRange(flyouts);
                                                return list;
                                            };
        }

        [Export(typeof(StartupTask))]
        public void ApplyViewLocatorOverride()
        {
            var viewLocator = this._serviceLocator.GetInstance<IViewLocator>();
            ViewLocator.GetOrCreateViewType = viewLocator.GetOrCreateViewType;
        }
    }
}