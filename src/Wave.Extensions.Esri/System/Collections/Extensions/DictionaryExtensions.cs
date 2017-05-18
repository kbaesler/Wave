using System.Collections.Generic;

namespace System.Collections
{
    /// <summary>
    /// Provides extension methods for the <see cref="Dictionary{TKey,TValue}"/>
    /// </summary>
    public static class DictionaryExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Adds a key/value pair to the <see cref="Dictionary{TKey,TValue}" />
        ///     if the key does not already exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valueFactory">The function used to generate a value for the key</param>
        /// <returns>
        ///     The value for the key.  This will be either the existing value for the key if the
        ///     key is already in the dictionary, or the new value for the key as returned by valueFactory
        ///     if the key was not in the dictionary.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key" /> is a null reference
        ///     (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key" /> is a null reference
        ///     (Nothing in Visual Basic).
        /// </exception>
        /// <exception cref="T:System.OverflowException">
        ///     The dictionary contains too many
        ///     elements.
        /// </exception>
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue resultingValue;
            if (source.TryGetValue(key, out resultingValue))
            {
                return resultingValue;
            }

            resultingValue = valueFactory(key);
            source.Add(key, resultingValue);

            return resultingValue;
        }

        #endregion
    }
}