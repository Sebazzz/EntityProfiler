using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using EntityProfiler.Viewer.Modules.Connection.ViewModels;

namespace EntityProfiler.Viewer.Modules.Connection
{
    [Export("SessionDataCommandProvider", typeof (SessionDataCommandProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SessionDataCommandProvider
    {
        public void ShowAllDataContexts()
        {
            SessionData.Current.ShowAllDataContexts();
        }

        public void ShowDataContext(DataContextViewModel dataContextViewModel)
        {
            SessionData.Current.ShowDataContext(dataContextViewModel);
        }

        public void HideDataContext(DataContextViewModel dataContextViewModel)
        {
            SessionData.Current.HideDataContext(dataContextViewModel);
        }

        public void OnPreviewKeyDown(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;
            var dataContextViewModel = SessionData.Current.SelectedDataContext;
            if (keyArgs != null && dataContextViewModel != null)
            {
                /*if (keyArgs.Key == Key.Space || keyArgs.Key == Key.Enter)
                {
                    return;
                }*/
                if (keyArgs.Key == Key.Delete)
                {
                    SessionData.Current.HideDataContext(dataContextViewModel);
                    return;
                }
            }
        }
    }
}