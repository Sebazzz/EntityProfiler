namespace EntityProfiler.Common.Protocol.Serializer {
    using System;

    /// <summary>
    /// Interface for classes that aid in resolving types for messaging
    /// </summary>
    internal interface IMessageTypeResolver {
        Type ResolveType(string simpleTypeName);
        string CreateTypeRef(Type type);
    }
}