using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using EntityProfiler.Viewer.Modules.QueryTools.Views;
using EntityProfiler.Viewer.Services;
using Gemini.Framework;

namespace EntityProfiler.Viewer.Modules.QueryTools.ViewModels
{
    [Export(typeof(DatabaseQueryRunnerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DatabaseQueryRunnerViewModel : Document
    {
        private static readonly Lazy<HashSet<Guid>> _instances = 
            new Lazy<HashSet<Guid>>(() => new HashSet<Guid>());
        private const int DefaultQueryTimeout = 120;
        private readonly Guid _instanceId;
        private CancellationTokenSource _cancellationTokenSource;
        private IDatabaseQueryRunnerView _view;
        private string _originalCommand;
        private IObservableCollection<ResultsSetModel> _resultsSets;
        private string _documentName;
        private bool _isDirty;
        private bool _isBusy;
        private int _selectedResultsSetsIndex;
        private bool _isExecutingQuery;
        private bool _isCancelingQuery;
        private string _status;

        [ImportingConstructor]
        public DatabaseQueryRunnerViewModel()
        {
            _instanceId = Guid.NewGuid();
            _instances.Value.Add(_instanceId);
            UpdateDisplayName();
        }

        internal Guid InstanceId
        {
            get { return _instanceId; }
        }

        public void SetNewQuery(string connetionString, string commandText)
        {
            ConnectionString = connetionString;
            _originalCommand = commandText;
            CommandText = commandText;

            NotifyOfPropertyChange(()=> CanExecuteQuery);
            NotifyOfPropertyChange(()=> CanCopyConnectionString);
        }

        public string CommandText { get; private set; }

        public string ConnectionString { get; private set; }

        public int SelectedResultsSetsIndex
        {
            get { return _selectedResultsSetsIndex; }
            set
            {
                if (value == _selectedResultsSetsIndex) return;
                _selectedResultsSetsIndex = value;
                NotifyOfPropertyChange();
            }
        }

        public IObservableCollection<ResultsSetModel> ResultsSets
        {
            get { return _resultsSets ?? (_resultsSets = new BindableCollection<ResultsSetModel>()); }
            private set
            {
                if (Equals(value, _resultsSets)) return;
                _resultsSets = value;
                NotifyOfPropertyChange();
            }
        }

        public string SelectedText
        {
            get
            {
                if (_view != null)
                    return _view.TextEditor.SelectedText;
                return null;
            }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            private set
            {
                if (value == _isDirty) return;
                _isDirty = value;
                NotifyOfPropertyChange();
                UpdateDisplayName();
            }
        }

        public string Status
        {
            get { return _status; }
            private set
            {
                if (value == _status) return;
                _status = value;
                NotifyOfPropertyChange();
            }
        }

        public async Task ExecuteQuery()
        {
            if (!CanExecuteQuery)
                return;

            var dtNow = DateTime.Now;
            _cancellationTokenSource = new CancellationTokenSource();

            IsExecutingQuery = true;
            Status = "Executing query.";
            
            var databaseQueryService = new DbClient();
            var executeParameters = new ExecuteParameters
            {
                ConnectionString = ConnectionString,
                SqlStatement = CommandTextSource
            };
            var cancellationToken = _cancellationTokenSource.Token;
            var updateStatusAction = OneTimeAction.Execute(1000, () =>
            {
                Status = string.Format("Executing query in {0}. ", (DateTime.Now - dtNow).ToString(@"dd\.hh\:mm\:ss"));
            });

            var results = await databaseQueryService.ExecuteSqlAsync(executeParameters, DefaultQueryTimeout, cancellationToken);
            
            // Reset
            updateStatusAction.Cancel();
            Status = string.Empty;
            ResultsSets.Clear();
            // Preccess results
            var i = 1;
            foreach (var executeResults in results)
            {
                var hasData = executeResults.ResultsData != null && executeResults.ResultsData.Rows.Count > 0;
                var resultsSetModel = new ResultsSetModel
                {
                    Title = "Result Set " + i,
                    StatusMessage = executeResults.StatusMessage,
                    ExceptionDetails = executeResults.ExceptionDetails
                };
                if (hasData)
                    resultsSetModel.ResultsData = executeResults.ResultsData;
                else
                    resultsSetModel.ResultsData = executeResults.StatusMessage;
                ResultsSets.Add(resultsSetModel);
                i++;
            }
            IsExecutingQuery = false;
            IsCancelingQuery = false;
            SelectedResultsSetsIndex = 0;
        }

        public void CancelExecutingQuery()
        {
            if (_cancellationTokenSource == null)
                return;

            IsCancelingQuery = true;
            _cancellationTokenSource.Cancel();
        }

        public bool CanExecuteQuery
        {
            get { return !IsExecutingQuery && !IsCancelingQuery && !string.IsNullOrEmpty(CommandTextSource); }
        }

        public bool CanCancelExecutingQuery
        {
            get { return IsExecutingQuery && !IsCancelingQuery; }
        }

        private string CommandTextSource 
        {
            get
            {
                return !string.IsNullOrWhiteSpace(SelectedText) ? SelectedText : CommandText;
            }
        }
        
        public bool IsExecutingQuery
        {
            get { return _isExecutingQuery; }
            set
            {
                if (value == _isExecutingQuery) return;
                _isExecutingQuery = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanExecuteQuery);
                NotifyOfPropertyChange(() => CanCancelExecutingQuery);
            }
        }

        public bool IsCancelingQuery
        {
            get { return _isCancelingQuery; }
            set
            {
                if (value == _isCancelingQuery) return;
                _isCancelingQuery = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanExecuteQuery);
                NotifyOfPropertyChange(()=> CanCancelExecutingQuery);
            }
        }

        public void CopyConnectionString()
        {
            var connectionString = ConnectionString;
            if (!string.IsNullOrEmpty(connectionString))
                Clipboard.SetText(connectionString);
        }

        public bool CanCopyConnectionString
        {
            get { return !string.IsNullOrEmpty(ConnectionString); }
        }

        protected override void OnViewLoaded(object view)
        {
            _view = (IDatabaseQueryRunnerView) view;
            _view.TextEditor.Text = _originalCommand;

            _view.TextEditor.TextChanged += (sender, args) =>
            {
                CommandText = _view.TextEditor.Text;
                IsDirty = string.Compare(_originalCommand, _view.TextEditor.Text, StringComparison.InvariantCulture) != 0;
                NotifyOfPropertyChange(() => CanExecuteQuery);
            };
            _view.TextEditor.TextArea.SelectionChanged += (sender, args) =>
            {
                NotifyOfPropertyChange(()=> SelectedText);
                NotifyOfPropertyChange(() => CanExecuteQuery);
            };
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
                _instances.Value.Remove(_instanceId);
        }
        
        private void UpdateDisplayName()
        {
            if (string.IsNullOrEmpty(_documentName))
            {
                _documentName = string.Format("SqlQuery {0}", _instances.Value.Count);
            }
            DisplayName = (IsDirty) ? _documentName + "*" : _documentName;
        }
    }
}