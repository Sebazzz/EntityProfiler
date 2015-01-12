namespace EntityProfiler.UI.ViewModels {
    using System.Collections.ObjectModel;

    public class Record {
        private readonly ObservableCollection<Property> properties = new ObservableCollection<Property>();

        public Record(params Property[] properties) {
            foreach (var property in properties)
                Properties.Add(property);
        }

        public ObservableCollection<Property> Properties {
            get { return properties; }
        }
    }

    public class Property {
        public Property(string name, object value) {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public object Value { get; set; }
    }
}