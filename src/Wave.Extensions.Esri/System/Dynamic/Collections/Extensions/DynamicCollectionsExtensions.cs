using System.Collections.Generic;

namespace System.Dynamic
{
    /// <summary>
    ///     Provides extensions for dynamic types.
    /// </summary>
    public static class DynamicCollectionsExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Converts the dictionary to a <see cref="DynamicDictionary{TValue}" />
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The dictionary.</param>
        /// <returns>Returns a <see cref="DynamicDictionary{TValue}" /> representing the dictionary.</returns>
        /// <exception cref="ArgumentNullException">dictionary;Dictionary cannot be null</exception>
        public static dynamic ToDynamic<TValue>(this IDictionary<string, TValue> source)
        {
            if (source == null)
                throw new ArgumentNullException("source", "Dictionary cannot be null");

            return new DynamicDictionary<TValue>(source);
        }

        /// <summary>
        ///     Converts the dictionary to a <see cref="DynamicDictionary{TValue}" />
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The dictionary.</param>
        /// <returns>
        ///     Returns a <see cref="DynamicDictionary{TValue}" /> representing the dictionary.
        /// </returns>
        public static dynamic ToDynamic<TKey, TValue>(this KeyValuePair<TKey, TValue> source)
        {
            return new DynamicKeyValuePair<TKey, TValue>(source);
        }

        #endregion
    }
}