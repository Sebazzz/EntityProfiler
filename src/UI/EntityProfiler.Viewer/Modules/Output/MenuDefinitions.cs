using System.ComponentModel.Composition;
using EntityProfiler.Viewer.Modules.Output.Commands;
using Gemini.Framework.Menus;

namespace EntityProfiler.Viewer.Modules.Output
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition ViewOutputMenuItem = new CommandMenuItemDefinition<ViewOutputCommandDefinition>(
            Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup, 1);
    }
}