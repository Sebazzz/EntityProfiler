namespace EntityProfiler.Common {
    using Protocol.Serializer;

    /// <summary>
    /// Helper class for registration and getting dependencies
    /// </summary>
    internal static class Dependency {
        /// <summary>
        /// Configures dependencies into the dependency container
        /// </summary>
        /// <param name="container"></param>
        internal static void Configure(TinyIoC.TinyIoCContainer container) {
            container.Register<IMessageTypeResolver, DefaultMessageTypeResolver>();

            container.Register<IMessageSerializer, JsonMessageSerializer>();
            container.Register<IMessageDeserializer, JsonMessageDeserializer>();
        }
    }
}