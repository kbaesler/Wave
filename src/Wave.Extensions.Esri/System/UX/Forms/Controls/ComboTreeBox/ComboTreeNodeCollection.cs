using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace System.Forms.Controls
{
    /// <summary>
    ///     Represents a collection of ComboTreeNode objects contained within a node or a ComboTreeBox control.
    ///     Supports change notification through INotifyCollectionChanged. Implements the non-generic IList to
    ///     provide design-time support.
    /// </summary>
    /// <remarks>http://www.brad-smith.info/blog/archives/193</remarks>
    public sealed class ComboTreeNodeCollection : IList<ComboTreeNode>, IList, INotifyCollectionChanged
    {
        #region Fields

        private readonly List<ComboTreeNode> _InnerList;
        private readonly ComboTreeNode _Node;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initalises a new instance of ComboTreeNodeCollection and associates it with the specified ComboTreeNode.
        /// </summary>
        /// <param name="node"></param>
        public ComboTreeNodeCollection(ComboTreeNode node)
        {
            _InnerList = new List<ComboTreeNode>();
            _Node = node;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the node with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ComboTreeNode this[string name]
        {
            get
            {
                foreach (ComboTreeNode o in this)
                {
                    if (Equals(o.Name, name)) return o;
                }

                throw new KeyNotFoundException();
            }
        }

        #endregion

        #region IList Members

        /// <summary>
        ///     Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized
        ///     (thread safe).
        /// </summary>
        /// <returns>
        ///     true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise,
        ///     falSE.
        /// </returns>
        bool ICollection.IsSynchronized
        {
            get { return ((ICollection) _InnerList).IsSynchronized; }
        }

        /// <summary>
        ///     Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
        /// </summary>
        /// <returns>
        ///     An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
        /// </returns>
        object ICollection.SyncRoot
        {
            get { return ((ICollection) _InnerList).SyncRoot; }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.IList" /> has a fixed size; otherwise, falSE.
        /// </returns>
        bool IList.IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, falSE.
        /// </returns>
        bool IList.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        ///     The element at the specified index.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///     <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        ///     The property is set and the <see cref="T:System.Collections.Generic.IList`1" /> is read-only.
        /// </exception>
        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (ComboTreeNode) value; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection) _InnerList).CopyTo(array, index);
        }

        int IList.Add(object value)
        {
            Add((ComboTreeNode) value);
            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            return Contains((ComboTreeNode) value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((ComboTreeNode) value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (ComboTreeNode) value);
        }

        void IList.Remove(object value)
        {
            Remove((ComboTreeNode) value);
        }

        #endregion

        #region IList<ComboTreeNode> Members

        /// <summary>
        ///     Gets the number of nodes in the collection.
        /// </summary>
        public int Count
        {
            get { return _InnerList.Count; }
        }

        /// <summary>
        ///     Gets or sets the node at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ComboTreeNode this[int index]
        {
            get { return _InnerList[index]; }
            set
            {
                ComboTreeNode oldItem = _InnerList[index];
                _InnerList[index] = value;
                value.Parent = _Node;
                value.Nodes.CollectionChanged += CollectionChanged;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldItem));
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, falSE.
        /// </returns>
        bool ICollection<ComboTreeNode>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Adds a node to the collection.
        /// </summary>
        /// <param name="item"></param>
        public void Add(ComboTreeNode item)
        {
            _InnerList.Add(item);
            item.Parent = _Node;
            item.Nodes.CollectionChanged += CollectionChanged;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <summary>
        ///     Clears the collection.
        /// </summary>
        public void Clear()
        {
            foreach (ComboTreeNode item in _InnerList) item.Nodes.CollectionChanged -= CollectionChanged;
            _InnerList.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        ///     Determines whether the collection contains the specified node.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ComboTreeNode item)
        {
            return _InnerList.Contains(item);
        }

        /// <summary>
        ///     Copies all the nodes from the collection to a compatible array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ComboTreeNode[] array, int arrayIndex)
        {
            _InnerList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///     Returns an enumerator which can be used to cycle through the nodes in the collection (non-recursive).
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ComboTreeNode> GetEnumerator()
        {
            return _InnerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _InnerList.GetEnumerator();
        }

        /// <summary>
        ///     Returns the index of the specified node.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(ComboTreeNode item)
        {
            return _InnerList.IndexOf(item);
        }

        /// <summary>
        ///     Inserts a node into the collection at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, ComboTreeNode item)
        {
            _InnerList.Insert(index, item);
            item.Parent = _Node;
            item.Nodes.CollectionChanged += CollectionChanged;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <summary>
        ///     Removes the specified node from the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ComboTreeNode item)
        {
            if (_InnerList.Remove(item))
            {
                item.Nodes.CollectionChanged -= CollectionChanged;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Removes the node at the specified index from the collection.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            ComboTreeNode item = _InnerList[index];
            item.Nodes.CollectionChanged -= CollectionChanged;
            _InnerList.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        #endregion

        #region INotifyCollectionChanged Members

        /// <summary>
        ///     Fired when the collection (sub-tree) changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a node and adds it to the collection.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public ComboTreeNode Add(string text)
        {
            var item = new ComboTreeNode(text);
            Add(item);
            return item;
        }

        /// <summary>
        ///     Creates a node and adds it to the collection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public ComboTreeNode Add(string name, string text)
        {
            var item = new ComboTreeNode(name, text);
            Add(item);
            return item;
        }

        /// <summary>
        ///     Adds a range of ComboTreeNode to the collection.
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<ComboTreeNode> items)
        {
            foreach (ComboTreeNode item in items)
            {
                _InnerList.Add(item);
                item.Parent = _Node;
                item.Nodes.CollectionChanged += CollectionChanged;
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        ///     Determines whether the collection contains a node with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsKey(string name)
        {
            foreach (ComboTreeNode o in this)
            {
                if (Equals(o.Name, name)) return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns the index of the node with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int IndexOf(string name)
        {
            for (int i = 0; i < _InnerList.Count; i++)
            {
                if (Equals(_InnerList[i].Name, name)) return i;
            }

            return -1;
        }

        /// <summary>
        ///     Removes the node with the specified name from the collection.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Remove(string name)
        {
            for (int i = 0; i < _InnerList.Count; i++)
            {
                if (Equals(_InnerList[i].Name, name))
                {
                    ComboTreeNode item = _InnerList[i];
                    item.Nodes.CollectionChanged -= CollectionChanged;
                    _InnerList.RemoveAt(i);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Sorts the collection and its entire sub-tree using the specified comparer.
        /// </summary>
        /// <param name="comparer"></param>
        internal void Sort(IComparer<ComboTreeNode> comparer)
        {
            if (comparer == null) comparer = Comparer<ComboTreeNode>.Default;
            SortInternal(comparer);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Raises the CollectionChanged event.
        /// </summary>
        /// <param name="e"></param>
        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler eventHandler = CollectionChanged;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        /// <summary>
        ///     Recursive helper method for Sort(IComparer&lt;ComboTreeNode&gt;).
        /// </summary>
        /// <param name="comparer"></param>
        private void SortInternal(IComparer<ComboTreeNode> comparer)
        {
            _InnerList.Sort(comparer);
            foreach (ComboTreeNode node in _InnerList)
            {
                node.Nodes.Sort(comparer);
            }
        }

        #endregion
    }
}