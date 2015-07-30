using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using EntityProfiler.Common.Events;
using EntityProfiler.Common.Protocol.Serializer;

namespace EntityProfiler.Interceptor.Reader.Protocol
{
    internal class TcpMessageListener : IMessageListener
    {
        private readonly Thread _clientThread;
        private readonly IMessageDeserializerFactory _messageDeserializerFactory;
        private readonly MessageEventDispatcher _messageDispatcher;
        private readonly ITcpClientFactory _tcpClientFactory;
        private bool _isDisposed;
        private volatile bool _isStopping;
        private TcpClient _tcpClient;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TcpMessageListener(ITcpClientFactory tcpClientFactory,
            IMessageDeserializerFactory messageDeserializerFactory, MessageEventDispatcher messageDispatcher)
        {
            _tcpClientFactory = tcpClientFactory;
            _messageDeserializerFactory = messageDeserializerFactory;
            _messageDispatcher = messageDispatcher;

            _clientThread = new Thread(ClientThread) {IsBackground = true};
        }

        public void Start()
        {
            EnsureNotDisposed();
            if (_clientThread.IsAlive)
            {
                throw new InvalidOperationException("This TcpInterceptorClient has already been started");
            }

            _clientThread.Start();
        }

        public void Stop()
        {
            Dispose();
        }

        /// <summary>
        ///     Creates a new instance based on the current
        /// </summary>
        public IMessageListener Clone()
        {
            return new TcpMessageListener(
                _tcpClientFactory,
                _messageDeserializerFactory,
                _messageDispatcher);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isStopping = true;

            // if we yank the TcpConnection out of the client thread the client thread will stop eventually
            if (_tcpClient != null) _tcpClient.Close();
            if (_clientThread.IsAlive) _clientThread.Join(1000);

            _isDisposed = true;
        }

        /// <summary>
        ///     Method in which the client thread runs
        /// </summary>
        private void ClientThread()
        {
            try
            {
                _tcpClient = _tcpClientFactory.CreateTcpClient();
            }
            catch (Exception ex)
            {
                DispatchError(ex);
                return;
            }

            var messageDeserializer =
                _messageDeserializerFactory.CreateDeserializer(
                    new StreamReader(_tcpClient.GetStream()));

            var connectionError = false;
            while (!connectionError)
            {
                connectionError = DispatchIncomingMessage(messageDeserializer);
            }
        }

        private bool DispatchIncomingMessage(IMessageDeserializer messageDeserializer)
        {
            try
            {
                var msg = messageDeserializer.DeserializeMessage();

                _messageDispatcher.DispatchReceived(new MessageEvent(msg));
            }
            catch (Exception ex)
            {
                DispatchError(ex);

                if (ex.InnerException is IOException)
                {
                    return true;
                }

                if (ex.InnerException is ObjectDisposedException)
                {
                    return true;
                }
            }

            return false;
        }

        private void DispatchError(Exception ex)
        {
            if (!_isStopping)
            {
                _messageDispatcher.DispatchReceived(new MessageEvent(ex));
            }
        }

        private void EnsureNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}