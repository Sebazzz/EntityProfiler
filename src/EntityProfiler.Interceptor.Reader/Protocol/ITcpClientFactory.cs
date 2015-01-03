namespace EntityProfiler.Interceptor.Reader.Protocol {
    using System.Net.Sockets;

    internal interface ITcpClientFactory {
        TcpClient CreateTcpClient();
    }
}