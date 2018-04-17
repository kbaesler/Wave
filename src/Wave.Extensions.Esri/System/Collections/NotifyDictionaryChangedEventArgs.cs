using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;

namespace System.Collections
{
    /// <summary>
    ///     The event that hold the event data when the dictionary has changed.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class NotifyDictionaryChangedEventArgs<TKey, TValue> : EventArgs
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotifyDictionaryChangedEventArgs&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        /// <param name="newItem">The new item.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="action">The action.</param>
        public NotifyDictionaryChangedEventArgs(KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem, NotifyCollectionChangedAction action)
        {
            this.Action = action;
            this.NewItems = new Dictionary<TKey, TValue>(1);
            this.NewItems.Add(newItem);
            this.OldItems = new Dictionary<TKey, TValue>(1);
            this.OldItems.Add(oldItem);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotifyDictionaryChangedEventArgs&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="action">The action.</param>
        public NotifyDictionaryChangedEventArgs(IDictionary<TKey, TValue> items, NotifyCollectionChangedAction action)
        {
            this.Action = action;
            if (action == NotifyCollectionChangedAction.Reset | action == NotifyCollectionChangedAction.Remove)
                this.OldItems = items;
            else
                this.NewItems = items;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotifyDictionaryChangedEventArgs&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="action">The action.</param>
        public NotifyDictionaryChangedEventArgs(KeyValuePair<TKey, TValue> item, NotifyCollectionChangedAction action)
            : this(item.Key, item.Value, action)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotifyDictionaryChangedEventArgs&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="action">The action.</param>
        public NotifyDictionaryChangedEventArgs(TKey key, TValue value, NotifyCollectionChangedAction action)
            : this(new Dictionary<TKey, TValue>(1), action)
        {
            var dictionary = action == NotifyCollectionChangedAction.Reset | action == NotifyCollectionChangedAction.Remove ? this.OldItems : this.NewItems;
            dictionary[key] = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotifyDictionaryChangedEventArgs&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        /// <param name="newItems">The new items.</param>
        /// <param name="oldItems">The old items.</param>
        /// <param name="action">The action.</param>
        protected NotifyDictionaryChangedEventArgs(IDictionary<TKey, TValue> newItems, IDictionary<TKey, TValue> oldItems, NotifyCollectionChangedAction action)
        {
            this.Action = action;
            this.OldItems = oldItems;
            this.NewItems = newItems;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the action.
        /// </summary>
        /// <value>The action.</value>
        public NotifyCollectionChangedAction Action { get; private set; }

        /// <summary>
        ///     Gets first new item.
        /// </summary>
        /// <value>The new item.</value>
        public KeyValuePair<TKey, TValue> NewItem
        {
            get
            {
                if (this.NewItems == null)
                    return new KeyValuePair<TKey, TValue>();
                return this.NewItems.ElementAtOrDefault(0);
            }
        }

        /// <summary>
        ///     Gets the new items.
        /// </summary>
        /// <value>The new items.</value>
        public IDictionary<TKey, TValue> NewItems { get; private set; }

        /// <summary>
        ///     Gets first old item.
        /// </summary>
        /// <value>The old item.</value>
        public KeyValuePair<TKey, TValue> OldItem
        {
            get
            {
                if (this.OldItems == null)
                    return new KeyValuePair<TKey, TValue>();
                return this.OldItems.ElementAtOrDefault(0);
            }
        }

        /// <summary>
        ///     Gets the old items.
        /// </summary>
        /// <value>The old items.</value>
        public IDictionary<TKey, TValue> OldItems { get; private set; }

        #endregion
    }
}