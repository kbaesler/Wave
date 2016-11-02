using System.Collections.Generic;

namespace System.Dynamic
{
    /// <summary>
    ///     Provides a dynamic version of the <see cref="KeyValuePair{TKey,TValue}" /> struct.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <seealso cref="System.Dynamic.DynamicObject" />
    public sealed class DynamicKeyValuePair<TKey, TValue> : DynamicObject
    {
        #region Fields

        private KeyValuePair<TKey, TValue> _KeyValuePair;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicKeyValuePair{TKey, TValue}" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        internal DynamicKeyValuePair(KeyValuePair<TKey, TValue> item)
        {
            _KeyValuePair = item;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the key.
        /// </summary>
        /// <value>
        ///     The key.
        /// </value>
        public TKey Key
        {
            get { return _KeyValuePair.Key; }
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public TValue Value
        {
            get { return _KeyValuePair.Value; }
        }

        #endregion

        #region Public Methods

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
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_KeyValuePair.Key.ToString() != binder.Name)
                return base.TryGetMember(binder, out result);

            result = _KeyValuePair.Value;

            return true;
        }

        #endregion
    }
}