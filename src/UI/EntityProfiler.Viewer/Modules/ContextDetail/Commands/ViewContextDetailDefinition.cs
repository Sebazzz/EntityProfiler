using Gemini.Framework.Commands;

namespace EntityProfiler.Viewer.Modules.ContextDetail
{
    [CommandDefinition]
    public class ViewContextDetailDefinition : CommandDefinition
    {
        public const string CommandName = "View.ContextDetail";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "Context detail"; }
        }

        public override string ToolTip
        {
            get { return "Context detail"; }
        }
    }
}