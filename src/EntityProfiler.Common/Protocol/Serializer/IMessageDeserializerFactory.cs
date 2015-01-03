namespace EntityProfiler.Common.Protocol.Serializer {
    using System.IO;

    internal interface IMessageDeserializerFactory {
        IMessageDeserializer CreateDeserializer(TextReader textReader);
    }
}