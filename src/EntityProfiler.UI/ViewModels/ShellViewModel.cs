namespace EntityProfiler.UI.ViewModels {
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using Caliburn.Micro;
    using Common.Events;
    using Common.Protocol;
    using Interceptor.Reader.Core;
    using Interceptor.Reader.Protocol;
    using PropertyChanged;
    using Services;

    [ImplementPropertyChanged]
    public class ShellViewModel : Screen, IShell, IHandle<MessageEvent> {
        private readonly IRestartableMessageListener _messageListener;
        private readonly IMessageFilter _messageFilter;
        private readonly IObservableCollection<DataContextViewModel> _dataContexts;

        /// <summary>
        ///     Creates an instance of the screen.
        /// </summary>
        public ShellViewModel(IRestartableMessageListener messageListener, IEventAggregator eventAggregator) {
            this._messageListener = messageListener;
            this._dataContexts = new BindableCollection<DataContextViewModel>();
            this._messageFilter = new DuplicateQueryDetectionMessageFilter();
            this.PropertyChanged += this.OnPropertyChanged;

            eventAggregator.Subscribe(this);

        }

        /// <summary>
        ///     Creates an instance of the screen.
        /// </summary>
        [Obsolete("This is a design-time only constructor")]
        public ShellViewModel() {
            this.StatusBar = "Connected";

            this._dataContexts = new BindableCollection<DataContextViewModel>();
            this._dataContexts.AddRange(SeedData.DataContexts());

            this.SelectedDataContext = this._dataContexts[0];
        }

        public string StatusBar { get; set; }

        public IObservableCollection<QueryMessageViewModel> Queries {
            [DebuggerStepThrough] get {
                return this.SelectedDataContext != null ? this.SelectedDataContext.Queries : new BindableCollection<QueryMessageViewModel>();
            }
        }

        public IObservableCollection<DataContextViewModel> DataContexts {
            [DebuggerStepThrough] get { return this._dataContexts; }
        }

        public DataContextViewModel SelectedDataContext { get; set; }

        public QueryMessageViewModel SelectedQuery { get; set; }

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

            this._resetStatusBarAction = OneTimeAction.Execute(1750, () => this.StatusBar = "Ready").CancelExisting(this._resetStatusBarAction);

            // find data context
            DataContextViewModel dataContext = this.GetOrCreateDataContext(queryMessage.Context);

            IObservableCollection<QueryMessageViewModel> queries = dataContext.Queries;
            if (queries.Count == 0) {
                queries.Add(queryMessage);
                return;
            }

            // try to merge with last
            QueryMessage lastQueryMessage = queries[queries.Count - 1].Model;
            QueryMessage merged = this._messageFilter.FilterTwo(lastQueryMessage, queryMessage) as QueryMessage;

            if (merged == null) {
                queries.Add(queryMessage);
            } else {
                queries[queries.Count - 1].Model = merged;
            }
        }

        private DataContextViewModel GetOrCreateDataContext(ExecutionContext context) {
            DataContextViewModel result = this._dataContexts.FirstOrDefault(x => x.Identifier == context.Identifier);
            if (result != null) {
                return result;
            }

            result = new DataContextViewModel() {
                Description = context.Description,
                Identifier = context.Identifier
            };

            // auto-select if no data context has been selected
            this._dataContexts.Insert(0, result);
            if (this.SelectedDataContext == null) {
                this.SelectedDataContext = result;
            }

            return result;
        }

        private void TryConnect() {
            this.StatusBar = "Connecting...";
            this._messageListener.Restart();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case "SelectedDataContext":
                    // select the last query
                    this.SelectedQuery = this.SelectedDataContext != null ? this.SelectedDataContext.Queries.Last() : null;
                    break;
            }
        }
    }
}