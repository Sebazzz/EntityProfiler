using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using EntityProfiler.Viewer.Modules.Connection;
using EntityProfiler.Viewer.Properties;
using Gemini.Framework.Services;
using Gemini.Modules.Shell.Views;

namespace EntityProfiler.Viewer.Modules.Main
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Gemini.Modules.Shell.ViewModels.ShellViewModel
    {
        static ShellViewModel()
        {
            ViewLocator.AddNamespaceMapping(typeof(ShellViewModel).Namespace, typeof(ShellView).Namespace);
        }

        public override void CanClose(Action<bool> callback)
        {
            Coroutine.BeginExecute(CanClose().GetEnumerator(), null, (s, e) => callback(!e.WasCancelled));
        }

        private IEnumerable<IResult> CanClose()
        {
            yield return new MessageBoxResult();
        }

        private class MessageBoxResult : IResult
        {
            public event EventHandler<ResultCompletionEventArgs> Completed;

            public async void Execute(CoroutineExecutionContext context)
            {
                var result = System.Windows.MessageBoxResult.Yes;
                var connectionHandler = IoC.Get<IConnectionHandler>();

                if (Settings.Default.Connection_ConfirmOnCloseConnected && connectionHandler.ConnectionRequestState == ConnectionRequestState.Start)
                {
                    result = MessageBox.Show("You are connected, you want to exit?", "Confirm", MessageBoxButton.YesNo);
                    if (result == System.Windows.MessageBoxResult.Yes)
                        await connectionHandler.StopSession();
                }

                if (Completed != null)
                    Completed(this, new ResultCompletionEventArgs { WasCancelled = (result != System.Windows.MessageBoxResult.Yes) });
            }
        }
    }
}