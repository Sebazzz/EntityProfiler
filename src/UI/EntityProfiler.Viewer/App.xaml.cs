using System;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using EntityProfiler.Viewer.Modules.Output;
using EntityProfiler.Viewer.PresentationCore;
using EntityProfiler.Viewer.Properties;
using Serilog;
using Serilog.Events;

namespace EntityProfiler.Viewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool _errorNotified;
        public const long FileSizeLimit = 256 * 1024 * 1024;
        
        public App()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.RollingFile(pathFormat: Environment.CurrentDirectory, restrictedToMinimumLevel: LogEventLevel.Warning, fileSizeLimitBytes: FileSizeLimit)
                .WriteTo.OutputModule(() => IoC.Get<IOutput>(), () => IoC.Get<IOutputLogFilter>())
                .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;

            InitilizeSettings();
        }

        private void InitilizeSettings()
        {
            var save = false;
            if (Settings.Default.TextEditor_CodeEditorOptions == null)
            {
                save = true;
                Settings.Default.TextEditor_CodeEditorOptions = new ExtendedTextEditorOptions();
            }
            if (Settings.Default.TextEditor_SqlEditorOptions == null)
            {
                save = true;
                Settings.Default.TextEditor_SqlEditorOptions = new ExtendedTextEditorOptions();
            }
            
            if (save)
            {
                Settings.Default.Save();
            }
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            
            if (exception == null && e.ExceptionObject != null)
            {
                exception = new ApplicationException(string.Format(">A non-CLI exception occurred: {0}", e.ExceptionObject));
            }

            Log.Error(exception, "An unhandled exception occurred", true);

            ShowErrorMessage(exception != null ? exception.Message : "");
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, "An unhandled exception occurred", true);

            ShowErrorMessage(e.Exception.Message);
        }

        private void ShowErrorMessage(string message = "")
        {
            if(_errorNotified)
                return;

            _errorNotified = true;

            if (string.IsNullOrEmpty(message))
                message = "There was an error in the application, see more details in the log file.";
            MessageBox.Show(message, "Application error");
        }
    }
}
