namespace EntityProfiler.Tests.Unit.Interceptor.Reader.Core {
    using System.Collections.Generic;
    using System.Linq;
    using EntityProfiler.Common.Protocol;
    using EntityProfiler.Interceptor.Reader.Core;
    using NUnit.Framework;

    [TestFixture]
    internal class DuplicateQueryDetectionMessageFilterTheoryGroupsAdjecentQueryMessages {
        private const int Single = 1;

        [Theory]
        public void DuplicateQueryDetectionMessageFilter_GroupsAdjecentQueryMessages(QueryData queryData) {
            // given
            IMessageFilter messageFilter = new DuplicateQueryDetectionMessageFilter();

            // when
            Queue<Message> messages = new Queue<Message>(messageFilter.Filter(queryData.Messages));

            // then
            foreach (int msgCount in queryData.ReturnedMessages) {
                if (msgCount == Single) {
                    Message message = messages.Dequeue();

                    Assert.That(message, Is.Not.InstanceOf<DuplicateQueryMessage>(), "Expected a singular message to be returned");
                    continue;
                }

                // find group
                DuplicateQueryMessage groupMessage = (DuplicateQueryMessage)messages.Dequeue();

                // assert group size
                Assert.IsNotNull(groupMessage, "Cannot find grouped query - expected at least a group to be found of size "+msgCount);
                Assert.That(groupMessage.NumberOfQueries, Is.EqualTo(msgCount), "Incorrect query group message size");
            }
        }

        [Datapoint]
        public readonly QueryData OneTwoOne = new QueryData() {
            Messages = new[] { MakeQueryMessage("SELECT 1"), MakeQueryMessage("SELECT 2"), MakeQueryMessage("SELECT 2"), MakeQueryMessage("SELECT 1") },
            ReturnedMessages = new[] { Single, 2, Single }
        };


        [Datapoint]
        public readonly QueryData TwoOneOneTwo = new QueryData() {
            Messages = new[] { MakeQueryMessage("SELECT 2"), MakeQueryMessage("SELECT 2"), MakeQueryMessage("SELECT 1"), MakeQueryMessage("SELECT 3"), MakeQueryMessage("SELECT 2"), MakeQueryMessage("SELECT 2") },
            ReturnedMessages = new[] { 2, Single, Single, 2 }
        };

        private static Message MakeQueryMessage(string queryText) {
            return new DbReaderQueryMessage() {
                Context = new ExecutionContext(0),
                Performance = new PerformanceData(),
                Query = new Query() {
                    CommandText = queryText,
                    Parameters = new Dictionary<string, object>()
                }
            };
        }

        public struct QueryData {
            public Message[] Messages;
            public int[] ReturnedMessages;
        }
    }
}