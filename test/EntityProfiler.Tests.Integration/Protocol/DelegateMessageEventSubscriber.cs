namespace EntityProfiler.Tests.Integration.Protocol {
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;
    using NUnit.Framework;
    using Common.Events;
    using Common.Protocol;

    internal class DelegateMessageEventSubscriber : IMessageEventSubscriber {
        private readonly ConcurrentQueue<MessageEvent> _receivedMessageEvents;
        private readonly ConcurrentQueue<MessageEvent> _sentMessageEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DelegateMessageEventSubscriber() {
            this._receivedMessageEvents = new ConcurrentQueue<MessageEvent>();
            this._sentMessageEvents = new ConcurrentQueue<MessageEvent>();
        }

        /// <summary>
        /// Occurs when a message is received
        /// </summary>
        /// <param name="event"></param>
        public void OnReceived(MessageEvent @event) {
            Debug.WriteLine("Received: {0}", @event.Message == null ? @event.Exception.GetType().FullName : @event.Message.GetType().FullName, "");
            this._receivedMessageEvents.Enqueue(@event);
        }

        /// <summary>
        /// Occurs before a message is send
        /// </summary>
        /// <param name="event"></param>
        public void OnSending(MessageEvent @event) {
            Debug.WriteLine("Send: {0}", @event.Message == null ? @event.Exception.GetType().FullName : @event.Message.GetType().FullName, "");
            this._sentMessageEvents.Enqueue(@event);
        }

        public MessageEvent GetReceivedMessage(int timeout) {
            return GetFromQueue(this._receivedMessageEvents, timeout);
        }

        public MessageEvent GetSendingMessage(int timeout) {
            return GetFromQueue(this._sentMessageEvents, timeout);
        }

        public void AssertReceivedConnectMessage() {
            AssertConnectMessage(this._receivedMessageEvents);
        }

        private static void AssertConnectMessage(ConcurrentQueue<MessageEvent> queue) {
            MessageEvent ev = GetFromQueue(queue, 100);

            Assert.That(ev.Message, Is.InstanceOf<ConnectedMessage>());
        }

        private static MessageEvent GetFromQueue(ConcurrentQueue<MessageEvent> queue, int timeout) {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (true) {
                MessageEvent ev;

                if (queue.TryDequeue(out ev)) {
                    sw.Reset();

                    return ev;
                }

                if (sw.ElapsedMilliseconds > timeout) {
                    Assert.Fail("Failed to get message from queue within {0} ms", timeout);
                }

                Thread.Sleep(1);
            }
        }

        public void AssertNoFurtherMessagesReceived() {
            Assert.IsTrue(this._receivedMessageEvents.IsEmpty, "Expected the message event queue to be empty");
        }

        public void Reset() {
            ClearQueue(this._receivedMessageEvents);
            ClearQueue(this._sentMessageEvents);
        }

        private static void ClearQueue(ConcurrentQueue<MessageEvent> queue) {
            while (!queue.IsEmpty) {
                MessageEvent bogus;
                queue.TryDequeue(out bogus);
            }
        }
    }
}