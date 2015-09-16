using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;

namespace EntityProfiler.Viewer.Modules.Connection
{
    [CommandHandler]
    public class PauseReaderCommandHandler : CommandHandlerBase<PauseReaderCommandDefinition>
    {
        private readonly IConnectionHandler _connectionHandler;

        [ImportingConstructor]
        public PauseReaderCommandHandler(IConnectionHandler connectionHandler)
        {
            _connectionHandler = connectionHandler;
        }

        public override Task Run(Command command)
        {
            return _connectionHandler.PauseSession();
        }

        public override void Update(Command command)
        {
            var result = _connectionHandler.ConnectionRequestState == ConnectionRequestState.Start;
            command.Enabled = !_connectionHandler.IsBusy && result;
            command.Visible = _connectionHandler.ConnectionRequestState >= ConnectionRequestState.Start;
        }
    }
}