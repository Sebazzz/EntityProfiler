namespace EntityProfiler.UI.ViewModels {
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Caliburn.Micro;
    using Common.Annotations;
    using Common.Protocol;
    using Interceptor.Reader.Core;
    using PropertyChanged;

    public class DataContextViewModel : INotifyPropertyChanged {
        private IObservableCollection<QueryMessageViewModel> _queries;

        /// <seealso cref="ExecutionContext.Identifier"/>
        public ContextIdentifier Identifier { get; set; }

        public string Description { get; set; }

        [AlsoNotifyFor("NumberOfQueries")]
        public IObservableCollection<QueryMessageViewModel> Queries {
            get { return this._queries; }
        }

        public int NumberOfQueries {
            get { return this.CountQueries(); }
        }

        private int CountQueries() {
            int normalQueryCount = this.Queries.Count(x => !(x.Model is DuplicateQueryMessage));
            int dupQueryCount = this.Queries.Where(x => x.Model is DuplicateQueryMessage).Sum(x => ((DuplicateQueryMessage)x.Model).NumberOfQueries);

            return dupQueryCount + normalQueryCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DataContextViewModel() {
            this._queries = new BindableCollection<QueryMessageViewModel>();
            this._queries.CollectionChanged += this.OnQueryCollectionChanged;
        }

        private void OnQueryCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            this.OnPropertyChanged("NumberOfQueries");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = this.PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [ImplementPropertyChanged]
    public class QueryMessageViewModel {
        public int Index { get; set; }

        public string QueryPart {
            get {
                // try to get table
                string query = this.Model.Query.CommandText.Replace(Environment.NewLine, "");
                return query.Substring(0, Math.Min(25, query.Length));
            }
        }

        public QueryMessage Model { get; set; }

        public IEnumerable<Record> Parameters {
            get {
                DuplicateQueryMessage dupQuery = this.Model as DuplicateQueryMessage;
                if (dupQuery != null) {
                    return GetQueryParameters(dupQuery.Query.ParameterCollection);
                }

                return GetQueryParameters(this.Model.Query.Parameters);
            }
        }

        private static IEnumerable<Record> GetQueryParameters(Dictionary<string, object> parameters) {
            return parameters.Select(param => 
                new Record(
                    new Property("Name", param.Key),
                    new Property("Value", param.Value)));
        }

        private static IEnumerable<Record> GetQueryParameters(DataTable parameters) {
            return parameters.Select(entry => new Record(GetParameters(entry).ToArray()));
        }

        private static IEnumerable<Property> GetParameters(DataTable.DataTableEntry entry) {
            yield return new Property("#", entry.Row);

            foreach (var kvp in entry.Columns) {
                yield return new Property(kvp.Key, kvp.Value);
            }
        }
    }


    public static class HelperExtensions {

        public static void Add(this IObservableCollection<QueryMessageViewModel> collection, QueryMessage message) {
            collection.Add(new QueryMessageViewModel() {
                Index = collection.Count,
                Model = message
            });
        }
    }
}