using Gemini.Framework.Commands;

namespace EntityProfiler.Viewer.Modules.ContextExplorer
{
    [CommandDefinition]
    public class ViewContextExplorerDefinition : CommandDefinition
    {
        public const string CommandName = "View.ContextExplorer";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Objects contexts"; }
        }

        public override string ToolTip
        {
            get { return "Objects contexts"; }
        }
    }
}