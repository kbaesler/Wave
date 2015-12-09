using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace System.Windows
{
    /// <summary>
    ///     A partial abstract data validation class used to maintain a dictionary of validation errors that can be used in
    ///     validating property values using the <see cref="IDataErrorInfo" /> interface.
    /// </summary>
    /// <remarks>Use the ValidatesOnDataErrors binding property in XAML to use <see cref="IDataErrorInfo" /> interface.</remarks>
    public abstract class DataValidation : IDataErrorInfo
    {
        #region Fields

        private readonly Dictionary<string, string> _ValidationErrors = new Dictionary<string, string>();

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        protected virtual bool HasErrors
        {
            get { return (_ValidationErrors.Count > 0); }
        }

        #endregion

        #region IDataErrorInfo Members

        /// <summary>
        ///     Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <value></value>
        /// <returns>
        ///     An error message indicating what is wrong with this object. The default is an empty string ("").
        /// </returns>
        [IgnoreDataMember]
        public string Error
        {
            get
            {
                if (_ValidationErrors.Count > 0)
                {
                    return string.Format(CultureInfo.CurrentCulture, "{0} data is invalid.", this.GetType().Name);
                }

                return string.Empty;
            }
        }

        /// <summary>
        ///     Gets an error message indicating what is wrong with the specified property name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>
        ///     An error message indicating what is wrong with this property. The default is an empty string ("").
        /// </returns>
        public string this[string columnName]
        {
            get
            {
                if (_ValidationErrors.ContainsKey(columnName))
                {
                    return _ValidationErrors[columnName];
                }

                return string.Empty;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Adds the validation error to the dictionary based on the property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="errorMessage">The error message.</param>
        protected void AddError(string propertyName, string errorMessage)
        {
            this.VerifyPropertyName(propertyName);

            if (!_ValidationErrors.ContainsKey(propertyName))
                _ValidationErrors.Add(propertyName, errorMessage);
            else
                _ValidationErrors[propertyName] = errorMessage;
        }

        /// <summary>
        ///     Removes the validation error from the dictionary based on the property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RemoveError(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            if (_ValidationErrors.ContainsKey(propertyName))
                _ValidationErrors.Remove(propertyName);
        }

        /// <summary>
        ///     Warns the developer if this object does not have
        ///     a public property with the specified name. This
        ///     method does not exist in a Release build.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        protected void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;
                Debug.Fail(msg);
            }
        }

        #endregion
    }
}