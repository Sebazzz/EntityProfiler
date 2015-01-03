namespace EntityProfiler.Common.Protocol.Serializer {
    using System.IO;

    internal interface IMessageSerializerFactory {
        IMessageSerializer CreateSerializer(TextWriter textWriter);
    }
}