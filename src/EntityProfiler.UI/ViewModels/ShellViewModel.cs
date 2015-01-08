﻿namespace EntityProfiler.UI.ViewModels {
    using System;
    using Caliburn.Micro;
    using Common.Events;
    using Common.Protocol;
    using Interceptor.Reader.Protocol;
    using PropertyChanged;

    [ImplementPropertyChanged]
    public class ShellViewModel : Screen, IShell, IHandle<MessageEvent> {
        private readonly IMessageListener _messageListener;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public ShellViewModel(IMessageListener messageListener, IEventAggregator eventAggregator) {
            this._messageListener = messageListener;
            
            eventAggregator.Subscribe(this);
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

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="event">The message.</param>
        public void Handle(MessageEvent @event) {
            if (@event.Exception != null) {
                this.HandleError(@event.Exception);
                return;
            }

            QueryMessage queryMessage = @event.Message as QueryMessage;
            if (queryMessage != null) {
                this.HandleQueryMessage(queryMessage);
            }
        }

        private void HandleError(Exception exception) {
            this.StatusBar = exception.GetType().FullName;
        }

        private int _msgCount = 0;
        private void HandleQueryMessage(QueryMessage queryMessage) {
            this.StatusBar = this._msgCount++.ToString() + " messages received";
        }
    }
}