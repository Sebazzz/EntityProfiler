namespace EntityProfiler.UI.ViewModels {
    using System;
    using System.ComponentModel.Composition;
    using Caliburn.Micro;
    using PropertyChanged;

    [Export(typeof (IShell))]
    [ImplementPropertyChanged]
    public class ShellViewModel : Screen, IShell {

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
            this.StatusBar = "XX";
        }
    }
}