namespace EntityProfiler.UI.Services {
    using System;
    using System.Threading;
    using System.Windows.Threading;

    internal class OneTimeAction {
        private readonly DispatcherTimer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        private OneTimeAction(TimeSpan interval, Action callback) {
            Dispatcher current = Dispatcher.FromThread(Thread.CurrentThread);
            if (current == null) {
                throw new InvalidOperationException("No dispatcher running on current thread");
            }

            this._timer = new DispatcherTimer(interval, DispatcherPriority.Normal, (_,__) => callback.Invoke(), current);
            this._timer.Start();
        }

        public static OneTimeAction Execute(int milliseconds, Action callback) {
            return new OneTimeAction(TimeSpan.FromMilliseconds(milliseconds), callback);
        }

        public OneTimeAction CancelExisting(OneTimeAction other) {
            if (other != null) {
                other.Cancel();
            }

            return this;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Cancel() {
            // cancel the timer
            this._timer.Stop();
        }
    }
}