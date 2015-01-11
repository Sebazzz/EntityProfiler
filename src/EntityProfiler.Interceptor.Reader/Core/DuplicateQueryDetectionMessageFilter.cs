namespace EntityProfiler.Interceptor.Reader.Core {
    using System;
    using System.Collections.Generic;
    using Common.Protocol;

    /// <summary>
    /// Represents a <see cref="IMessageFilter"/> that detects for duplicate queries (SELECT N+1) specifically and filters them
    /// </summary>
    internal class DuplicateQueryDetectionMessageFilter : IMessageFilter {
        private const int QueryThreshold = 1;

        public IEnumerable<Message> Filter(IEnumerable<Message> messages) {
            Query checkQuery = null;
            List<QueryMessage> queryMessages = new List<QueryMessage>(50);

            foreach (Message message in messages) {
                // only handle query messages
                QueryMessage currentMessage = message as QueryMessage;

                if (currentMessage == null || message is DuplicateQueryMessage) {
                    yield return message;
                    continue;
                }

                if (!AreQueriesEqual(checkQuery, currentMessage.Query)) {
                    foreach (var msg in FinalizeGroup(queryMessages)) yield return msg;

                    checkQuery = null;
                }

                if (checkQuery == null) {
                    // buffer
                    checkQuery = currentMessage.Query;
                    queryMessages.Add(currentMessage);
                    continue;
                }

                // query is still equal - buffer
                queryMessages.Add(currentMessage);
            }

            // flush buffer
            foreach (var msg in FinalizeGroup(queryMessages)) yield return msg;
        }

        private static IEnumerable<Message> FinalizeGroup(List<QueryMessage> queryMessages) {
// can't group one query (QueryThreshold)
            if (queryMessages.Count <= QueryThreshold) {
                foreach (QueryMessage queryMessage in queryMessages) {
                    yield return queryMessage;
                }
            }
            else {
                // flush buffer
                yield return CreateGroupQuery(queryMessages);
            }

            queryMessages.Clear();
        }

        private static DuplicateQueryMessage CreateGroupQuery(List<QueryMessage> queryMessages) {
            FlattenExistingDuplicateQueryMessages(queryMessages);

            QueryMessage one = queryMessages[0];
            
            DuplicateQueryMessage duplicate = new DuplicateQueryMessage();
            duplicate.Context = one.Context;
            duplicate.Performance = AggregatePerformanceData.Create(queryMessages);
            duplicate.Query = AggregateQuery.Create(queryMessages);
            duplicate.Timestamp = one.Timestamp;
            duplicate.Error = one.Error;
            duplicate.NumberOfQueries = queryMessages.Count;

            return duplicate;
        }

        private static void FlattenExistingDuplicateQueryMessages(List<QueryMessage> queryMessages) {
            DuplicateQueryMessage duplicateQueryMessage = queryMessages[0] as DuplicateQueryMessage;
            if (duplicateQueryMessage == null) {
                return;
            }

            queryMessages.RemoveAt(0);

            // if at the beginning of the stream a duplicate query message we need to expand it so we can collapse it back into one
            foreach (DataTable.DataTableEntry entry in duplicateQueryMessage.Query.ParameterCollection) {
                DbReaderQueryMessage qm = new DbReaderQueryMessage();
                qm.Context = duplicateQueryMessage.Context;
                qm.Error = duplicateQueryMessage.Error;
                qm.Performance = new PerformanceData() {TotalTime = duplicateQueryMessage.Performance.Times[entry.Row]};
                qm.Timestamp = duplicateQueryMessage.Timestamp;
                qm.Query = new Query {
                    CommandText = duplicateQueryMessage.Query.CommandText,
                    Parameters = entry.Columns
                };

                queryMessages.Insert(entry.Row, qm);
            }
        }

        private static bool AreQueriesEqual(Query checkQuery, Query query) {
            if (checkQuery == null || query == null) {
                return false; // we know: this doesn't cover the case both parameters are null but this will never happen
            }

            return String.Equals(checkQuery.CommandText, query.CommandText, StringComparison.Ordinal);
        }
    }
}