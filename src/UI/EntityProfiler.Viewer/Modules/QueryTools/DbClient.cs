using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EntityProfiler.Viewer.PresentationCore;
using Serilog;

namespace EntityProfiler.Viewer.Modules.QueryTools
{
    public class DbClient
    {
        public const string ProviderRegex = @"provider\s*=";

        public bool HasProvider(string connectionString)
        {
            return Regex.Match(connectionString, ProviderRegex, RegexOptions.IgnoreCase).Success;
        }
        
        public async Task<IEnumerable<ExecuteResults>> ExecuteSqlAsync(ExecuteParameters param, int timeout, CancellationToken cancellationToken)
        {
            var dtNow = DateTime.Now;
            Log.Information(@"Query execute started at {executeDate}. Connection : {connectionString}. {sql}",
                dtNow,
                param.ConnectionString.RemoveConnectionStringSecurity(),
                param.SqlStatement);
            
            var results = new List<ExecuteResults>();

            try
            {
                using (var c = new SqlConnection(param.ConnectionString))
                {
                    await c.OpenAsync(cancellationToken);
                    
                    using (SqlCommand cmd = c.CreateCommand())
                    {
                        cmd.CommandTimeout = timeout;
                        cmd.CommandText = param.SqlStatement;
                        
                        using (SqlDataReader dataReader = await cmd.ExecuteReaderAsync(cancellationToken))
                        {
                            do
                            {
                                dtNow = DateTime.Now;
                                cancellationToken.ThrowIfCancellationRequested();

                                var executeResults = new ExecuteResults();
                                try
                                {
                                    executeResults.RecordsAffected = dataReader.RecordsAffected;

                                    var dataTable = new DataTable();
                                    dataTable.Load(dataReader);
                                    
                                    executeResults.ResultsData = dataTable;
                                    executeResults.RowsCount = dataTable.Rows.Count;
                                    executeResults.RecordsAffected = dataReader.RecordsAffected;
                                    executeResults.StatusMessage = string.Format("Completed in {0}. ", (DateTime.Now - dtNow));
                                }
                                catch (Exception ex)
                                {
                                    executeResults.ExceptionDetails = ex;
                                    executeResults.StatusMessage = string.Format("Completed with errors in {0}. ", (DateTime.Now - dtNow));
                                }

                                if (executeResults.RowsCount != null)
                                    executeResults.StatusMessage = executeResults.StatusMessage + executeResults.RowsCount + " row(s) returned. ";

                                if (executeResults.RecordsAffected > -1)
                                    executeResults.StatusMessage = executeResults.StatusMessage + executeResults.RecordsAffected + " records affected. ";

                                results.Add(executeResults);
                                
                            } while (!dataReader.IsClosed);

                            if (!dataReader.IsClosed)
                            {
                                dataReader.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!results.Any())
                {
                    results.Add(new ExecuteResults());
                }
                var lastResult = results.Last();
                lastResult.ExceptionDetails = ex;
                lastResult.StatusMessage = string.Format("Completed with errors in {0}. ", (DateTime.Now - dtNow));
            }

            Log.Information(@"Query results. {executeResultsMessages}", results.SelectMany(p=> p.StatusMessage));

            return results;
        }
    }

    public class ExecuteParameters
    {
        public string SqlStatement { get; set; }

        public string ConnectionString { get; set; }
    }

    public class ExecuteResults
    {
        public string StatusMessage { get; set; }

        public int RecordsAffected { get; set; }

        public int? RowsCount { get; set; }

        public DataTable ResultsData { get; set; }

        public Exception ExceptionDetails { get; set; }
    }
}