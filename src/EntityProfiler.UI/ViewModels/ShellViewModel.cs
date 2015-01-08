namespace EntityProfiler.UI.ViewModels {
    using System;
    using Caliburn.Micro;
    using Interceptor.Reader.Protocol;
    using PropertyChanged;

    [ImplementPropertyChanged]
    public class ShellViewModel : Screen, IShell {
        private readonly IMessageListener _messageListener;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public ShellViewModel(IMessageListener messageListener) {
            this._messageListener = messageListener;
        }

        public string StatusBar { get; set; }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize() {
            base.OnInitialize();

            this.DisplayName = "Entity Profiler";
            this.StatusBar = "Loading?";
        }

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        [Obsolete("This is a design-time only constructor")]
        public ShellViewModel() {
            this.StatusBar = "Connected";
        }

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public sealed override void NotifyOfPropertyChange(string propertyName = null) {
            base.NotifyOfPropertyChange(propertyName);
        }
    }
}