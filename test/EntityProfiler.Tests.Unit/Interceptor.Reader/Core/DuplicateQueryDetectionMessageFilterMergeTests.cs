namespace EntityProfiler.Tests.Unit.Interceptor.Reader.Core {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using EntityProfiler.Common.Protocol;
    using EntityProfiler.Interceptor.Reader.Core;
    using NUnit.Framework;

    [TestFixture]
    public sealed class DuplicateQueryDetectionMessageFilterMergeTests {
        [Test]
        public void DuplicateQueryDetectionMessageFilter_MergesEqualQueries_ToDuplicateQueryInstance() {
            // given
            IMessageFilter messageFilter = new DuplicateQueryDetectionMessageFilter();
            QueryMessage first = MakeQueryMessage("SELECT 1");
            QueryMessage second = MakeQueryMessage("SELECT 1");

            // when
            DuplicateQueryMessage output = (DuplicateQueryMessage) messageFilter.FilterTwo(first, second);

            // assert
            Assert.That(output.NumberOfQueries, Is.EqualTo(1 + 1));
        }

         [Test]
        public void DuplicateQueryDetectionMessageFilter_MergesNonEqualQueries_ToNullInstance() {
            // given
            IMessageFilter messageFilter = new DuplicateQueryDetectionMessageFilter();
            QueryMessage first = MakeQueryMessage("SELECT 2");
            QueryMessage second = MakeQueryMessage("SELECT 1");

            // when
            Message output = messageFilter.FilterTwo(first, second);

            // assert
            Assert.That(output, Is.Null, "Expected not to merge");
        }

        [Test]
        public void DuplicateQueryDetectionMessageFilter_MergesEqualQueriesWithDuplicate_ToDuplicateQueryInstance() {
            // given
            IMessageFilter messageFilter = new DuplicateQueryDetectionMessageFilter();
            QueryMessage first = MakeDuplicateQueryMessage("SELECT 1", 4);
            QueryMessage second = MakeQueryMessage("SELECT 1");

            // when
            DuplicateQueryMessage output = (DuplicateQueryMessage) messageFilter.FilterTwo(first, second);

            // assert
            Assert.That(output.NumberOfQueries, Is.EqualTo(1 + 4));
        }

        private static DuplicateQueryMessage MakeDuplicateQueryMessage(string queryText, int count) {
            Debug.Assert(count > 1);

            return new DuplicateQueryMessage() {
                Context = new ExecutionContext(new ContextIdentifier(DateTime.UtcNow, 0)),
                Performance = new AggregatePerformanceData() { Times = new long[count]},
                Query = new AggregateQuery() {
                    CommandText = queryText,
                    Parameters = new Dictionary<string, object>(),
                    ParameterCollection = new DataTable(
                        Enumerable.Repeat(new Dictionary<string,object>(), count).ToArray())
                },
                NumberOfQueries = count
            };
        }

        private static QueryMessage MakeQueryMessage(string queryText) {
            return new DbReaderQueryMessage() {
                Context = new ExecutionContext(new ContextIdentifier(DateTime.UtcNow, 0)),
                Performance = new PerformanceData(),
                Query = new Query() {
                    CommandText = queryText,
                    Parameters = new Dictionary<string, object>()
                }
            };
        }
    }
}