using System.Collections.Generic;

namespace System.Configuration
{
    /// <summary>
    ///     Provides the properties and methods for a collection of <see cref="System.Configuration.ConfigurationElement" />
    ///     objects.
    /// </summary>
    /// <typeparam name="TElement">The type of the configuration element.</typeparam>
    public interface IKeyedElementCollection<in TElement>
        where TElement : ConfigurationElement
    {
        #region Public Properties

        /// <summary>
        ///     Gets all of the keys in the collection.
        /// </summary>
        /// <value>All keys.</value>
        IEnumerable<string> AllKeys { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Adds the specified <paramref name="element" /> to the collection.
        /// </summary>
        /// <param name="element">The element </param>
        void Add(TElement element);

        /// <summary>
        ///     Adds the specified the <paramref name="element" /> to the collection at the specified <paramref name="index" />.
        /// </summary>
        /// <param name="index">The index </param>
        /// <param name="element">The element </param>
        void Add(int index, TElement element);

        /// <summary>
        ///     Clears the collection.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Determines if the collection contains the specified <paramref name="key" />.
        /// </summary>
        /// <param name="key">The key in question</param>
        /// <returns>True if exists; otherwise falSE.</returns>
        bool ContainsKey(string key);

        /// <summary>
        ///     Removes the element with the specified <paramref name="key" /> from the collection.
        /// </summary>
        /// <param name="key">The string key of element to remove</param>
        void Remove(string key);

        #endregion
    }
}