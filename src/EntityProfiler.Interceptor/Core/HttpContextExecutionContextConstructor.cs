namespace EntityProfiler.Interceptor.Core {
    using System;
    using System.Collections;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Remoting.Messaging;
    using System.Threading;
    using ExecutionContext = Common.Protocol.ExecutionContext;

    /// <summary>
    /// Adds HttpContext information
    /// </summary>
    internal sealed class HttpContextExecutionContextConstructor : IExecutionContextConstructor {
        private readonly Lazy<bool> _isSystemWebLoaded;
        private readonly Lazy<Type> _httpContextType;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public HttpContextExecutionContextConstructor() {
            this._isSystemWebLoaded = new Lazy<bool>(DetermineSystemWebLoaded, LazyThreadSafetyMode.PublicationOnly);
            this._httpContextType = new Lazy<Type>(() => Type.GetType("System.Web.HttpContext, System.Web", true), LazyThreadSafetyMode.PublicationOnly);
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
            HttpContextWrapper ctx = this.GetHttpContext();
            if (!ctx.IsAvailable) return null;

            int id = ctx.Id();
            string url = ctx.Url();

            string description = String.Format("#{0} {1}", id, url);
            return new ExecutionContext(id, description);
        }

        /// <summary>
        /// Modifies an execution execution context and adds more information to i
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="executionContext"></param>
        public void ModifyExistingExecutionContext(DbContext dbContext, ExecutionContext executionContext) {
            HttpContextWrapper ctx = this.GetHttpContext();
            if (!ctx.IsAvailable) return;

            executionContext.Values["HttpRequestSeq"] = ctx.Id();
            executionContext.Values["HttpRequestUrl"] = ctx.Url();
        }


        private HttpContextWrapper GetHttpContext() {
            // if system.web isn't loaded we are probably not running in a web context
            if (!this._isSystemWebLoaded.Value) {
                return default(HttpContextWrapper);
            }

            // in ASP.NET the host context is the HttpContext
            object context = CallContext.HostContext;

            if (context == null || context.GetType() != this._httpContextType.Value) {
                return default(HttpContextWrapper);
            }

            return new HttpContextWrapper(context);
        }

        private struct HttpContextWrapper {
            private static int _IdCounter = 0;
            private static readonly object Unavailable = new object();
            private readonly object _httpContext;
            private object _httpRequest;

            public bool IsAvailable {
                get { return this._httpContext != null; }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public HttpContextWrapper(object httpContext) : this() {
                this._httpContext = httpContext;
            }

            /// <summary>
            /// Gets the unique request id, assigns an id to the request if required
            /// </summary>
            /// <returns></returns>
            public int Id() {
                string key = this.GetType().FullName + "-RequestId";
                IDictionary dict = this.GetItemsDictionary();
                object boxedId = dict[key];
                if (boxedId != null && boxedId is Int32) {
                    return (int) boxedId;
                }

                int id = Interlocked.Increment(ref _IdCounter);
                dict[key] = id;

                return id;
            }

            /// <summary>
            /// Gets the url associated with the request
            /// </summary>
            /// <returns></returns>
            public string Url() {
                object httpRequest = this.GetHttpRequest();

                if (httpRequest != null) {
                    return GetProperty<Uri>(httpRequest, "Uri").ToString();
                }

                return null;
            }

            private object GetHttpRequest() {
                if (this._httpRequest != null) {
                    return this._httpRequest == Unavailable ? null : this._httpRequest;
                }

                try {
                    return this._httpRequest = GetProperty<object>(this._httpContext, "Request");
                }
                catch (TargetInvocationException) {
                    // request not available (yet)
                    return this._httpContext == Unavailable;
                }
            }

            private static T GetProperty<T>(object source, string property) {
                Type ctxType = source.GetType();
                PropertyInfo prop = ctxType.GetProperty(property, BindingFlags.Instance | BindingFlags.Public);
                Debug.Assert(prop != null);

                return (T) prop.GetValue(source);
            }

            private IDictionary GetItemsDictionary() {
                return GetProperty<IDictionary>(this._httpContext, "Items");
            }
        }
    }
}