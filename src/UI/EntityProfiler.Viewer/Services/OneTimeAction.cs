using System;
using System.Threading;
using System.Windows.Threading;

namespace EntityProfiler.Viewer.Services
{
    internal class OneTimeAction
    {
        private readonly DispatcherTimer _timer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        private OneTimeAction(TimeSpan interval, Action callback)
        {
            var current = Dispatcher.FromThread(Thread.CurrentThread);
            if (current == null)
            {
                throw new InvalidOperationException("No dispatcher running on current thread");
            }

            _timer = new DispatcherTimer(interval, DispatcherPriority.Normal, (_, __) => callback.Invoke(), current);
            _timer.Start();
        }

        public static OneTimeAction Execute(int milliseconds, Action callback)
        {
            return new OneTimeAction(TimeSpan.FromMilliseconds(milliseconds), callback);
        }

        public OneTimeAction CancelExisting(OneTimeAction other)
        {
            if (other != null)
            {
                other.Cancel();
            }

            return this;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Cancel()
        {
            // cancel the timer
            _timer.Stop();
        }
    }
}