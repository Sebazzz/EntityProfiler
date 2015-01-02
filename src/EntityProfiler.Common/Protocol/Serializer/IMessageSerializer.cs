namespace EntityProfiler.Common.Protocol.Serializer {
    /// <summary>
    /// Defines the interface for serializers which can serialize a message to a target
    /// </summary>
    internal interface IMessageSerializer {
        void SerializeMessage(Message message);
    }
}