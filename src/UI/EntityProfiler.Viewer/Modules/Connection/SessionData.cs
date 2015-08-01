using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using EntityProfiler.Common.Annotations;
using EntityProfiler.Common.Protocol;
using EntityProfiler.Interceptor.Reader.Core;
using EntityProfiler.Viewer.Modules.Connection.ViewModels;
using ExecutionContext = EntityProfiler.Common.Protocol.ExecutionContext;

namespace EntityProfiler.Viewer.Modules.Connection
{
    public class SessionData : Freezable, INotifyPropertyChanged
    {
        private static readonly Lazy<SessionData> _current =
            new Lazy<SessionData>(() => new SessionData(Guid.NewGuid()));

        private readonly DuplicateQueryDetectionMessageFilter _messageFilter;
        private readonly Guid _sessionId;
        private BindableCollection<DataContextViewModel> _dataContexts;
        private DataContextViewModel _selectedDataContext;
        private QueryMessageViewModel _selectedQuery;
        private bool _autoSelectedDataContext;

        private readonly DispatcherTimer _notificationTimer;

        [Obsolete("This is a design-time only constructor, use static Current instead.")]
        public SessionData()
        {
        }

        private SessionData(Guid sessionId)
        {
            _sessionId = sessionId;
            _messageFilter = new DuplicateQueryDetectionMessageFilter();
            _notificationTimer = new DispatcherTimer(DispatcherPriority.Normal,
                Dispatcher.FromThread(Thread.CurrentThread)) {Interval = TimeSpan.FromMilliseconds(300)};
            _notificationTimer.Tick += NotificationTimerOnTick;
        }

        private void NotificationTimerOnTick(object sender, EventArgs eventArgs)
        {
            _notificationTimer.Stop();
            UpdateUI();
        }

        public static SessionData Current
        {
            get { return _current.Value; }
        }

        public string StatusBar { get; set; }

        public IObservableCollection<QueryMessageViewModel> Queries
        {
            [DebuggerStepThrough]
            get
            {
                return SelectedDataContext != null
                    ? SelectedDataContext.Queries
                    : new BindableCollection<QueryMessageViewModel>();
            }
        }

        public IObservableCollection<DataContextViewModel> DataContexts
        {
            get
            {
                if (_dataContexts == null)
                {
                    _dataContexts = new BindableCollection<DataContextViewModel>();
                    _dataContexts.CollectionChanged += DataContextsOnCollectionChanged;
                }
                return _dataContexts;
            }
        }

        public ReadOnlyCollection<DataContextViewModel> VisibleDataContexts
        {
            get { return new ReadOnlyCollection<DataContextViewModel>(DataContexts.Where(p => !p.IsHidden).ToList()); }
        }

        public ReadOnlyCollection<DataContextViewModel> HiddenDataContexts
        {
            get { return new ReadOnlyCollection<DataContextViewModel>(DataContexts.Where(p => p.IsHidden).ToList()); }
        }

        public bool HasHiddenDataContexts
        {
            get { return DataContexts.Any(p => p.IsHidden); }
        }

        public DataContextViewModel SelectedDataContext
        {
            get { return _selectedDataContext; }
            set
            {
                if (Equals(value, _selectedDataContext)) return;
                _selectedDataContext = value;
                OnPropertyChanged();
                OnPropertyChanged("Queries");

                if (_notificationTimer.IsEnabled)
                    return;

                SelectFirstQuery();
            }
        }

        public QueryMessageViewModel SelectedQuery
        {
            get { return _selectedQuery; }
            set
            {
                if (Equals(value, _selectedQuery)) return;
                _selectedQuery = value;
                OnPropertyChanged();
            }
        }

        public bool AutoSelectedDataContext
        {
            get { return _autoSelectedDataContext; }
            set
            {
                if (value == _autoSelectedDataContext) return;
                _autoSelectedDataContext = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void DataContextsOnCollectionChanged(object s, NotifyCollectionChangedEventArgs e)
        {
            InvalidateDataContextsVisibility();
        }

        public void ShowAllDataContexts()
        {
            foreach (var dataContextViewModel in DataContexts)
            {
                dataContextViewModel.IsHidden = false;
            }
            InvalidateDataContextsVisibility();
        }

        public void ShowDataContext(DataContextViewModel dataContextViewModel)
        {
            if (dataContextViewModel != null)
            {
                dataContextViewModel.IsHidden = false;
                InvalidateDataContextsVisibility();
            }
        }

        public void HideDataContext(DataContextViewModel dataContextViewModel)
        {
            if (dataContextViewModel != null)
            {
                var isSelected = SelectedDataContext == dataContextViewModel;
                var indexOf = VisibleDataContexts.IndexOf(dataContextViewModel);
                var count = VisibleDataContexts.Count - 1;
                dataContextViewModel.IsHidden = true;
                InvalidateDataContextsVisibility();
                if (isSelected)
                {
                    SelectedDataContext = indexOf == count
                        ? VisibleDataContexts.LastOrDefault()
                        : VisibleDataContexts.Skip(indexOf).FirstOrDefault();
                }
            }
        }

        private void InvalidateDataContextsVisibility()
        {
            OnPropertyChanged("VisibleDataContexts");
            OnPropertyChanged("HiddenDataContexts");
            OnPropertyChanged("HasHiddenDataContexts");
        }
        
        internal void HandleQueryMessage(QueryMessage queryMessage)
        {
            // find data context
            var dataContext = GetOrCreateDataContext(queryMessage.Context);
            var queries = dataContext.Queries;

            if (queries.IsNotifying)
                queries.IsNotifying = false;

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (queries.Count == 0)
                {
                    queries.AddOnTop(queryMessage);
                }
                else
                {
                    // try to merge with last
                    var lastQueryMessage = queries[queries.Count - 1].Model;
                    var merged = _messageFilter.FilterTwo(lastQueryMessage, queryMessage) as QueryMessage;

                    if (merged == null)
                    {
                        queries.AddOnTop(queryMessage);
                    }
                    else
                    {
                        queries[queries.Count - 1].Model = merged;
                    }
                }

                // auto-select if no data context has been selected
                if (AutoSelectedDataContext && SelectedDataContext == null)
                {
                    SelectedDataContext = dataContext;
                }
            });
            
            _notificationTimer.Start();
        }

        private void UpdateUI()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Queries.IsNotifying = true;
                Queries.Refresh();
                SelectFirstQuery();
            });
        }

        private void SelectFirstQuery()
        {
            if (SelectedQuery != null && Queries.Contains(SelectedQuery))
                return;

            SelectedQuery = Queries.FirstOrDefault();
        }

        private DataContextViewModel GetOrCreateDataContext(ExecutionContext context)
        {
            var result = _dataContexts.FirstOrDefault(x => x.Identifier == context.Identifier);
            if (result != null)
            {
                return result;
            }

            result = new DataContextViewModel
            {
                Description = context.Description,
                Identifier = context.Identifier
            };

            _dataContexts.Insert(0, result);

            return result;
        }
        
        protected override Freezable CreateInstanceCore()
        {
            return Current;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}