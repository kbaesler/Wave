using System.Threading.Tasks;

namespace System.Runtime.Caching
{
    /// <summary>
    ///     Provides extension methods for the <see cref="MemoryCache" />
    /// </summary>
    public static class MemoryCacheExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets or adds the item into the memory cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <param name="addItem">The function delegate used to add the item into the cache.</param>
        /// <param name="policy">The policy.</param>
        /// <returns>Returns a <see cref="T" /> representing the item in the cache.</returns>
        public static T GetOrAdd<T>(this MemoryCache source, string key, Func<string, T> addItem, CacheItemPolicy policy)
        {
            ValidateKey(key);

            var lazyCacheItem = new Lazy<T>(() => addItem(key));

            EnsureRemovedCallback<T>(policy);

            var cacheItem = source.AddOrGetExisting(key, lazyCacheItem, policy);

            if (cacheItem != null)
                return Unwrap<T>(cacheItem);

            try
            {
                return lazyCacheItem.Value;
            }
            catch
            {
                source.Remove(key);
                throw;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Ensures the removed callback is handled properly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="policy">The policy.</param>
        private static void EnsureRemovedCallback<T>(CacheItemPolicy policy)
        {
            if (policy != null && policy.RemovedCallback != null)
            {
                var callback = policy.RemovedCallback;
                policy.RemovedCallback = args =>
                {
                    var item = args != null ? (args.CacheItem != null ? args.CacheItem.Value as Lazy<T> : null) : null;
                    if (item != null)
                        args.CacheItem.Value = item.IsValueCreated ? item.Value : default(T);

                    callback(args);
                };
            }
        }

        /// <summary>
        ///     Unwraps the lazy value object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns>Returns the <see cref="T" /> representing the value of the object.</returns>
        private static T Unwrap<T>(object item)
        {
            var lazy = item as Lazy<T>;
            if (lazy != null)
                return lazy.Value;

            if (item is T)
                return (T) item;

            var task = item as Task<T>;
            if (task != null)
                return task.Result;

            return default(T);
        }

        /// <summary>
        ///     Validates the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="System.ArgumentNullException">key</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">key - Cache keys cannot be empty or whitespace</exception>
        private static void ValidateKey(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentOutOfRangeException("key", "Cache keys cannot be empty or whitespace");
        }

        #endregion
    }
}