using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Miner.Interop.Process.Collections
{
    /// <summary>
    ///     A wrapper around the <see cref="Miner.Interop.Process.IDictionary" /> interface that uses reference types instead
    ///     of value types.
    /// </summary>
    [ComVisible(false),
     ClassInterface(ClassInterfaceType.None)]
    public sealed class ReferenceDictionary : Dictionary<string, object>, IDictionary
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReferenceDictionary" /> class.
        /// </summary>
        public ReferenceDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDictionary" /> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <exception cref="System.ArgumentNullException">dictionary</exception>
        public ReferenceDictionary(IDictionary dictionary)
        {
            if(dictionary == null) throw new ArgumentNullException("dictionary");

            object[] keys = (object[]) dictionary.Keys();
            foreach (object k in keys)
            {
                object key = k;
                this.Add((string) key, dictionary.get_Item(ref key));
            }
        }

        #endregion

        #region IDictionary Members

        /// <summary>
        ///     Gets or sets the compare mode.
        /// </summary>
        /// <value>The compare mode.</value>
        CompareMethod IDictionary.CompareMode { get; set; }

        /// <summary>
        ///     Gets the number of key/value pairs contained in the <see cref="T:System.Collections.Generic.Dictionary`2" />.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     The number of key/value pairs contained in the <see cref="T:System.Collections.Generic.Dictionary`2" />.
        /// </returns>
        int IDictionary.Count
        {
            get { return base.Count; }
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="System.ArgumentNullException">key</exception>
        void IDictionary.Add(ref object key, ref object item)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            base.Add(key.ToString(), item);
        }

        /// <summary>
        /// Existses the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">key</exception>
        bool IDictionary.Exists(ref object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return base.ContainsKey(key.ToString());
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IDictionary.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets the hash val.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">key</exception>
        object IDictionary.get_HashVal(ref object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return key.GetHashCode();
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">key</exception>
        object IDictionary.get_Item(ref object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return base[key.ToString()];
        }

        /// <summary>
        ///     Gets the values
        /// </summary>
        /// <returns></returns>
        object IDictionary.Items()
        {
            return base.Values;
        }

        /// <summary>
        ///     Keyses this instance.
        /// </summary>
        /// <returns></returns>
        object IDictionary.Keys()
        {
            return base.Keys;
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="System.ArgumentNullException">key</exception>
        void IDictionary.let_Item(ref object key, ref object item)
        {
            if(key == null)
                throw new ArgumentNullException("key");

            base[key.ToString()] = item;
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="System.ArgumentNullException">key</exception>
        void IDictionary.Remove(ref object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (base.ContainsKey(key.ToString()))
                base.Remove(key.ToString());
        }

        /// <summary>
        ///     Removes all.
        /// </summary>
        void IDictionary.RemoveAll()
        {
            base.Clear();
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="System.ArgumentNullException">key</exception>
        void IDictionary.set_Item(ref object key, ref object item)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            base[key.ToString()] = item;
        }

        /// <summary>
        /// Sets the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="System.ArgumentNullException">key</exception>
        void IDictionary.set_Key(ref object key, ref object item)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            base[key.ToString()] = item;
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this).GetEnumerator();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new instance of the <see cref="ReferenceDictionary" /> using the specified <see cref="IDictionary" />
        /// object.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>
        /// A new instance of the <see cref="ReferenceDictionary" /> containing the information from the specified
        /// dictionary.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">dictionary</exception>
        public static ReferenceDictionary Create(IDictionary dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            ReferenceDictionary reference = new ReferenceDictionary();
            object[] keys = (object[]) dictionary.Keys();
            foreach (object k in keys)
            {
                object key = k;
                reference.Add((string) key, dictionary.get_Item(ref key));
            }

            return reference;
        }

        #endregion
    }
}