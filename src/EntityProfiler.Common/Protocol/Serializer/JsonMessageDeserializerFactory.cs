namespace EntityProfiler.Common.Protocol.Serializer {
    using System.IO;

    internal class JsonMessageDeserializerFactory : IMessageDeserializerFactory {
        private readonly IMessageTypeResolver _typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public JsonMessageDeserializerFactory(IMessageTypeResolver typeResolver) {
            this._typeResolver = typeResolver;
        }

        public IMessageDeserializer CreateDeserializer(TextReader textReader) {
            return new JsonMessageDeserializer(this._typeResolver, textReader);
        }
    }
}