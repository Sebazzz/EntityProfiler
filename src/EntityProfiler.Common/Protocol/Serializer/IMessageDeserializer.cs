namespace EntityProfiler.Common.Protocol.Serializer {
    /// <summary>
    /// Defines the interface for serializers which can serialize a message to a target
    /// </summary>
    internal interface IMessageDeserializer {
        /// <summary>
        /// Deserializes a new message. Depending on the implementor the method may block forever until a message is received
        /// or returns <c>null</c> if the read times out
        /// </summary>
        /// <returns></returns>
        Message DeserializeMessage();
    }
}