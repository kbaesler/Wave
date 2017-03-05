using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace System.Windows
{
    /// <summary>
    ///     Provides an implemetation of the <see cref="IEditableObject" /> interface by
    ///     preserving values for all public properties do not keep an eye on all fields, private properties, public properties
    ///     without set ancestor
    /// </summary>
    /// <seealso cref="System.Windows.Observable" />
    /// <seealso cref="System.ComponentModel.IEditableObject" />
    public abstract class EditableObservable : Observable, IEditableObject
    {
        #region Fields

        private Hashtable _Properties;

        #endregion

        #region IEditableObject Members

        /// <summary>
        ///     Begins an edit on an object.
        /// </summary>
        public virtual void BeginEdit()
        {
            PropertyInfo[] properties = (this.GetType()).GetProperties
                (BindingFlags.Public | BindingFlags.Instance);

            _Properties = new Hashtable(properties.Length - 1);

            for (int i = 0; i < properties.Length; i++)
            {
                if (null != properties[i].GetSetMethod())
                {
                    object value = properties[i].GetValue(this, null);
                    _Properties.Add(properties[i].Name, value);
                }
            }
        }

        /// <summary>
        ///     Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> call.
        /// </summary>
        public virtual void CancelEdit()
        {
            if (_Properties == null) return;

            PropertyInfo[] properties = (this.GetType()).GetProperties
                (BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < properties.Length; i++)
            {
                if (null != properties[i].GetSetMethod())
                {
                    object value = _Properties[properties[i].Name];
                    properties[i].SetValue(this, value, null);
                }
            }

            _Properties = null;
        }

        /// <summary>
        ///     Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> or
        ///     <see cref="M:System.ComponentModel.IBindingList.AddNew" /> call into the underlying object.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void EndEdit()
        {
            _Properties = null;
        }

        #endregion
    }
}