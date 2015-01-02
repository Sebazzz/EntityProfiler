namespace EntityProfiler.Common.Protocol.Serializer {
    using System;
    using System.IO;

    /// <summary>
    /// Represents a serializer for <see cref="Message"/> to a string
    /// </summary>
    internal abstract class StringMessageSerializer : IMessageSerializer {
        private readonly TextWriter _textWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        protected StringMessageSerializer(TextWriter textWriter) {
            this._textWriter = textWriter;
        }

        public void SerializeMessage(Message message) {
            try {
                this.SerializeMessage(message, this._textWriter);
            } catch (Exception ex) {
                throw new MessageTransferException("Failed to serialize the message to the target", ex);
            }
        }

        protected abstract void SerializeMessage(Message message, TextWriter textWriter);
    }
}