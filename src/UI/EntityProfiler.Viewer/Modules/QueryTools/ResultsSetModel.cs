using System;
using System.Data;

namespace EntityProfiler.Viewer.Modules.QueryTools
{
    public class ResultsSetModel
    {
        public string Title { get; set; }

        public string StatusMessage { get; set; }
        
        public object ResultsData { get; set; }
        
        public Exception ExceptionDetails { get; set; }

        public DataTable ResultsDataTable
        {
            get { return ResultsData as DataTable; }
        }
    }
}