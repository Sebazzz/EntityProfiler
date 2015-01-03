namespace EntityProfiler.Tests.Integration.Protocol {
    using System.Reflection;
    using EntityProfiler.Common.Protocol.Serializer;

    internal class UnitTestMessageTypeResolver : SingleAssemblyMessageTypeResolver {
        private readonly Assembly _targetAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public UnitTestMessageTypeResolver() {
            this._targetAssembly = this.GetType().Assembly;
        }

        protected override Assembly TargetAssembly {
            get {return this._targetAssembly;}
        }
    }
}
