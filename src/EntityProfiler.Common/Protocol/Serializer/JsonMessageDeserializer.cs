namespace EntityProfiler.Common.Protocol.Serializer {
    using System;
    using System.IO;
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
            throw new System.NotImplementedException();
        }
    }
}