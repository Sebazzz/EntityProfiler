using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Caliburn.Micro;
using EntityProfiler.Common.Annotations;
using EntityProfiler.Common.Protocol;
using EntityProfiler.Interceptor.Reader.Core;
using EntityProfiler.Viewer.Modules.Connection.ViewModels;

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

        [Obsolete("This is a design-time only constructor, use static Current instead.")]
        public SessionData()
        {
        }

        private SessionData(Guid sessionId)
        {
            _sessionId = sessionId;
            _messageFilter = new DuplicateQueryDetectionMessageFilter();
            PropertyChanged += OnPropertyChanged;
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
                var count = VisibleDataContexts.Count -1;
                dataContextViewModel.IsHidden = true;
                InvalidateDataContextsVisibility();
                if (isSelected)
                {
                    SelectedDataContext = indexOf == count ?
                        VisibleDataContexts.LastOrDefault() :
                        VisibleDataContexts.Skip(indexOf).FirstOrDefault();
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
            if (queries.Count == 0)
            {
                queries.Add(queryMessage);
                return;
            }

            // try to merge with last
            var lastQueryMessage = queries[queries.Count - 1].Model;
            var merged = _messageFilter.FilterTwo(lastQueryMessage, queryMessage) as QueryMessage;

            if (merged == null)
            {
                queries.Add(queryMessage);
            }
            else
            {
                queries[queries.Count - 1].Model = merged;
            }

            // auto-select if no data context has been selected
            if (SelectedDataContext == null)
            {
                SelectedDataContext = dataContext;
            }
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

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedDataContext":
                    // select the last query
                    SelectedQuery = SelectedDataContext != null ? SelectedDataContext.Queries.LastOrDefault() : null;
                    break;
            }
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