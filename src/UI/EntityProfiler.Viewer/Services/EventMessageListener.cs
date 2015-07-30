using Caliburn.Micro;
using EntityProfiler.Common.Events;

namespace EntityProfiler.Viewer.Services
{
    internal sealed class EventMessageListener : IMessageEventSubscriber
    {
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public EventMessageListener(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        /// <summary>
        ///     Occurs when a message is received
        /// </summary>
        /// <param name="event"></param>
        public void OnReceived(MessageEvent @event)
        {
            _eventAggregator.BeginPublishOnUIThread(@event);
        }

        /// <summary>
        ///     Occurs before a message is send
        /// </summary>
        /// <param name="event"></param>
        public void OnSending(MessageEvent @event)
        {
            // we are currently not interested in these
        }
    }
}