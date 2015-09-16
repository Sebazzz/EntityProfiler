using Gemini.Framework.Commands;

namespace EntityProfiler.Viewer.Modules.Output.Commands
{
    [CommandDefinition]
    public class ViewOutputCommandDefinition : CommandDefinition
    {
        public const string CommandName = "View.Output";

        public override string Name
        {
            get { return CommandName; }
        }

        public override string Text
        {
            get { return "_Output"; }
        }

        public override string ToolTip
        {
            get { return "Output"; }
        }
    }
}