namespace EntityProfiler.UI.ViewModels {
    using System;
    using Caliburn.Micro;
    using Common.Events;
    using Common.Protocol;
    using Interceptor.Reader.Protocol;
    using PropertyChanged;
    using Services;

    [ImplementPropertyChanged]
    public class ShellViewModel : Screen, IShell, IHandle<MessageEvent> {
        private readonly IMessageListener _messageListener;

        /// <summary>
        ///     Creates an instance of the screen.
        /// </summary>
        public ShellViewModel(IMessageListener messageListener, IEventAggregator eventAggregator) {
            this._messageListener = messageListener;

            eventAggregator.Subscribe(this);
        }

        /// <summary>
        ///     Creates an instance of the screen.
        /// </summary>
        [Obsolete("This is a design-time only constructor")]
        public ShellViewModel() {
            this.StatusBar = "Connected";
        }

        public string StatusBar { get; set; }

        /// <summary>
        ///     Handles the message.
        /// </summary>
        /// <param name="event">The message.</param>
        public void Handle(MessageEvent @event) {
            if (@event.Exception != null) {
                this.HandleError(@event.Exception);
                return;
            }

            var queryMessage = @event.Message as QueryMessage;
            if (queryMessage != null) {
                this.HandleQueryMessage(queryMessage);
            }

            var connectedMessage = @event.Message as ConnectedMessage;
            if (connectedMessage != null) {
                this.HandleConnectedMessage(connectedMessage);
            }
        }

        /// <summary>
        ///     Called when initializing.
        /// </summary>
        protected override void OnInitialize() {
            base.OnInitialize();

            this.DisplayName = "Entity Profiler";

            this.TryConnect();
        }

        /// <summary>
        ///     Notifies subscribers of the property change.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public override sealed void NotifyOfPropertyChange(string propertyName = null) {
            base.NotifyOfPropertyChange(propertyName);
        }

        private void HandleConnectedMessage(ConnectedMessage connectedMessage) {
            this.StatusBar = "Connected to v" + connectedMessage.Version;
        }

        private OneTimeAction _connectAttempt;
        private void HandleError(Exception exception) {
            this.StatusBar = "Connecting error... [" + exception.GetType().FullName + "] retrying...";

            this._connectAttempt = OneTimeAction.Execute(1000, this.TryConnect).CancelExisting(this._connectAttempt);
        }

        private OneTimeAction _resetStatusBarAction;
        private void HandleQueryMessage(QueryMessage queryMessage) {
            this.StatusBar = queryMessage.Query.CommandText;

            this._resetStatusBarAction = OneTimeAction.Execute(750, () => this.StatusBar = "Ready").CancelExisting(this._resetStatusBarAction);
        }

        private void TryConnect() {
            this.StatusBar = "Connecting...";
            this._messageListener.Start();
        }
    }
}