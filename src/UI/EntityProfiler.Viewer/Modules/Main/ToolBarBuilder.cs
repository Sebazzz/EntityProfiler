using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Gemini.Framework.Commands;
using Gemini.Framework.ToolBars;
using Gemini.Modules.ToolBars;
using Gemini.Modules.ToolBars.Models;

namespace EntityProfiler.Viewer.Modules.Main
{
    [Export(typeof(IToolBarBuilder))]
    public class ToolBarBuilder : IToolBarBuilder
    {
        private readonly ICommandService _commandService;
        private readonly ToolBarDefinition[] _toolBars;
        private readonly ToolBarItemGroupDefinition[] _toolBarItemGroups;
        private readonly ToolBarItemDefinition[] _toolBarItems;
        private readonly IEnumerable<ToolbarExcludeDefinition> _fromTextExcludeDefinition;
        private readonly IEnumerable<ToolbarExcludeDefinition> _fromCommandNameExcludeDefinition;

        [ImportingConstructor]
        public ToolBarBuilder(
            ICommandService commandService,
            [ImportMany] ToolBarDefinition[] toolBars,
            [ImportMany] ToolBarItemGroupDefinition[] toolBarItemGroups,
            [ImportMany] ToolBarItemDefinition[] toolBarItems,
            [ImportMany] ToolbarExcludeDefinition[] toolbarExcludeDefinition)
        {
            _commandService = commandService;
            _toolBars = toolBars;
            _toolBarItemGroups = toolBarItemGroups;
            _toolBarItems = toolBarItems;

            if (toolbarExcludeDefinition != null)
            {
                _fromTextExcludeDefinition = toolbarExcludeDefinition.Where(p => !String.IsNullOrEmpty(p.Text));
                _fromCommandNameExcludeDefinition = toolbarExcludeDefinition.Where(p => !String.IsNullOrEmpty(p.CommandDefinitionName));
            }
        }

        public void BuildToolBars(IToolBars result)
        {
            var toolBars = _toolBars.OrderBy(x => x.SortOrder);

            foreach (var toolBar in toolBars)
            {
                var toolBarModel = new ToolBarModel();
                BuildToolBar(toolBar, toolBarModel);
                if (toolBarModel.Any())
                    result.Items.Add(toolBarModel);
            }
        }

        public void BuildToolBar(ToolBarDefinition toolBarDefinition, IToolBar result)
        {
            var groups = _toolBarItemGroups
                .Where(x => x.ToolBar == toolBarDefinition)
                .OrderBy(x => x.SortOrder)
                .ToList();

            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var toolBarItems = _toolBarItems
                    .Where(x => x.Group == group && CanAddToolbarItem(x))
                    .OrderBy(x => x.SortOrder);

                foreach (var toolBarItem in toolBarItems)
                    result.Add(new CommandToolBarItem(toolBarItem,
                        _commandService.GetCommand(toolBarItem.CommandDefinition), result));

                if (i < groups.Count - 1 && toolBarItems.Any())
                    result.Add(new ToolBarItemSeparator());
            }
        }

        private bool CanAddToolbarItem(ToolBarItemDefinition toolBarItem)
        {
            var result = true;

            if (!String.IsNullOrEmpty(toolBarItem.Text) && _fromTextExcludeDefinition != null)
            {
                result = !_fromTextExcludeDefinition.Any(p => p.Text.Replace("_", "").Equals(toolBarItem.Text.Replace("_", ""), StringComparison.OrdinalIgnoreCase));
            }

            var commandDefinitionBase = toolBarItem.CommandDefinition;

            if (result && _fromCommandNameExcludeDefinition != null && commandDefinitionBase != null)
            {
                result = !_fromCommandNameExcludeDefinition.Any(p => p.CommandDefinitionName.Equals(commandDefinitionBase.Name, StringComparison.OrdinalIgnoreCase));
            }

            return result;
        }
    }

    public class ToolbarExcludeDefinition
    {
        public static ToolbarExcludeDefinition ExcludeWithText(string text)
        {
            return new ToolbarExcludeDefinition
            {
                Text = text
            };
        }

        public static ToolbarExcludeDefinition ExcludeWithCommandName(string commandDefinitionName)
        {
            return new ToolbarExcludeDefinition
            {
                CommandDefinitionName = commandDefinitionName
            };
        }

        public string Text { get; set; }
        public string CommandDefinitionName { get; set; }
    }
}