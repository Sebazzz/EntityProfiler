namespace EntityProfiler.UI {
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Caliburn.Micro;
    using Common.Events;
    using Controls;
    using Interceptor.Reader;
    using Services;
    using TinyIoC;
    using ViewModels;

    public sealed class AppBootstrapper : BootstrapperBase, IDisposable {
        private TinyIoCContainer _container;
        private bool _isDisposed;

        public AppBootstrapper() {
            this.Initialize();
        }

        protected override void BuildUp(object instance) {
            this._container.BuildUp(instance);
        }

        /// <summary>
        ///     By default, we are configured to use MEF
        /// </summary>
        protected override void Configure() {
            this._container = new TinyIoCContainer();

            // framework and infrastructure
            this._container.Register<IWindowManager>(new WindowManager());
            this._container.Register<IEventAggregator>(new EventAggregator());
            this._container.Register<IViewLocator, Controls.ViewLocator>();
            this._container.Register<IThemeManager, Controls.ThemeManager>();

            this._container.Register<IServiceLocator>(new TinyServiceLocator(this._container));
            this._container.Register<IMessageEventSubscriber, EventMessageListener>().AsSingleton();

            // register view models
            this._container.AutoRegister(t => String.Equals(t.Namespace, typeof(ShellViewModel).Namespace, StringComparison.Ordinal));
            this._container.Register<ShellViewModel>().AsSingleton();
            this._container.Register<IShell, ShellViewModel>();

            // register other implementations
            DependencyFactory.Configure(this._container);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType) {
            return this._container.ResolveAll(serviceType);
        }

        protected override object GetInstance(Type serviceType, string key) {
            return this._container.Resolve(serviceType);
        }

        protected override void OnStartup(object sender, StartupEventArgs e) {
            new StartupTasks(this._container.Resolve<IServiceLocator>()).Execute();

            this.DisplayRootViewFor<IShell>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            if (!this._isDisposed) {
                if (this._container != null) {
                    this._container.Dispose();
                }

                this._container = null;

                this._isDisposed = true;
            }
        }
    }
}