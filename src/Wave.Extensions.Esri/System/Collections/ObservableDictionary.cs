using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace System.Collections
{
    /// <summary>
    ///     Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole
    ///     list is refreshed.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyDictionaryChanged<TKey, TValue>, INotifyDictionaryChanging<TKey, TValue>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableDictionary&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        public ObservableDictionary()
        {
            this.Dictionary = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableDictionary&lt;TKey, TValue&gt;" /> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.Dictionary = dictionary;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        protected IDictionary<TKey, TValue> Dictionary { get; private set; }

        #endregion

        #region IDictionary<TKey,TValue> Members

        /// <summary>
        ///     Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public int Count
        {
            get { return this.Dictionary.Count; }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, falSE.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        public ICollection<TKey> Keys
        {
            get { return this.Dictionary.Keys; }
        }

        /// <summary>
        ///     Gets or sets the value with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     Returns the value for the key.
        /// </returns>
        public TValue this[TKey key]
        {
            get { return this.Dictionary[key]; }
            set
            {
                var oldItem = new KeyValuePair<TKey, TValue>(key, this[key]);
                var newItem = new KeyValuePair<TKey, TValue>(key, value);

                NotifyDictionaryChangingEventArgs<TKey, TValue> e = new NotifyDictionaryChangingEventArgs<TKey, TValue>(newItem, oldItem, NotifyCollectionChangedAction.Replace);
                this.OnDictionaryChanging(e);
                if (e.Cancel)
                    return;

                this.Dictionary[key] = value;
                this.OnDictionaryChanged(new NotifyDictionaryChangedEventArgs<TKey, TValue>(newItem, oldItem, NotifyCollectionChangedAction.Replace));
            }
        }

        /// <summary>
        ///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        public ICollection<TValue> Values
        {
            get { return this.Dictionary.Values; }
        }

        /// <summary>
        ///     Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.
        /// </exception>
        public void Add(TKey key, TValue value)
        {
            NotifyDictionaryChangingEventArgs<TKey, TValue> e = new NotifyDictionaryChangingEventArgs<TKey, TValue>(key, value, NotifyCollectionChangedAction.Add);
            this.OnDictionaryChanging(e);
            if (e.Cancel)
                return;

            this.Dictionary.Add(key, value);
            this.OnDictionaryChanged(new NotifyDictionaryChangedEventArgs<TKey, TValue>(key, value, NotifyCollectionChangedAction.Add));
        }

        /// <summary>
        ///     Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        /// <summary>
        ///     Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public void Clear()
        {
            NotifyDictionaryChangingEventArgs<TKey, TValue> e = new NotifyDictionaryChangingEventArgs<TKey, TValue>(this.Dictionary, NotifyCollectionChangedAction.Reset);
            this.OnDictionaryChanging(e);
            if (e.Cancel)
                return;

            this.Dictionary.Clear();
            this.OnDictionaryChanged(new NotifyDictionaryChangedEventArgs<TKey, TValue>(Dictionary, NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        ///     Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        ///     true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />;
        ///     otherwise, falSE.
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.Dictionary.Contains(item);
        }

        /// <summary>
        ///     Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the
        ///     specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise,
        ///     falSE.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key" /> is null.
        /// </exception>
        public bool ContainsKey(TKey key)
        {
            return this.Dictionary.ContainsKey(key);
        }

        /// <summary>
        ///     Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an
        ///     <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied
        ///     from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have
        ///     zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="array" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="arrayIndex" /> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="array" /> is multidimensional.
        ///     -or-
        ///     <paramref name="arrayIndex" /> is equal to or greater than the length of <paramref name="array" />.
        ///     -or-
        ///     The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the
        ///     available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.Dictionary.GetEnumerator();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary) this.Dictionary).GetEnumerator();
        }

        /// <summary>
        ///     Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        ///     true if the element is successfully removed; otherwise, falSE.  This method also returns false if
        ///     <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key" /> is null.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.
        /// </exception>
        public bool Remove(TKey key)
        {
            if (!this.ContainsKey(key)) return false;

            TValue value = this[key];
            NotifyDictionaryChangingEventArgs<TKey, TValue> e = new NotifyDictionaryChangingEventArgs<TKey, TValue>(key, value, NotifyCollectionChangedAction.Add);
            this.OnDictionaryChanging(e);
            if (e.Cancel) return false;

            this.Dictionary.Remove(key);
            this.OnDictionaryChanged(new NotifyDictionaryChangedEventArgs<TKey, TValue>(key, value, NotifyCollectionChangedAction.Remove));

            return true;
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        ///     true if <paramref name="item" /> was successfully removed from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, falSE. This method also returns false if
        ///     <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        ///     The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">
        ///     When this method returns, the value associated with the specified key, if the key is found;
        ///     otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed
        ///     uninitialized.
        /// </param>
        /// <returns>
        ///     true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element
        ///     with the specified key; otherwise, falSE.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key" /> is null.
        /// </exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.Dictionary.TryGetValue(key, out value);
        }

        #endregion

        #region INotifyDictionaryChanged<TKey,TValue> Members

        /// <summary>
        ///     Occurs when the dictionary has changed.
        /// </summary>
        public event EventHandler<NotifyDictionaryChangedEventArgs<TKey, TValue>> DictionaryChanged;

        #endregion

        #region INotifyDictionaryChanging<TKey,TValue> Members

        /// <summary>
        ///     Occurs when the dictionary is changing.
        /// </summary>
        public event EventHandler<NotifyDictionaryChangingEventArgs<TKey, TValue>> DictionaryChanging;

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Raises the <see cref="DictionaryChanged" /> event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="System.Collections.NotifyDictionaryChangedEventArgs&lt;TKey,TValue&gt;" /> instance
        ///     containing the event data.
        /// </param>
        protected virtual void OnDictionaryChanged(NotifyDictionaryChangedEventArgs<TKey, TValue> e)
        {
            EventHandler<NotifyDictionaryChangedEventArgs<TKey, TValue>> eventHandler = this.DictionaryChanged;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="DictionaryChanging" /> event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="System.Collections.NotifyDictionaryChangingEventArgs&lt;TKey,TValue&gt;" /> instance
        ///     containing the event data.
        /// </param>
        protected virtual void OnDictionaryChanging(NotifyDictionaryChangingEventArgs<TKey, TValue> e)
        {
            EventHandler<NotifyDictionaryChangingEventArgs<TKey, TValue>> eventHandler = this.DictionaryChanging;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        #endregion
    }
}