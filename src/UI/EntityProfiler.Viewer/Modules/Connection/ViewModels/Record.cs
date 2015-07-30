using System;
using System.Collections.ObjectModel;

namespace EntityProfiler.Viewer.Modules.Connection.ViewModels
{
    public class Record
    {
        private readonly ObservableCollection<Property> _properties = new ObservableCollection<Property>();

        public Record(params Property[] properties)
        {
            Identifier = Guid.NewGuid();
            foreach (var property in properties)
                Properties.Add(property);
        }

        public Guid Identifier { get; private set; }

        public ObservableCollection<Property> Properties
        {
            get { return _properties; }
        }
    }

    public class Property
    {
        public Property(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public object Value { get; set; }
    }
}