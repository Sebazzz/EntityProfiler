namespace EntityProfiler.Common.Protocol.Serializer {
    using System.IO;

    internal class JsonMessageSerializerFactory : IMessageSerializerFactory {
        private readonly IMessageTypeResolver _typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public JsonMessageSerializerFactory(IMessageTypeResolver typeResolver) {
            this._typeResolver = typeResolver;
        }

        public IMessageSerializer CreateSerializer(TextWriter textWriter) {
            return new JsonMessageSerializer(this._typeResolver, textWriter);
        }
    }
}