using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace EntityProfiler.Viewer.Modules.ContextExplorer
{
    [CommandHandler]
    public class ViewContextExplorerCommandHandler : CommandHandlerBase<ViewContextExplorerDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewContextExplorerCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<ContextExplorerViewModel>();
            return TaskUtility.Completed;
        }
    }
}