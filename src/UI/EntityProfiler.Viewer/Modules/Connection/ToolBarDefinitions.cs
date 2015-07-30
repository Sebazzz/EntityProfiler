using System.ComponentModel.Composition;
using Gemini.Framework.ToolBars;

namespace EntityProfiler.Viewer.Modules.Connection
{
    public static class ToolBarDefinitions
    {
        [Export]
        public static ToolBarDefinition ConnectionToolBar = new ToolBarDefinition(0, "Connection");

        [Export]
        public static ToolBarItemGroupDefinition StandardConnectionToolBarGroup = new ToolBarItemGroupDefinition(
         ConnectionToolBar, 0);

        [Export]
        public static ToolBarItemDefinition StartReaderToolBarItem = new CommandToolBarItemDefinition<StartReaderCommandDefinition>(
            StandardConnectionToolBarGroup, 0, ToolBarItemDisplay.IconAndText);

        [Export]
        public static ToolBarItemDefinition PauseReaderToolBarItem = new CommandToolBarItemDefinition<PauseReaderCommandDefinition>(
            StandardConnectionToolBarGroup, 1);

        [Export]
        public static ToolBarItemDefinition StopReaderToolBarItem = new CommandToolBarItemDefinition<StopReaderCommandDefinition>(
            StandardConnectionToolBarGroup, 2);
    }
}