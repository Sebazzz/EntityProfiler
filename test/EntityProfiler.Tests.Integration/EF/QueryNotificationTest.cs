namespace EntityProfiler.Tests.Integration.EF {
    using System;
    using System.Linq;
    using Common.Events;
    using Common.Protocol;
    using Common.Protocol.Serializer;
    using Interceptor.Protocol;
    using Interceptor.Reader.Protocol;
    using NUnit.Framework;
    using Protocol;
    using Support;

    [TestFixture]
    //[Timeout(10000)]
    public sealed class QueryNotificationTest : TcpLockedTest {
        private IMessageListener _messageListener;
        private DelegateMessageEventSubscriber _eventSubscriber;
        private MessageEventDispatcher _messageEventDispatcher;

        [TestFixtureSetUp]
        public void FixtureSetup() {
            this._eventSubscriber = new DelegateMessageEventSubscriber();
            this._messageEventDispatcher = new MessageEventDispatcher(new[]{this._eventSubscriber});
        }

        [SetUp]
        public void TestSetup() {
            // ensure an empty db context exists
            using (TestDbContext dbContext = TestDbContext.CreateReset()) {
                dbContext.SaveChanges();
            }

            // set-up a listener
            this._messageListener = new TcpMessageListener(
                new TcpClientFactory(), new JsonMessageDeserializerFactory(new DefaultMessageTypeResolver()), this._messageEventDispatcher);
        }

        [TearDown]
        public void TestTearDown() {
            this._eventSubscriber.Reset();
            this._messageListener.Dispose();
        }

        [Test]
        public void QueryNotification_DbReaderQueryTest() {
            // given
            this._messageListener.Start();

            TestDbContext dbContext = new TestDbContext();

            // when
            Assert.NotNull(dbContext.TestEntities.ToList());

            // then
            MessageEvent ev = this.GetMessageEvent();
            Message msg = ev.Message;
            
            Assert.IsNull(ev.Exception, "Connection error occurred");
            Assert.That(() => msg, Is.InstanceOf<DbReaderQueryMessage>());

            this._eventSubscriber.AssertNoFurtherMessagesReceived();
        }

        [Test]
        public void QueryNotification_InsertQueryTest() {
            // given
            this._messageListener.Start();

            TestDbContext dbContext = new TestDbContext();

            // when
            dbContext.TestEntities.Add(SomeEntity.CreateNew());
            dbContext.SaveChanges();

            // then
            MessageEvent ev = this.GetMessageEvent();
            Message msg = ev.Message;
            
            Assert.IsNull(ev.Exception, "Connection error occurred");
            Assert.That(() => msg, Is.InstanceOf<DbReaderQueryMessage>(), "We expected DbReaderQueryMessage to be returned");

            this._eventSubscriber.AssertNoFurtherMessagesReceived();
        }

        [Test]
        public void QueryNotification_CountQueryTest() {
            // given
            this._messageListener.Start();

            TestDbContext dbContext = new TestDbContext();

            // when
            int count = dbContext.TestEntities.Count();

            // then
            MessageEvent ev = this.GetMessageEvent();
            Message msg = ev.Message;
            
            Assert.IsNull(ev.Exception, "Connection error occurred");
            Assert.That(() => msg, Is.InstanceOf<DbReaderQueryMessage>(), "We expected DbReaderQueryMessage to be returned");

            this._eventSubscriber.AssertNoFurtherMessagesReceived();
        }

        private MessageEvent GetMessageEvent() {
            MessageEvent ev = default(MessageEvent);

            while (ev.Message == null || ev.Message is ConnectedMessage) {
                ev = this._eventSubscriber.GetReceivedMessage(5000);
            }

            return ev;
        }
    }
}