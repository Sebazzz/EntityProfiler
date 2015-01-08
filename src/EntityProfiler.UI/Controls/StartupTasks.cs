namespace EntityProfiler.UI.Controls {
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Caliburn.Micro;
    using MahApps.Metro.Controls;
    using Services;

    public class StartupTasks
    {
        private readonly IServiceLocator _serviceLocator;

        public StartupTasks(IServiceLocator serviceLocator)
        {
            this._serviceLocator = serviceLocator;
        }

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

        public void ApplyViewLocatorOverride()
        {
            var viewLocator = this._serviceLocator.GetInstance<IViewLocator>();
            Caliburn.Micro.ViewLocator.GetOrCreateViewType = viewLocator.GetOrCreateViewType;
        }


        public void Execute() {
            this.ApplyBindingScopeOverride();
            this.ApplyViewLocatorOverride();
        }
    }
}