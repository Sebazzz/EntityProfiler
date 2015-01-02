namespace EntityProfiler.Common.Protocol.Serializer {
    using System;
    using System.Reflection;

    /// <summary>
    /// Resolves types from a specified assembly
    /// </summary>
    internal abstract class SingleAssemblyMessageTypeResolver : IMessageTypeResolver {
        protected abstract Assembly TargetAssembly { get; }

        public Type ResolveType(string simpleTypeName) {
            return this.TargetAssembly.GetType(simpleTypeName, true);
        }

        public string CreateTypeRef(Type type) {
            return type.FullName;
        }
    }
}