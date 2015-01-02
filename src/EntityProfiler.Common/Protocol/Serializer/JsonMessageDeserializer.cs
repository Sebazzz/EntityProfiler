namespace EntityProfiler.Common.Protocol.Serializer {
    using System.IO;

    /// <summary>
    /// Deserializer which deserializes messages using <c>SimpleJson</c>
    /// </summary>
    internal class JsonMessageDeserializer : StringMessageDeserializer {
        protected override Message DeserializeMessage(TextReader textReader) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public JsonMessageDeserializer(TextReader textReader) : base(textReader) {}
    }
}