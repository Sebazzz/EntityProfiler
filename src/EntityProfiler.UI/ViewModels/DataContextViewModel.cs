namespace EntityProfiler.UI.ViewModels {
    using System.Linq;
    using Caliburn.Micro;
    using Common.Protocol;
    using Interceptor.Reader.Core;
    using PropertyChanged;

    public class DataContextViewModel {
        /// <seealso cref="ExecutionContext.Identifier"/>
        public ContextIdentifier Identifier { get; set; }

        public string Description { get; set; }

        public IObservableCollection<QueryMessageViewModel> Queries { get; set; }

        public int NumberOfQueries {
            get { return this.CountQueries(); }
        }

        private int CountQueries() {
            return this.Queries.Count(x => !(x.Model is DuplicateQueryMessage)) +
                   this.Queries.Where(x => x.Model is DuplicateQueryMessage).Sum(x => ((DuplicateQueryMessage)x.Model).NumberOfQueries);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DataContextViewModel() {
            this.Queries = new BindableCollection<QueryMessageViewModel>();
        }
    }

    [ImplementPropertyChanged]
    public class QueryMessageViewModel {
        
        public int Index { get; set; }

        public QueryMessage Model { get; set; }
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