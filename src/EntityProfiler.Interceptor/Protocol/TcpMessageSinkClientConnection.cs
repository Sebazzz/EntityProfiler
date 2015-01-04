namespace EntityProfiler.Interceptor.Protocol {
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading;
    using Common.Protocol;
    using Common.Protocol.Serializer;

    /// <summary>
    /// Represents an accepted connection on a <see cref="TcpMessageSink"/>
    /// </summary>
    internal class TcpMessageSinkClientConnection : IDisposable {
        private readonly TcpClient _tcpClient;
        private readonly IMessageSerializerFactory _messageSerializerFactory;
        private readonly Lazy<IMessageSerializer> _messageSerializer;
        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TcpMessageSinkClientConnection(TcpClient tcpClient, IMessageSerializerFactory messageSerializerFactory) {
            this._tcpClient = tcpClient;
            this._messageSerializerFactory = messageSerializerFactory;

            this._messageSerializer = new Lazy<IMessageSerializer>(this.CreateSerializer, true);
        }

        private IMessageSerializer CreateSerializer() {
            lock (this._tcpClient) {
                return this._messageSerializerFactory.CreateSerializer(new StreamWriter(this._tcpClient.GetStream()));
            }
        }

        public void DispatchMessage(Message message) {
            this.EnsureNotDisposed();

            ThreadPool.QueueUserWorkItem(this.DispatchMessageInternal, message);
        }

        private void EnsureNotDisposed() {
            if (this._isDisposed) {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private void DispatchMessageInternal(object state) {
            try {
                DispatchMessageInternal((Message) state, this._messageSerializer.Value);
            }
            catch (SocketException) {
                // socket was closed - ignore
            }
            catch (IOException) {
                // random I/O error - ignore
            }
        }

        private static void DispatchMessageInternal(Message message, IMessageSerializer messageSerializer) {
            messageSerializer.SerializeMessage(message);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            if (this._isDisposed) {
                return;
            }

            this._tcpClient.Close();
            this._isDisposed = true;
        }
    }
}