using System.Net;
using System.Net.Sockets;
using EntityProfiler.Common.Protocol;

namespace EntityProfiler.Interceptor.Reader.Protocol
{
    internal class TcpClientFactory : ITcpClientFactory
    {
        public TcpClient CreateTcpClient()
        {
            var client = new TcpClient();

            client.Connect(new IPEndPoint(IPAddress.Loopback, Constants.Portnumber));

            return client;
        }
    }
}