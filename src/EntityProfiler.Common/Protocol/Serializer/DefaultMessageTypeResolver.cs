namespace EntityProfiler.Common.Protocol.Serializer {
    using System;
    using System.Reflection;

    /// <summary>
    /// Represents the default message type resolver, which resolves type using the Common assembly
    /// </summary>
    internal sealed class DefaultMessageTypeResolver : IMessageTypeResolver {
        private readonly Assembly _targetAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DefaultMessageTypeResolver() {
            this._targetAssembly = this.GetType().Assembly;
        }

        public Type ResolveType(string simpleTypeName) {
            return this._targetAssembly.GetType(simpleTypeName, true);
        }

        public string CreateTypeRef(Type type) {
            return type.FullName;
        }
    }
}