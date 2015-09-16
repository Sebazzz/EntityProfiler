namespace EntityProfiler.Common.Protocol.Serializer
{
    using System.Reflection;

    /// <summary>
    /// Represents the default message type resolver, which resolves type using the Common assembly
    /// </summary>
    internal sealed class DefaultMessageTypeResolver : SingleAssemblyMessageTypeResolver
    {
        private readonly Assembly _targetAssembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DefaultMessageTypeResolver()
        {
            this._targetAssembly = this.GetType().Assembly;
        }

        protected override Assembly TargetAssembly
        {
            get { return this._targetAssembly; }
        }
    }
}