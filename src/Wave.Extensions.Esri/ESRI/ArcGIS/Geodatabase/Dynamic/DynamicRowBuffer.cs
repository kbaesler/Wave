using System;
using System.Collections.Generic;
using System.Dynamic;

namespace ESRI.ArcGIS.Geodatabase
{
    /// <summary>
    ///     Provides a dynamic field and value behavior at run-time.
    /// </summary>
    /// <seealso cref="System.Dynamic.DynamicObject" />
    public sealed class DynamicRowBuffer : DynamicDictionary<object>
    {
        #region Fields

        private readonly IRowBuffer _Buffer;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicRowBuffer" /> class.
        /// </summary>
        /// <param name="buffer">The object buffer.</param>
        public DynamicRowBuffer(IRowBuffer buffer)
            : base(buffer.ToDictionary())
        {
            _Buffer = buffer;
        }

        #endregion

        #region Public Methods

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
            var name = binder.Name;
            if (!this.ContainsKey(name))
                throw new KeyNotFoundException(string.Format("Field \"{0}\" was not found in the given row", name));

            var i = _Buffer.Fields.FindField(name);
            if (!_Buffer.Fields.Field[i].Editable)
                throw new InvalidOperationException("This field is read-only, you cannot modify the data it contains");

            _Buffer.Value[i] = value;

            return base.TrySetMember(binder, value);
        }

        #endregion
    }
}