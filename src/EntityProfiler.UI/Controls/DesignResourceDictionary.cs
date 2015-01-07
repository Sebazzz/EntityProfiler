namespace EntityProfiler.UI.Controls {
    using System.Windows;

    /// <summary>
    /// Dictionary which allows at least *some* design time support 
    /// </summary>
    public sealed class DesignResourceDictionary : ResourceDictionary {
        /// <summary>
        /// Occurs when the <see cref="T:System.Windows.ResourceDictionary"/> receives a request for a resource.
        /// </summary>
        /// <param name="key">The key of the resource to get.</param><param name="value">The value of the requested resource.</param><param name="canCache">true if the resource can be saved and used later; otherwise, false.</param>
        // ReSharper disable once RedundantAssignment
        protected override void OnGettingValue(object key, ref object value, out bool canCache) {
            // no-op
            canCache = false;
        }
    }
}
