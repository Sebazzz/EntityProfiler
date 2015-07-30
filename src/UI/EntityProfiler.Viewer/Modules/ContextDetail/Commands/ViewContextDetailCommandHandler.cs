using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;

namespace EntityProfiler.Viewer.Modules.ContextDetail
{
    [CommandHandler]
    public class ViewContextDetailCommandHandler : CommandHandlerBase<ViewContextDetailDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewContextDetailCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.OpenDocument(IoC.Get<ContextDetailViewModel>());
            return TaskUtility.Completed;
        }
    }
}