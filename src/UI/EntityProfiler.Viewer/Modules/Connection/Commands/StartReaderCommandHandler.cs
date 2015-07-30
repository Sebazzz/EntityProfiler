using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Gemini.Framework.Commands;

namespace EntityProfiler.Viewer.Modules.Connection
{
    [CommandHandler]
    public class StartReaderCommandHandler : CommandHandlerBase<StartReaderCommandDefinition>
    {
        private readonly IConnectionHandler _connectionHandler;

        [ImportingConstructor]
        public StartReaderCommandHandler(IConnectionHandler connectionHandler)
        {
            _connectionHandler = connectionHandler;
        }

        public override Task Run(Command command)
        {
            return _connectionHandler.StartSession();
        }

        public override void Update(Command command)
        {
            var sessionState = _connectionHandler.ConnectionRequestState;
            var result = sessionState == ConnectionRequestState.Initial ||
                         sessionState == ConnectionRequestState.Pause ||  
                         sessionState == ConnectionRequestState.Stop;

            command.Enabled = !_connectionHandler.IsBusy && result;
            command.Text = sessionState == ConnectionRequestState.Initial || sessionState == ConnectionRequestState.Stop ? "Start" : "Continue";
            
        }
    }
}