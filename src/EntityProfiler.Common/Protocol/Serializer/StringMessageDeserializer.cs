namespace EntityProfiler.Common.Protocol.Serializer {
    using System;
    using System.IO;
    using Annotations;

    /// <summary>
    /// Represents a serializer for <see cref="Message"/> from a string
    /// </summary>
    internal abstract class StringMessageDeserializer : IMessageDeserializer {
        private readonly TextReader _textReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        protected StringMessageDeserializer([NotNull] TextReader textReader) {
            if (textReader == null) {
                throw new ArgumentNullException("textReader");
            }

            this._textReader = textReader;
        }

        public Message DeserializeMessage() {
            try {
                return this.DeserializeMessage(this._textReader);
            } catch (Exception ex) {
                throw new MessageTransferException("Failed to deserialize the message from the target", ex);
            }
        }

        protected abstract Message DeserializeMessage(TextReader textReader);
    }
}