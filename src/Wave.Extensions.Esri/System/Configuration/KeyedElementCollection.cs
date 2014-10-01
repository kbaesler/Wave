using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace System.Configuration
{
    /// <summary>
    ///     A collection of <see cref="System.Configuration.KeyedElement" /> configuration elements within a configuration
    ///     file.
    /// </summary>
    /// <typeparam name="TElement">The type of the configuration element.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    [Serializable]
    public class KeyedElementCollection<TElement> : ConfigurationElementCollection
        where TElement : KeyedElement, IKeyedElementCollection<TElement>, new()
    {
        #region Public Properties

        /// <summary>
        ///     Gets all of the keys in the collection.
        /// </summary>
        /// <value>All keys.</value>
        public IEnumerable<string> AllKeys
        {
            get
            {
                var keys = BaseGetAllKeys();
                foreach (var o in keys)
                    yield return o.ToString();
            }
        }

        /// <summary>
        ///     Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        ///     The specified property, attribute, or child element
        /// </returns>
        public virtual TElement this[int index]
        {
            get { return (TElement) BaseGet(index); }

            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                this.Add(index, value);
            }
        }

        /// <summary>
        ///     Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     The specified property, attribute, or child element
        /// </returns>
        public new virtual TElement this[string key]
        {
            get { return (TElement) BaseGet(key); }
            set
            {
                if (BaseGet(key) != null)
                    BaseRemove(key);

                this.Add(value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Adds the specified <paramref name="element" /> to the collection.
        /// </summary>
        /// <param name="element">The element </param>
        public virtual void Add(TElement element)
        {
            BaseAdd(element, true);
        }

        /// <summary>
        ///     Adds the specified the <paramref name="element" /> to the collection at the specified <paramref name="index" />.
        /// </summary>
        /// <param name="index">The index </param>
        /// <param name="element">The element </param>
        public virtual void Add(int index, TElement element)
        {
            base.BaseAdd(index, element);
        }

        /// <summary>
        ///     Clears the collection.
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }

        /// <summary>
        ///     Determines if the collection contains the specified <paramref name="key" />.
        /// </summary>
        /// <param name="key">The key in question</param>
        /// <returns>True if exists; otherwise falSE.</returns>
        public bool ContainsKey(string key)
        {
            return this.AllKeys.Contains(key);
        }

        /// <summary>
        ///     Removes the element with the specified <paramref name="key" /> from the collection.
        /// </summary>
        /// <param name="key">The string key of element to remove</param>
        public virtual void Remove(string key)
        {
            if (!BaseIsRemoved(key))
                BaseRemove(key);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        /// </summary>
        /// <returns>
        ///     A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TElement();
        }

        /// <summary>
        ///     Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"></see> to return the key for.</param>
        /// <returns>
        ///     An <see cref="T:System.Object"></see> that acts as the key for the specified
        ///     <see cref="T:System.Configuration.ConfigurationElement"></see>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TElement) element).ElementKey;
        }

        #endregion
    }
}