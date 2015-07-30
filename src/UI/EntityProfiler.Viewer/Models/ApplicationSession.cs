using System.Collections.Generic;

namespace EntityProfiler.Viewer.Models
{
    public class ApplicationSession
    {
        private IDictionary<string, string> _caches;

        public IDictionary<string, string> Caches
        {
            get { return _caches ?? (_caches = new Dictionary<string, string>()); }
            set { _caches = value; }
        }
    }
}