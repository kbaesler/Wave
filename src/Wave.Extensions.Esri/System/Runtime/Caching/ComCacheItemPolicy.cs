using System.Runtime.InteropServices;

namespace System.Runtime.Caching
{
    /// <summary>
    /// A cache item policy that releases the com objects when removed.
    /// </summary>
    /// <seealso cref="System.Runtime.Caching.CacheItemPolicy" />
    public class ComCacheItemPolicy : CacheItemPolicy
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ComCacheItemPolicy" /> class.
        /// </summary>
        public ComCacheItemPolicy()
        {
            this.RemovedCallback = delegate(CacheEntryRemovedArguments arguments)
            {
                var o = arguments.CacheItem.Value;

                if (o != null && Marshal.IsComObject(o))
                {
                    while (Marshal.ReleaseComObject(o) > 0)
                    {
                    }
                }
            };
        }

        #endregion
    }
}