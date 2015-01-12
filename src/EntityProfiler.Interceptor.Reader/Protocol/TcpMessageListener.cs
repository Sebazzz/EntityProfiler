namespace EntityProfiler.Interceptor.Reader.Protocol {
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using Common.Events;
    using Common.Protocol;
    using Common.Protocol.Serializer;

    internal class TcpMessageListener : IMessageListener {
        private TcpClient _tcpClient;
        private readonly ITcpClientFactory _tcpClientFactory;
        private readonly IMessageDeserializerFactory _messageDeserializerFactory;
        private readonly MessageEventDispatcher _messageDispatcher;
        private readonly Thread _clientThread;

        private volatile bool _isStopping;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TcpMessageListener(ITcpClientFactory tcpClientFactory, IMessageDeserializerFactory messageDeserializerFactory, MessageEventDispatcher messageDispatcher) {
            this._tcpClientFactory = tcpClientFactory;
            this._messageDeserializerFactory = messageDeserializerFactory;
            this._messageDispatcher = messageDispatcher;

            this._clientThread = new Thread(this.ClientThread);
            this._clientThread.IsBackground = true;
        }

        /// <summary>
        /// Method in which the client thread runs
        /// </summary>
        private void ClientThread() {
            try {
                this._tcpClient = this._tcpClientFactory.CreateTcpClient();
            }
            catch (Exception ex) {
                this.DispatchError(ex);
                return;
            }

            IMessageDeserializer messageDeserializer =
                this._messageDeserializerFactory.CreateDeserializer(
                    new StreamReader(this._tcpClient.GetStream()));

            bool connectionError = false;
            while (!connectionError) {
                connectionError = this.DispatchIncomingMessage(messageDeserializer);
            }
        }

        private bool DispatchIncomingMessage(IMessageDeserializer messageDeserializer) {
            try {
                Message msg = messageDeserializer.DeserializeMessage();

                this._messageDispatcher.DispatchReceived(new MessageEvent(msg));
            }
            catch (Exception ex) {
                this.DispatchError(ex);

                if (ex.InnerException is IOException) {
                    return true;
                }

                if (ex.InnerException is ObjectDisposedException) {
                    return true;
                }
            }

            return false;
        }

        private void DispatchError(Exception ex) {
            if (!this._isStopping) {
                 this._messageDispatcher.DispatchReceived(new MessageEvent(ex));
            }
        }

        public void Start() {
            this.EnsureNotDisposed();
            if (this._clientThread.IsAlive) {
                throw new InvalidOperationException("This TcpInterceptorClient has already been started");
            }

            this._clientThread.Start();
        }

        public void Stop() {
            this.Dispose();
        }

        /// <summary>
        /// Creates a new instance based on the current
        /// </summary>
        public IMessageListener Clone() {
            return new TcpMessageListener(
                this._tcpClientFactory,
                this._messageDeserializerFactory,
                this._messageDispatcher);
        }

        private void EnsureNotDisposed() {
            if (this._isDisposed) {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            if (this._isDisposed) {
                return;
            }

            this._isStopping = true;

            // if we yank the TcpConnection out of the client thread the client thread will stop eventually
            if (this._tcpClient != null) this._tcpClient.Close();
            if (this._clientThread.IsAlive) this._clientThread.Join(1000);

            this._isDisposed = true;
        }
    }
}