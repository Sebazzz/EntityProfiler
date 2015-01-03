namespace EntityProfiler.Interceptor.Reader.Protocol {
    using System.Net;
    using System.Net.Sockets;
    using Common.Protocol;

    internal class TcpClientFactory : ITcpClientFactory {
        public TcpClient CreateTcpClient() {
            TcpClient client = new TcpClient();

            client.Connect(new IPEndPoint(IPAddress.Loopback, Constants.Portnumber));

            return client;
        }
    }
}