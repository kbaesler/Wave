using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace System.Collections
{
    /// <summary>
    ///     The event arguments that hold the data for when the dictionary is changing.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class NotifyDictionaryChangingEventArgs<TKey, TValue> : NotifyDictionaryChangedEventArgs<TKey, TValue>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotifyDictionaryChangingEventArgs&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        /// <param name="newItem">The new item.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="action">The action.</param>
        public NotifyDictionaryChangingEventArgs(KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem, NotifyCollectionChangedAction action)
            : base(newItem, oldItem, action)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotifyDictionaryChangingEventArgs&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        /// <param name="newItems">The new items.</param>
        /// <param name="oldItems">The old items.</param>
        /// <param name="action">The action.</param>
        public NotifyDictionaryChangingEventArgs(IDictionary<TKey, TValue> newItems, IDictionary<TKey, TValue> oldItems, NotifyCollectionChangedAction action)
            : base(newItems, oldItems, action)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotifyDictionaryChangingEventArgs&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="action">The action.</param>
        public NotifyDictionaryChangingEventArgs(IDictionary<TKey, TValue> items, NotifyCollectionChangedAction action)
            : base(items, action)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotifyDictionaryChangingEventArgs&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="action">The action.</param>
        public NotifyDictionaryChangingEventArgs(KeyValuePair<TKey, TValue> item, NotifyCollectionChangedAction action)
            : this(item.Key, item.Value, action)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotifyDictionaryChangingEventArgs&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="action">The action.</param>
        public NotifyDictionaryChangingEventArgs(TKey key, TValue value, NotifyCollectionChangedAction action)
            : base(key, value, action)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the event should be canceled.
        /// </summary>
        /// <returns>
        ///     true if the event should be canceled; otherwise, falSE.
        /// </returns>
        public bool Cancel { get; set; }

        #endregion
    }
}