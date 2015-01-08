namespace EntityProfiler.Tests.Integration.Protocol {
    using System;
    using System.Linq;
    using EntityProfiler.Common.Protocol.Serializer;

    internal class UnitTestMessageTypeResolver : IMessageTypeResolver {
        private readonly Type[] _supportedTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public UnitTestMessageTypeResolver(params Type[] supportedTypes) {
            this._supportedTypes = supportedTypes;
        }

        public Type ResolveType(string simpleTypeName) {
            try {
                return
                    this._supportedTypes.First(x => String.Equals(x.FullName, simpleTypeName, StringComparison.Ordinal));
            }
            catch (InvalidOperationException) {
                throw new TypeLoadException("Unable to find type '"+simpleTypeName+"'");
            }
        }

        public string CreateTypeRef(Type type) {
            return type.FullName;
        }
    }
}
