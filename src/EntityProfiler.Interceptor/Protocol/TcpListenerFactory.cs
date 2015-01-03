namespace EntityProfiler.Interceptor.Protocol {
    using System.Net;
    using System.Net.Sockets;
    using Common.Protocol;

    internal interface ITcpListenerFactory {
        TcpListener CreateListener();
    }

    internal class TcpListenerFactory : ITcpListenerFactory {
        public TcpListener CreateListener() {
            TcpListener listener = new TcpListener(
                new IPEndPoint(
                    IPAddress.Loopback, Constants.Portnumber));

            listener.ExclusiveAddressUse = false;

            return listener;
        }
    }
}