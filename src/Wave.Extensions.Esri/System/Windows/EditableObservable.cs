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
            PropertyInfo[] properties = this.GetType().GetProperties
                (BindingFlags.Public | BindingFlags.Instance);

            _Properties = new Hashtable(properties.Length - 1);

            foreach (PropertyInfo t in properties)
            {
                if (t.GetSetMethod() != null)
                {
                    _Properties.Add(t.Name, t.GetValue(this, null));
                }
            }
        }

        /// <summary>
        ///     Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> call.
        /// </summary>
        public virtual void CancelEdit()
        {
            if (_Properties == null) return;

            PropertyInfo[] properties = this.GetType().GetProperties
                (BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo t in properties)
            {
                if (t.GetSetMethod() != null)
                {
                    t.SetValue(this, _Properties[t.Name], null);
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