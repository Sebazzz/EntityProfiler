namespace EntityProfiler.Common.Protocol.Serializer {
    using System.IO;

    /// <summary>
    /// Serializer which serializes messages using <c>SimpleJson</c>
    /// </summary>
    internal class JsonMessageSerializer : StringMessageSerializer {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public JsonMessageSerializer(TextWriter textWriter) : base(textWriter) {}
        protected override void SerializeMessage(Message message, TextWriter textWriter) {
            throw new System.NotImplementedException();
        }
    }
}