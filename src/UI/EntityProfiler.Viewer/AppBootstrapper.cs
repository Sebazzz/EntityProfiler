using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using Caliburn.Micro;
using EntityProfiler.Common.Events;
using EntityProfiler.Interceptor.Reader;
using EntityProfiler.Interceptor.Reader.Protocol;
using EntityProfiler.TinyIoC;
using EntityProfiler.Viewer.Modules.Connection;
using EntityProfiler.Viewer.Services;
using Gemini.Framework.Services;

namespace EntityProfiler.Viewer
{
    public class AppBootstrapper : BootstrapperBase, IDisposable
    {
        private TinyIoCContainer _tinyIoCContainer;
        private bool _isDisposed;

        protected CompositionContainer Container { get; set; }

        public AppBootstrapper()
        {
            this.Initialize();
        }

        /// <summary>
        /// By default, we are configured to use MEF
        /// </summary>
        protected override void Configure()
        {
            // Add all assemblies to AssemblySource (using a temporary DirectoryCatalog).
            var directoryCatalog = new DirectoryCatalog(@"./");
            AssemblySource.Instance.AddRange(
                directoryCatalog.Parts
                    .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                    .Where(assembly => !AssemblySource.Instance.Contains(assembly)));

            // Prioritise the executable assembly. This allows the client project to override exports, including IShell.
            // The client project can override SelectAssemblies to choose which assemblies are prioritised.
            var priorityAssemblies = SelectAssemblies().ToList();
            var priorityCatalog = new AggregateCatalog(priorityAssemblies.Select(x => new AssemblyCatalog(x)));
            var priorityProvider = new CatalogExportProvider(priorityCatalog);

            // Now get all other assemblies (excluding the priority assemblies).
            var mainCatalog = new AggregateCatalog(
                AssemblySource.Instance
                    .Where(assembly => !priorityAssemblies.Contains(assembly))
                    .Select(x => new AssemblyCatalog(x)));
            var mainProvider = new CatalogExportProvider(mainCatalog);

            Container = new CompositionContainer(priorityProvider, mainProvider);
            priorityProvider.SourceProvider = Container;
            mainProvider.SourceProvider = Container;

            var batch = new CompositionBatch();

            BindServices(batch);

            batch.AddExportedValue(mainCatalog);

            Container.Compose(batch);
        }

        protected virtual void BindServices(CompositionBatch batch)
        {
            _tinyIoCContainer = new TinyIoCContainer();
            var eventAggregator = new EventAggregator();

            // Defaults
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(eventAggregator);
            
            // framework and infrastructure
            _tinyIoCContainer.Register<IEventAggregator>(eventAggregator);
            // _tinyIoCContainer.Register<IServiceLocator>(new TinyServiceLocator(_container));
            _tinyIoCContainer.RegisterMultiple<IMessageEventSubscriber>(new[] { typeof(EventMessageListener) }).AsSingleton();

            // register other implementations
            DependencyFactory.Configure(_tinyIoCContainer);

            // Export IoC registrations
            batch.AddExportedValue(_tinyIoCContainer.Resolve<IRestartableMessageListener>());

            batch.AddExportedValue(Container);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = Container.GetExports<object>(contract);

            if (exports.Any())
                return exports.First().Value;

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            Container.SatisfyImportsOnce(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            DisplayRootViewFor<IMainWindow>();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { Assembly.GetEntryAssembly() };
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (_tinyIoCContainer != null)
                {
                    _tinyIoCContainer.Dispose();
                }

                _tinyIoCContainer = null;

                _isDisposed = true;
            }
        }
    }
}