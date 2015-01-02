namespace EntityProfiler.Common.Protocol.Serializer {
    using System.IO;

    /// <summary>
    /// Deserializer which deserializes messages using <c>SimpleJson</c>
    /// </summary>
    internal class JsonMessageDeserializer : StringMessageDeserializer {
        private readonly IMessageTypeResolver _typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public JsonMessageDeserializer(IMessageTypeResolver typeResolver, TextReader textReader) : base(textReader) {
            this._typeResolver = typeResolver;
        }

        protected override Message DeserializeMessage(TextReader textReader) {
            throw new System.NotImplementedException();
        }
    }
}