using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace EntityProfiler.Viewer.Modules.ContextExplorer
{
    [Export(typeof(ContextExplorerViewModel))]
    public class ContextExplorerViewModel : Tool
    {
        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Left; }
        }

        public override string DisplayName
        {
            get { return "Object contexts"; }
        }
    }
}