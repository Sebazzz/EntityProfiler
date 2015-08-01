using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using EntityProfiler.Common.Annotations;
using EntityProfiler.Common.Protocol;
using EntityProfiler.Interceptor.Reader.Core;
using EntityProfiler.Viewer.PresentationCore;

namespace EntityProfiler.Viewer.Modules.Connection.ViewModels
{
    public class DataContextViewModel : INotifyPropertyChanged, IEquatable<DataContextViewModel>
    {
        private string _description;
        private ContextIdentifier _identifier;
        private BindableCollection<QueryMessageViewModel> _queries;
        private bool _isHidden;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public DataContextViewModel()
        {
            IsHidden = false;
            Queries = new BindableCollection<QueryMessageViewModel>();
        }

        /// <seealso cref="ExecutionContext.Identifier" />
        public ContextIdentifier Identifier
        {
            get { return _identifier; }
            set
            {
                if (Equals(value, _identifier)) return;
                _identifier = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                if (value == _isHidden) return;
                _isHidden = value;
                OnPropertyChanged();
            }
        }

        public BindableCollection<QueryMessageViewModel> Queries
        {
            get { return _queries; }
            private set
            {
                if (Equals(value, _queries)) return;

                if (_queries != null)
                    _queries.CollectionChanged -= OnQueryCollectionChanged;

                _queries = value;
                
                if(_queries != null)
                    _queries.CollectionChanged += OnQueryCollectionChanged;

                OnPropertyChanged();
            }
        }

        public int NumberOfQueries
        {
            get { return CountQueries(); }
        }

        private int CountQueries()
        {
            var normalQueryCount = Queries.Count(x => !(x.Model is DuplicateQueryMessage));
            var dupQueryCount =
                Queries.Where(x => x.Model is DuplicateQueryMessage)
                    .Sum(x => ((DuplicateQueryMessage) x.Model).NumberOfQueries);

            return dupQueryCount + normalQueryCount;
        }

        private void OnQueryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("NumberOfQueries");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool Equals(DataContextViewModel other)
        {
            return other != null && this == other;
        }

        public override string ToString()
        {
            return Description;
        }
    }

    public class CallStackViewModel
    {
        private StackTrace _sourceStackTrace;

        public CallStackViewModel()
        {
        }

        public CallStackViewModel(StackTrace sourceStackTrace)
        {
            SourceStackTrace = sourceStackTrace;
        }

        public StackTrace SourceStackTrace
        {
            get { return _sourceStackTrace; }
            set
            {
                _sourceStackTrace = value;
                LoadStackTrace(_sourceStackTrace);
            }
        }

        public int NumberOfFrames { get; private set; }

        private void LoadStackTrace(StackTrace stackTrace)
        {
            var frames = stackTrace.Frames;
            NumberOfFrames = stackTrace.Frames.Count();
            CreateStackTraceInfo(frames);
        }
        
        private void CreateStackTraceInfo(StackFrame[] frames)
        {
            if(frames == null)
                return;
            InCodeStackFrames = frames.Reverse().Where(p => !string.IsNullOrEmpty(p.FilePath));
        }

        public IEnumerable<StackFrame> InCodeStackFrames { get; private set; }
    }

    public class QueryMessageViewModel : INotifyPropertyChanged
    {
        private int _index;
        private QueryMessage _model;
        private bool _isHidden;
        private CallStackViewModel _callStackModel;
        private IEnumerable<Record> _parameters;

        public int Index
        {
            get { return _index; }
            set
            {
                if (value == _index) return;
                _index = value;
                OnPropertyChanged();
            }
        }

        public string QueryPart
        {
            get
            {
                // try to get table
                var shortQuery = Model.Query.CommandText.Replace(Environment.NewLine, " ").PostfixLongLiteral();
                return shortQuery;
            }
        }

        public QueryMessage Model
        {
            get { return _model; }
            set
            {
                if (Equals(value, _model)) return;
                _model = value;
                OnPropertyChanged();
                OnPropertyChanged("QueryPart");
                OnPropertyChanged("Parameters");
                OnPropertyChanged("CallStackModel");
            }
        }


        public CallStackViewModel CallStackModel
        {
            get
            {
                if (_callStackModel == null && _model != null && _model.Context != null)
                {
                    _callStackModel = new CallStackViewModel(Model.Context.CallStack);
                }
                return _callStackModel;
            }
        }
        
        public IEnumerable<Record> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    var dupQuery = Model as DuplicateQueryMessage;
                    if (dupQuery != null)
                    {
                        var parameterCollection = dupQuery.Query.ParameterCollection;
                        _parameters = GetQueryParameters(parameterCollection);
                    }
                    else
                    {
                        _parameters = GetQueryParameters(Model.Query.Parameters);
                    }
                }
                return _parameters;
            }
        }

        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                if (value == _isHidden) return;
                _isHidden = value;
                OnPropertyChanged();
            }
        }

        private static IEnumerable<Record> GetQueryParameters(Dictionary<string, object> parameters)
        {
            return parameters.Select(param =>
                new Record(
                    new Property("Name", param.Key),
                    new Property("Value", param.Value)));
        }

        private static IEnumerable<Record> GetQueryParameters(DataTable parameters)
        {
            return parameters.Select(entry => new Record(GetParameters(entry).ToArray()));
        }

        private static IEnumerable<Property> GetParameters(DataTable.DataTableEntry entry)
        {
            yield return new Property("#", entry.Row);

            foreach (var kvp in entry.Columns)
            {
                yield return new Property(kvp.Key, kvp.Value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public static class HelperExtensions
    {
        public static void AddOnTop(this IObservableCollection<QueryMessageViewModel> collection, QueryMessage message)
        {
            collection.Insert(0, new QueryMessageViewModel
            {
                Index = collection.Count,
                Model = message
            });
        }

        public static void Add(this IObservableCollection<QueryMessageViewModel> collection, QueryMessage message)
        {
            collection.Add(new QueryMessageViewModel
            {
                Index = collection.Count,
                Model = message
            });
        }
    }
}