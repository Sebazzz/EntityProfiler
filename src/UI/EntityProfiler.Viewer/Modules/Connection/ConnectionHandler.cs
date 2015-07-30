using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using EntityProfiler.Common.Annotations;
using EntityProfiler.Common.Events;
using EntityProfiler.Common.Protocol;
using EntityProfiler.Interceptor.Reader.Protocol;
using EntityProfiler.Viewer.PresentationCore;
using EntityProfiler.Viewer.Properties;
using EntityProfiler.Viewer.Services;
using Serilog;

namespace EntityProfiler.Viewer.Modules.Connection
{
    public interface IConnectionHandler
    {
        ConnectionRequestState ConnectionRequestState { get; }
        bool IsBusy { get; set; }
        Task StartSession();
        Task PauseSession(string sessionId = null);
        Task StopSession(string sessionId = null);
    }

    public enum ConnectionRequestState
    {
        Initial,
        Stop,
        Start,
        Pause
    }

    [Export(typeof(IConnectionHandler))]
    public class ConnectionHandler : IConnectionHandler, INotifyPropertyChanged, IDisposable, IHandle<MessageEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRestartableMessageListener _messageListener;
        private bool _isBusy;
        private ConnectionRequestState _connectionRequestState;

        [ImportingConstructor]
        public ConnectionHandler(
            IRestartableMessageListener messageListener,
            IEventAggregator eventAggregator)
        {
            _messageListener = messageListener;
            _eventAggregator = eventAggregator;

            _eventAggregator.Subscribe(this);

            MaxRetry = Settings.Default.Connection_MaxRetryConnect;
            RetryInterval = Settings.Default.Connection_RetryConnectInterval;
        }

        public int MaxRetry
        {
            get { return _maxRetry; }
            set
            {
                var result = value;
                if (result < 0)
                    result = 0;
                _maxRetry = result;
            }
        }

        public int RetryInterval
        {
            get { return _retryInterval; }
            set
            {
                var result = value;
                if (result < 1000)
                    result = 1000;
                _retryInterval = result;
            }
        }

        public ConnectionRequestState ConnectionRequestState
        {
            get { return _connectionRequestState; }
            private set
            {
                _connectionRequestState = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (value == _isBusy) return;
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public async Task StartSession()
        {
            Log.Information("Start session request.");
            IsBusy = true;
            PublishSessionState(ConnectionRequestState.Start).ConfigureAwait(false);

            await MessageListenerRestart(true);

            ConnectionRequestState = ConnectionRequestState.Start;

            PublishSessionState(ConnectionRequestState.Start, true).ConfigureAwait(false);
            IsBusy = false;
            CommandManager.InvalidateRequerySuggested();
        }

        public Task PauseSession(string sessionId = null)
        {
            Log.Information("Pause session request.");
            IsBusy = true;
            PublishSessionState(ConnectionRequestState.Pause).ConfigureAwait(false);

            ConnectionRequestState = ConnectionRequestState.Pause;

            PublishSessionState(ConnectionRequestState.Pause, true).ConfigureAwait(false);
            IsBusy = false;
            CommandManager.InvalidateRequerySuggested();

            return Task.FromResult(0);
        }

        public async Task StopSession(string sessionId = null)
        {
            Log.Information("Stop session request.");
            IsBusy = true;
            PublishSessionState(ConnectionRequestState.Stop).ConfigureAwait(false);

            await MessageListenerStop();

            ConnectionRequestState = ConnectionRequestState.Stop;

            PublishSessionState(ConnectionRequestState.Stop, true).ConfigureAwait(false);
            IsBusy = false;
            CommandManager.InvalidateRequerySuggested();
        }

        public void Dispose()
        {
            _eventAggregator.Unsubscribe(this);
        }

        public void Handle(MessageEvent message)
        {
            if (message.Exception != null)
            {
                HandleError(message.Exception);
                return;
            }

            var queryMessage = message.Message as QueryMessage;
            if (queryMessage != null && ConnectionRequestState != ConnectionRequestState.Pause)
            {
                HandleQueryMessage(queryMessage);
            }

            var connectedMessage = message.Message as ConnectedMessage;
            if (connectedMessage != null)
            {
                HandleConnectedMessage(connectedMessage);
            }
        }

        private async Task MessageListenerRestart(bool force = false)
        {
            if (_requestStop && !force)
                return;

            if (MaxRetry > 0 && _retryCount > MaxRetry)
            {
                Log.Information("Retry limit '{maxRetry}' exceeded.", MaxRetry);
                await MessageListenerStop();
                Notify("Retry limit exceeded.").ConfigureAwait(false);
                return;
            }

            _retryCount++;
            _requestStop = false;
            Log.Information("MessageListenerRestart request.");
            Notify("Connecting...").ConfigureAwait(false);
            _messageListener.Restart();
        }

        private async Task MessageListenerStop()
        {
            Log.Information("MessageListenerStop request.");
            
            await Notify("Stoping...");

            _retryCount = 0;
            _requestStop = true;
            _messageListener.Stop();
            _messageListener.Dispose();
        }

        private int _retryCount;
        private OneTimeAction _connectAttempt;
        private void HandleError(Exception exception)
        {
            Log.Error(exception, "Connecting error, retrying...");
            Notify("Connecting error... [" + exception.GetType().FullName + "] retrying...");
            _connectAttempt = OneTimeAction.Execute(RetryInterval, () => MessageListenerRestart()).CancelExisting(_connectAttempt);
        }

        private void HandleConnectedMessage(ConnectedMessage connectedMessage)
        {
            Log.Information("Connected to v {appVersion}", connectedMessage.Version);
            _retryCount = 0;
            Notify("Connected to v" + connectedMessage.Version);
        }

        private OneTimeAction _resetStatusBarAction;
        private bool _requestStop;
        private int _retryInterval;
        private int _maxRetry;

        private void HandleQueryMessage(QueryMessage queryMessage)
        {
            var commandText = queryMessage.Query.CommandText.Replace(Environment.NewLine, " ")
                .ToSingleWordsSpace().Ellipsis(64, EllipsisFormat.Word | EllipsisFormat.End);

            Log.Information("QueryMessage received {queryContextId}: {queryContextDescription}, Total time(ms): {queryTotalTime}",
                queryMessage.Context.Identifier,
                queryMessage.Context.Description, 
                queryMessage.Performance.TotalTime);

            Notify(commandText);

            _resetStatusBarAction = OneTimeAction.Execute(1750, () => Notify("Ready")).CancelExisting(_resetStatusBarAction);

            SessionData.Current.HandleQueryMessage(queryMessage);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private Task PublishSessionState(ConnectionRequestState connectionRequestState, bool stateChangeEnd = false)
        {
            return _eventAggregator.PublishOnUIThreadAsync(new ConnectionHandlerStateMessage(connectionRequestState, stateChangeEnd));
        }

        private Task Notify(string message)
        {
            return _eventAggregator.PublishOnUIThreadAsync(new StatusMessage(message));
        }
    }
}