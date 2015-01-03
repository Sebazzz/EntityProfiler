namespace EntityProfiler.Common.Protocol.Serializer {
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using Annotations;

    /// <summary>
    /// Deserializer which deserializes messages using <c>SimpleJson</c>
    /// </summary>
    /// <remarks>
    /// For the message format used, see doc at <see cref="JsonMessageSerializer"/>.
    /// </remarks>
    internal class JsonMessageDeserializer : StringMessageDeserializer {
        private readonly IMessageTypeResolver _typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public JsonMessageDeserializer([NotNull] IMessageTypeResolver typeResolver, TextReader textReader) : base(textReader) {
            if (typeResolver == null) {
                throw new ArgumentNullException("typeResolver");
            }

            this._typeResolver = typeResolver;
        }

        protected override Message DeserializeMessage(TextReader textReader) {
            // read "header"
            ReadHeader(textReader);

            // read type string
            Type readType = this.ReadType(textReader);

            // read json
            Message message = ReadMessageBody(readType, textReader);

            // read footer
            ReadFooter(textReader);

            return message;
        }

        private static void ReadHeader(TextReader textReader) {
            // by reading everything till the header we also ignore any - possible corrupt
            // or partial characters left from the previous message
            TextReaderUtils.ReadToString(JsonMessageSerializer.Format.Header, textReader);
        }

        private static void ReadFooter(TextReader textReader) {
            TextReaderUtils.ReadToString(JsonMessageSerializer.Format.Footer, textReader);
        }

        private static Message ReadMessageBody(Type targetType, TextReader textReader) {
            string json = ReadJson(textReader);

            try {
                return (Message) SimpleJson.DeserializeObject(json, targetType);
            }
            catch (InvalidCastException ex) {
                throw new MessageTransferException("Unknown internal error", ex);
            }
            catch (SerializationException ex) {
                throw new MessageFormatException(String.Format("Invalid JSON: {0}", json), ex);
            }
        }

        private static string ReadJson(TextReader textReader) {
            return TextReaderUtils.ReadPrefixedString(textReader);
        }

        private Type ReadType(TextReader textReader) {
            string typeName = TextReaderUtils.ReadPrefixedString(textReader);

            // get type
            try {
                return this._typeResolver.ResolveType(typeName);
            }
            catch (TypeLoadException ex) {
                throw new MessageFormatException(
                    String.Format("Unable to find type '{0}'", typeName), ex);
            }
        }
    }
}