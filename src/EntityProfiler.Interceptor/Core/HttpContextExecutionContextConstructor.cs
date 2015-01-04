namespace EntityProfiler.Interceptor.Core {
    using System;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using ExecutionContext = Common.Protocol.ExecutionContext;

    /// <summary>
    /// Adds HttpContext information
    /// </summary>
    internal sealed class HttpContextExecutionContextConstructor : IExecutionContextConstructor {
        private readonly Lazy<bool> _isSystemWebLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public HttpContextExecutionContextConstructor() {
            this._isSystemWebLoaded = new Lazy<bool>(DetermineSystemWebLoaded, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Determines if the System.Web assembly has been loaded
        /// </summary>
        /// <returns></returns>
        private static bool DetermineSystemWebLoaded() {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            return loadedAssemblies.Any(x => x.FullName.StartsWith("System.Web", StringComparison.Ordinal));
        }

        /// <summary>
        /// Creates an <see cref="ExecutionContext"/> instance or returns <c>null</c>
        /// </summary>
        /// <returns></returns>
        public ExecutionContext CreateExecutionContext(DbContext dbContext) {
            if (this.GetHttpContext()) return null;

            throw new NotImplementedException();
        }

        private bool GetHttpContext() {
            // if system.web isn't loaded we are probably not running in a web context
            if (!this._isSystemWebLoaded.Value) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Modifies an execution execution context and adds more information to i
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="executionContext"></param>
        public void ModifyExistingExecutionContext(DbContext dbContext, ExecutionContext executionContext) {
            throw new NotImplementedException();
        }
    }
}