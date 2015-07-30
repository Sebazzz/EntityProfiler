using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;

namespace EntityProfiler.Viewer.Modules.Connection
{
    [CommandHandler]
    public class StopReaderCommandHandler : CommandHandlerBase<StopReaderCommandDefinition>
    {
        private readonly IConnectionHandler _connectionHandler;

        [ImportingConstructor]
        public StopReaderCommandHandler(IConnectionHandler connectionHandler)
        {
            _connectionHandler = connectionHandler;
        }

        public override Task Run(Command command)
        {
            return _connectionHandler.StopSession();
        }

        public override void Update(Command command)
        {
            var result = _connectionHandler.ConnectionRequestState == ConnectionRequestState.Start ||
                         _connectionHandler.ConnectionRequestState == ConnectionRequestState.Pause;

            command.Enabled = !_connectionHandler.IsBusy && result;
            command.Visible = _connectionHandler.ConnectionRequestState >= ConnectionRequestState.Start;
        }
    }
}