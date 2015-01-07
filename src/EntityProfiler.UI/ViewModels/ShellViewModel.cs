namespace EntityProfiler.UI.ViewModels {
    using System.ComponentModel.Composition;
    using Caliburn.Micro;

    [Export(typeof (IShell))]
    public class ShellViewModel : Screen, IShell {
        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize() {
            base.OnInitialize();

            this.DisplayName = "Entity Profiler";
        }
    }
}