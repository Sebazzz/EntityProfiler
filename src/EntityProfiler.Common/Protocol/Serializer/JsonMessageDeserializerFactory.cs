using System.IO;

namespace EntityProfiler.Common.Protocol.Serializer
{
    internal class JsonMessageDeserializerFactory : IMessageDeserializerFactory
    {
        private readonly IMessageTypeResolver _typeResolver;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public JsonMessageDeserializerFactory(IMessageTypeResolver typeResolver)
        {
            _typeResolver = typeResolver;
        }

        public IMessageDeserializer CreateDeserializer(TextReader textReader)
        {
            return new JsonMessageDeserializer(_typeResolver, textReader);
        }
    }
}