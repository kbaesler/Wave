using System.Collections;
using System.Collections.Generic;

namespace System.Dynamic
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public sealed class DynamicDictionary<TValue> : DynamicObject, IDictionary<string, TValue>
    {
        #region Fields

        private readonly IDictionary<string, TValue> _Dictionary;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicDictionary{TValue}" /> class.
        /// </summary>
        public DynamicDictionary()
            : this(new Dictionary<string, TValue>())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicDictionary{TValue}" /> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public DynamicDictionary(IDictionary<string, TValue> dictionary)
        {
            _Dictionary = dictionary;
        }

        #endregion

        #region IDictionary<string,TValue> Members

        /// <summary>
        ///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        public ICollection<string> Keys
        {
            get { return _Dictionary.Keys; }
        }

        /// <summary>
        ///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the
        ///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return _Dictionary.Values; }
        }

        /// <summary>
        ///     Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public TValue this[string key]
        {
            get { return _Dictionary[key]; }
            set { _Dictionary[key] = value; }
        }

        /// <summary>
        ///     Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count
        {
            get { return _Dictionary.Count; }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _Dictionary.IsReadOnly; }
        }

        /// <summary>
        ///     Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(string key, TValue value)
        {
            _Dictionary.Add(key, value);
        }

        /// <summary>
        ///     Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the
        ///     specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
        /// <returns>
        ///     true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise,
        ///     false.
        /// </returns>
        public bool ContainsKey(string key)
        {
            return _Dictionary.ContainsKey(key);
        }

        /// <summary>
        ///     Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        ///     true if the element is successfully removed; otherwise, false.  This method also returns false if
        ///     <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        public bool Remove(string key)
        {
            return _Dictionary.Remove(key);
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
        ///     with the specified key; otherwise, false.
        /// </returns>
        public bool TryGetValue(string key, out TValue value)
        {
            return _Dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        ///     Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public void Add(KeyValuePair<string, TValue> item)
        {
            _Dictionary.Add(item);
        }

        /// <summary>
        ///     Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            _Dictionary.Clear();
        }

        /// <summary>
        ///     Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        ///     true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />;
        ///     otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<string, TValue> item)
        {
            return _Dictionary.Contains(item);
        }

        /// <summary>
        ///     Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            _Dictionary.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        ///     true if <paramref name="item" /> was successfully removed from the
        ///     <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if
        ///     <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public bool Remove(KeyValuePair<string, TValue> item)
        {
            return _Dictionary.Remove(item);
        }

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return _Dictionary.GetEnumerator();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Provides the implementation for operations that delete an object member. This method is not intended for use in C#
        ///     or Visual Basic.
        /// </summary>
        /// <param name="binder">Provides information about the deletion.</param>
        /// <returns>
        ///     true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the
        ///     language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            string name = binder.Name;

            if (!_Dictionary.ContainsKey(name))
                return false;

            _Dictionary.Remove(name);

            return true;
        }

        /// <summary>
        ///     Provides the implementation for operations that get member values. Classes derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for
        ///     operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">
        ///     Provides information about the object that called the dynamic operation. The binder.Name property
        ///     provides the name of the member on which the dynamic operation is performed. For example, for the
        ///     Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived
        ///     from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The
        ///     binder.IgnoreCase property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="result">
        ///     The result of the get operation. For example, if the method is called for a property, you can
        ///     assign the property value to <paramref name="result" />.
        /// </param>
        /// <returns>
        ///     true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the
        ///     language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name;
            if (_Dictionary.ContainsKey(name))
            {
                result = _Dictionary[name];
                return true;
            }

            bool member = base.TryGetMember(binder, out result);
            if (!member)
                throw new KeyNotFoundException(string.Format("Key \"{0}\" was not found in the given dictionary", name));

            return true;
        }

        /// <summary>
        ///     Provides the implementation for operations that set member values. Classes derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for
        ///     operations such as setting a value for a property.
        /// </summary>
        /// <param name="binder">
        ///     Provides information about the object that called the dynamic operation. The binder.Name property
        ///     provides the name of the member to which the value is being assigned. For example, for the statement
        ///     sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the
        ///     <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty". The binder.IgnoreCase
        ///     property specifies whether the member name is case-sensitive.
        /// </param>
        /// <param name="value">
        ///     The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where
        ///     sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the
        ///     <paramref name="value" /> is "Test".
        /// </param>
        /// <returns>
        ///     true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the
        ///     language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     This dictionary instance is read-only, you cannot modify the data it
        ///     contains
        /// </exception>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (IsReadOnly)
                throw new InvalidOperationException("This dictionary instance is read-only, you cannot modify the data it contains");

            string name = binder.Name;

            if (_Dictionary.ContainsKey(name))
            {
                _Dictionary[name] = (TValue) value;
                return true;
            }

            if (!base.TrySetMember(binder, value))
                _Dictionary.Add(name, (TValue) value);
            return true;
        }

        #endregion
    }
}