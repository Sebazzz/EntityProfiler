namespace EntityProfiler.Tests.Integration.Protocol {
    using System.Threading;
    using Common.Events;
    using Common.Protocol;
    using Common.Protocol.Serializer;
    using Interceptor.Protocol;
    using Interceptor.Reader.Protocol;
    using NUnit.Framework;

    [TestFixture]
    public sealed class MessageRoundTripTests {
        private IMessageSink _messageSink;
        private IMessageListener _messageListener;
        private DelegateMessageEventSubscriber _eventSubscriber;
        private MessageEventDispatcher _messageEventDispatcher;

        [TestFixtureSetUp]
        public void FixtureSetup() {
            this._eventSubscriber = new DelegateMessageEventSubscriber();
            this._messageEventDispatcher = new MessageEventDispatcher(new[]{this._eventSubscriber});
            this._messageSink = new TcpMessageSink(
                new TcpListenerFactory(),
                this._messageEventDispatcher,
                new JsonMessageSerializerFactory(CreateTypeResolver()));
        }

        private static UnitTestMessageTypeResolver CreateTypeResolver() {
            return new UnitTestMessageTypeResolver(typeof(FakeMessage), typeof(ConnectedMessage));
        }

        [TestFixtureTearDown]
        public void FixtureShutdown() {
            this._messageSink.Dispose();
        }

        [SetUp]
        public void TestSetup() {
            this._messageSink.Start();
            this._messageListener = new TcpMessageListener(
                new TcpClientFactory(), new JsonMessageDeserializerFactory(CreateTypeResolver()), this._messageEventDispatcher);
        }

        [TearDown]
        public void TestTearDown() {
            this._eventSubscriber.Reset();
            this._messageListener.Dispose();
        }

        [Test]
        public void MessageRoundTrip_SimpleTest() {
            // given
            FakeMessage sentMessage = new FakeMessage("test1", 1337);
            this._messageListener.Start();

            // when
            this._messageSink.DispatchMessage(sentMessage);

            // then
            this._eventSubscriber.AssertReceivedConnectMessage();

            MessageEvent messageEvent = this._eventSubscriber.GetReceivedMessage(100);

            Assert.IsNull(messageEvent.Exception, "Exception occurred during receiving of messages");
            FakeMessage receivedMessage = (FakeMessage) messageEvent.Message;

            Assert.That(() => receivedMessage.Prop1, Is.EqualTo(sentMessage.Prop1));
            Assert.That(() => receivedMessage.Prop2, Is.EqualTo(sentMessage.Prop2));

            this._eventSubscriber.AssertNoFurtherMessagesReceived();
        }
    }
}