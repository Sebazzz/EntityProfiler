namespace EntityProfiler.Common.Protocol.Serializer {
    using System.IO;

    /// <summary>
    /// Serializer which serializes messages using <c>SimpleJson</c>
    /// </summary>
    internal class JsonMessageSerializer : StringMessageSerializer {
        private readonly IMessageTypeResolver _typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public JsonMessageSerializer(TextWriter textWriter, IMessageTypeResolver typeResolver) : base(textWriter) {
            this._typeResolver = typeResolver;
        }

        protected override void SerializeMessage(Message message, TextWriter textWriter) {
            throw new System.NotImplementedException();
        }
    }
}