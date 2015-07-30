using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using EntityProfiler.Viewer.Models;
using EntityProfiler.Viewer.Modules.Connection;
using EntityProfiler.Viewer.Modules.ContextDetail;
using EntityProfiler.Viewer.Modules.ContextExplorer;
using EntityProfiler.Viewer.Modules.Output;
using EntityProfiler.Viewer.Modules.Output.ViewModels;
using EntityProfiler.Viewer.Properties;
using Gemini.Framework;
using Gemini.Framework.Menus;
using Gemini.Framework.ToolBars;
using Gemini.Modules.StatusBar.ViewModels;
using MahApps.Metro.Controls;
using MenuDefinitions = Gemini.Modules.MainMenu.MenuDefinitions;

namespace EntityProfiler.Viewer.Modules.Main
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase, IHandle<ConnectionHandlerStateMessage>, IHandle<StatusMessage>
    {
        private readonly IEventAggregator _eventAggregator;
        private StatusBarItemViewModel _firstStatusBarItem;

        [ImportingConstructor]
        public Module(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        public override void Initialize()
        {
            Shell.ShowFloatingWindowsInTaskbar = true;
            Shell.ToolBars.Visible = true;

            MainWindow.Title = "Entity Profiler";
            try
            {
                MainWindow.Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/images/icon.png", UriKind.Absolute));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Shell.StatusBar.AddItem("Ready", new GridLength(1, GridUnitType.Star));
            Shell.StatusBar.AddItem("", new GridLength(100));
            Shell.StatusBar.AddItem("", new GridLength(100));

            // Shell.ToolBars.Visible = false;

            RestoreWindowLocation();
        }

        public StatusBarItemViewModel FirstStatusBarItem
        {
            get 
            {
                return _firstStatusBarItem ?? (_firstStatusBarItem = Shell.StatusBar.Items.FirstOrDefault(p => p.Index == 0));
            }
        }

        public void Handle(ConnectionHandlerStateMessage stateMessage)
        {
            if (FirstStatusBarItem == null) return;

            var result = "Ready";
            var stateChangeEnd = stateMessage.StateChangeEnd;
            switch (stateMessage.ConnectionRequestState)
            {
                case ConnectionRequestState.Initial:
                    break;
                case ConnectionRequestState.Stop:
                    result = stateChangeEnd ? "Stoped" : "Stopping...";
                    break;
                case ConnectionRequestState.Start:
                    result = stateChangeEnd ? "Started" : "Starting...";
                    break;
                case ConnectionRequestState.Pause:
                    result = stateChangeEnd ? "Paused" : "Pausing...";
                    break;
            }
            FirstStatusBarItem.Message = result;
        }

        public void Handle(StatusMessage message)
        {
            if (FirstStatusBarItem == null) return;

            FirstStatusBarItem.Message = message.Message.Replace(Environment.NewLine, "");
        }

        public override IEnumerable<IDocument> DefaultDocuments
        {
            get
            {
                yield return IoC.Get<ContextDetailViewModel>();
            }
        }

        public override IEnumerable<Type> DefaultTools
        {
            get
            {
                yield return typeof(ContextExplorerViewModel);
                yield return typeof(IOutput);
            }
        }

        public override async void PostInitialize()
        {
            Shell.ShowTool<ContextExplorerViewModel>();
            Shell.OpenDocument(IoC.Get<ContextDetailViewModel>());

            LoadPreviousSession();

            await Task.Delay(500);

            var autoStart = Settings.Default.Connection_AutoStartWhenInitialized;
            if (autoStart)
            {
                var connectionHandler = IoC.Get<IConnectionHandler>();
                await connectionHandler.StartSession();
            }
        }

        private void OnAppClosing()
        {
            _eventAggregator.Unsubscribe(this);

            StoreWindowLocation();

            StoreApplicationSession();
        }

        public void LoadPreviousSession()
        {
            Task.Factory.StartNew(() =>
            {
                var appSettings = Settings.Default;
                if (appSettings.ApplicationSession != null)
                {
                    
                }
            });
        }

        protected static void StoreApplicationSession()
        {
            var settings = Settings.Default;
            
            settings.ApplicationSession = new ApplicationSession();

            settings.Save();
        }

        protected ApplicationSession RestoreApplicationSession()
        {
            var appSettings = Settings.Default;
            return appSettings.ApplicationSession;
        }

        #region window location

        protected void StoreWindowLocation()
        {
            var settings = Settings.Default;
            var mainWindow = Application.Current.MainWindow;

            if (settings != null && mainWindow != null)
            {
                if (mainWindow.WindowState == WindowState.Normal)
                {
                    settings.WindowTop = mainWindow.Top;
                    settings.WindowLeft = mainWindow.Left;
                }
                else
                {
                    settings.WindowTop = 0;
                    settings.WindowLeft = 0;
                }

                settings.WindowWidth = mainWindow.Width;
                settings.WindowHeight = mainWindow.Height;
                settings.WindowState = mainWindow.WindowState == WindowState.Minimized
                    ? WindowState.Normal
                    : mainWindow.WindowState;

                settings.Save();
            }
        }

        protected void RestoreWindowLocation()
        {
            var settings = Settings.Default;
            var mainWindow = Application.Current.MainWindow as MetroWindow;

            if (mainWindow != null)
            {
                mainWindow.Top = settings.WindowTop;
                mainWindow.Left = settings.WindowLeft;

                if (mainWindow.Top <= 0 || mainWindow.Left <= 0)
                    mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                else
                    mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;

                mainWindow.Width = settings.WindowWidth;
                mainWindow.Height = settings.WindowHeight;

                mainWindow.WindowState = settings.WindowState;

                mainWindow.Closing += (sender, args) => OnAppClosing();
            }
        }

        #endregion


        [Export]
        public static MenuExcludeDefinition ExcludeNewMenu = MenuExcludeDefinition.ExcludeWithText("New");

        [Export]
        public static MenuExcludeDefinition ExcludeToolbox = MenuExcludeDefinition.ExcludeWithCommandName("View.Toolbox");

        [Export]
        public static MenuExcludeDefinition ExcludeOpenFileMenu = MenuExcludeDefinition.ExcludeWithCommandName("File.OpenFile");

        [Export]
        public static ToolbarExcludeDefinition ExcludeOpenFileToolbar = ToolbarExcludeDefinition.ExcludeWithCommandName("File.OpenFile");

        [Export]
        public static MenuItemGroupDefinition ViewsMenuGroup = new MenuItemGroupDefinition(
            MenuDefinitions.ViewMenu, 0);

        [Export]
        public static MenuItemDefinition ViewHomeMenuItem = new CommandMenuItemDefinition<ViewContextExplorerDefinition>(
            ViewsMenuGroup, 0);

        [Export]
        public static MenuItemDefinition ViewContextDetailMenuItem = new CommandMenuItemDefinition<ViewContextDetailDefinition>(
            ViewsMenuGroup, 1);
        
        [Export]
        public static ToolBarDefinition StandardToolBar = new ToolBarDefinition(0, "Standard");

        /*[Export]
        public static ToolBarItemGroupDefinition StandardConnectionToolBarGroup = new ToolBarItemGroupDefinition(
         StandardToolBar, 0);

        [Export]
        public static ToolBarItemDefinition StartReaderToolBarItem = new CommandToolBarItemDefinition<StartReaderCommandDefinition>(
            StandardConnectionToolBarGroup, 0, ToolBarItemDisplay.IconAndText);

        [Export]
        public static ToolBarItemDefinition SepReaderToolBarItem = new CommandToolBarItemDefinition<PauseReaderCommandDefinition>(
            StandardConnectionToolBarGroup, 1);
        
        [Export]
        public static ToolBarItemDefinition StopReaderToolBarItem = new CommandToolBarItemDefinition<StopReaderCommandDefinition>(
            StandardConnectionToolBarGroup, 2);*/
    }
}