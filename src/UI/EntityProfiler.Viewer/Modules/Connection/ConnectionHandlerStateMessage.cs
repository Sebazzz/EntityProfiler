namespace EntityProfiler.Viewer.Modules.Connection
{
    public class ConnectionHandlerStateMessage
    {
        public ConnectionHandlerStateMessage(ConnectionRequestState connectionRequestState, bool stateChangeEnd = false)
        {
            ConnectionRequestState = connectionRequestState;
            StateChangeEnd = stateChangeEnd;
        }

        public ConnectionRequestState ConnectionRequestState { get; set; }
        public bool StateChangeEnd { get; set; }
    }
}