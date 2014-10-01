using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Collections
{
    /// <summary>
    ///     An observable collection that is unique based on the key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ObservableKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>, INotifyCollectionChanged, INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Fields

        private readonly Func<TItem, TKey> _KeySelector;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableKeyedCollection&lt;TKey, TItem&gt;" /> class.
        /// </summary>
        /// <param name="keySelector">The key selector.</param>
        public ObservableKeyedCollection(Func<TItem, TKey> keySelector)
        {
            _KeySelector = keySelector;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableKeyedCollection&lt;TKey, TItem&gt;" /> class.
        /// </summary>
        protected ObservableKeyedCollection()
        {
        }

        #endregion

        #region INotifyCollectionChanged Members

        /// <summary>
        ///     Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region INotifyPropertyChanging Members

        /// <summary>
        ///     Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Removes all elements from the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" />.
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        ///     When implemented in a derived class, extracts the key from the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        /// <returns>
        ///     The key for the specified element.
        /// </returns>
        protected override TKey GetKeyForItem(TItem item)
        {
            return _KeySelector(item);
        }

        /// <summary>
        ///     Inserts an element into the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" /> at the specified
        ///     index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is less than 0.
        ///     -or-
        ///     <paramref name="index" /> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count" />.
        /// </exception>
        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        /// <summary>
        ///     Raises the <see cref="CollectionChanged" /> event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing
        ///     the event data.
        /// </param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler eventHandler = this.CollectionChanged;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Raises the <see cref="PropertyChanging" /> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanging(string propertyName)
        {
            PropertyChangingEventHandler eventHandler = this.PropertyChanging;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        ///     Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.KeyedCollection`2" />
        ///     .
        /// </summary>
        /// <param name="index">The index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            TItem item = this[index];
            base.RemoveItem(index);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        /// <summary>
        ///     Replaces the item at the specified index with the specified item.
        /// </summary>
        /// <param name="index">The zero-based index of the item to be replaced.</param>
        /// <param name="item">The new item.</param>
        protected override void SetItem(int index, TItem item)
        {
            base.SetItem(index, item);
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, index));
        }

        #endregion
    }
}