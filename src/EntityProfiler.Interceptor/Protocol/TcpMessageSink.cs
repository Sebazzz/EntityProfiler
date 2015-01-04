namespace EntityProfiler.Interceptor.Protocol {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using Common.Events;
    using Common.Protocol;
    using Common.Protocol.Serializer;

    /// <summary>
    /// Represents a target for dispatching <see cref="Message"/> to
    /// </summary>
    internal class TcpMessageSink : IMessageSink {
        private readonly object _syncRoot = new object();
        private readonly TcpListener _tcpListener;
        private readonly MessageEventDispatcher _messageEventDispatcher;
        private readonly List<TcpMessageSinkClientConnection> _connections;
        private readonly IMessageSerializerFactory _messageSerializerFactory;
        private bool _isDisposed;
        private bool _isStarted;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TcpMessageSink(ITcpListenerFactory tcpListenerFactory, MessageEventDispatcher messageEventDispatcher, IMessageSerializerFactory messageSerializerFactory) {
            this._messageEventDispatcher = messageEventDispatcher;
            this._messageSerializerFactory = messageSerializerFactory;
            this._tcpListener = tcpListenerFactory.CreateListener();
            this._connections = new List<TcpMessageSinkClientConnection>();
        }

        /// <summary>
        /// Starts accepting connections
        /// </summary>
        public void Start() {
            this.EnsureNotDisposed();

            lock (this._syncRoot) {
                if (this._isStarted) {
                    return;
                }

                this._tcpListener.Start();
                this.ListenForConnectionAsync();
                this._isStarted = true;
            }
        }

        /// <summary>
        /// Dispatches a message to listening clients
        /// </summary>
        /// <param name="message"></param>
        public void DispatchMessage(Message message) {
            this.EnsureNotDisposed();

            this._messageEventDispatcher.DispatchSending(
                new MessageEvent(message));

            List<TcpMessageSinkClientConnection> connections;
            lock (this._connections) {
                connections = this._connections.ToList();
            }

            foreach (TcpMessageSinkClientConnection connection in connections) {
                connection.DispatchMessage(message);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            if (this._isDisposed) {
                return;
            }

            foreach (TcpMessageSinkClientConnection connection in this._connections) {
                connection.Dispose();
            }

            this._connections.Clear();
            this._tcpListener.Stop();
            this._isDisposed = true;
        }

        private void ListenForConnectionAsync() {
            this._tcpListener.BeginAcceptTcpClient(this.OnIncomingConnection, null);
        }

        /// <summary>
        /// Accepts an incoming connection - 
        /// </summary>
        /// <param name="ar"></param>
        private void OnIncomingConnection(IAsyncResult ar) {
            TcpClient tcpClient;

            try {
                tcpClient = this._tcpListener.EndAcceptTcpClient(ar);
            }
            catch (ObjectDisposedException) {
                // we are in process of being disposed
                return;
            }

            this.AcceptClient(tcpClient);

            // immediately accept any other connections
            this.ListenForConnectionAsync();
        }

        private void AcceptClient(TcpClient tcpClient) {
            TcpMessageSinkClientConnection clientConnection = 
                new TcpMessageSinkClientConnection(tcpClient, this._messageSerializerFactory);

            lock (this._connections) this._connections.Add(clientConnection);
        }

        private void EnsureNotDisposed() {
            if (this._isDisposed) {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}