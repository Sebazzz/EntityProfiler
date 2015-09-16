using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Windows;
using Anotar.Serilog;
using Caliburn.Micro;
using EntityProfiler.Common.Events;
using EntityProfiler.Interceptor.Reader;
using EntityProfiler.Interceptor.Reader.Protocol;
using EntityProfiler.TinyIoC;
using EntityProfiler.Viewer.Services;
using Gemini.Framework.Services;
using Serilog;

namespace EntityProfiler.Viewer
{
    public class AppBootstrapper : BootstrapperBase, IDisposable
    {
        private TinyIoCContainer _tinyIoCContainer;
        private bool _isDisposed;

        private static readonly string[] _composableEmbeddedAssemblies =
        {
            "gemini"
        };

        protected CompositionContainer Container { get; set; }

        public AppBootstrapper()
        {
            Initialize();
        }

        /// <summary>
        /// By default, we are configured to use MEF
        /// </summary>
        [LogToErrorOnException]
        protected override void Configure()
        {
            try
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
            catch (Exception e)
            {
                Log.Error(e, "Error on Bootstrapper Configure");
            }
        }

        protected virtual void BindServices(CompositionBatch batch)
        {
            try
            {
                _tinyIoCContainer = new TinyIoCContainer();
                var eventAggregator = new EventAggregator();

                // Defaults
                batch.AddExportedValue<IWindowManager>(new WindowManager());
                batch.AddExportedValue<IEventAggregator>(eventAggregator);

                // framework and infrastructure
                _tinyIoCContainer.Register<IEventAggregator>(eventAggregator);
                // _tinyIoCContainer.Register<IServiceLocator>(new TinyServiceLocator(_container));
                _tinyIoCContainer.RegisterMultiple<IMessageEventSubscriber>(new[] {typeof (EventMessageListener)})
                    .AsSingleton();

                // register other implementations
                DependencyFactory.Configure(_tinyIoCContainer);

                // Export IoC registrations
                batch.AddExportedValue(_tinyIoCContainer.Resolve<IRestartableMessageListener>());

                batch.AddExportedValue(Container);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error on Bootstrapper BindServices: {CompositionBatch}", batch);
            }
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

        protected virtual Dictionary<string, Assembly> RegisterAssemblyAndEmbeddedDependencies()
        {
            var currentDomain = AppDomain.CurrentDomain;
            var assemblies = currentDomain.GetAssemblies();

            var references = new Dictionary<string, Assembly>();
            var executingAssembly = Assembly.GetExecutingAssembly();

            var embeddedAssemblies = _composableEmbeddedAssemblies.Select(p => string.Format("costura.{0}.dll.zip", p));

            var manifestResourceNames = executingAssembly.GetManifestResourceNames()
                .Where(p => embeddedAssemblies.Contains(p.ToLower()));

            foreach (var resourceName in manifestResourceNames)
            {
                var solved = false;
                foreach (var assembly in assemblies)
                {
                    var refName = string.Format("costura.{0}.dll.zip", GetDllName(assembly, true));
                    if (resourceName.Equals(refName, StringComparison.OrdinalIgnoreCase))
                    {
                        references[assembly.FullName] = assembly;
                        solved = true;
                        break;
                    }
                }

                if (solved)
                    continue;

                using (var resourceStream = executingAssembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null) continue;

                    if (resourceName.EndsWith(".zip"))
                    {
                        using (var compressStream = new DeflateStream(resourceStream, CompressionMode.Decompress))
                        {
                            var memStream = new MemoryStream();
                            CopyTo(compressStream, memStream);
                            memStream.Position = 0;

                            var rawAssembly = new byte[memStream.Length];
                            memStream.Read(rawAssembly, 0, rawAssembly.Length);
                            var reference = Assembly.Load(rawAssembly);
                            references[reference.FullName] = reference;
                        }
                    }
                    else
                    {
                        var rawAssembly = new byte[resourceStream.Length];
                        resourceStream.Read(rawAssembly, 0, rawAssembly.Length);
                        var reference = Assembly.Load(rawAssembly);
                        references[reference.FullName] = reference;
                    }
                }
            }
            return references;
        }

        private static string GetDllName(Assembly assembly, bool withoutExtension = false)
        {
            var assemblyPath = assembly.CodeBase;
            return withoutExtension ? Path.GetFileNameWithoutExtension(assemblyPath) : Path.GetFileName(assemblyPath);
        }

        private static void CopyTo(Stream source, Stream destination)
        {
            var array = new byte[81920];
            int count;
            while ((count = source.Read(array, 0, array.Length)) != 0)
            {
                destination.Write(array, 0, count);
            }
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