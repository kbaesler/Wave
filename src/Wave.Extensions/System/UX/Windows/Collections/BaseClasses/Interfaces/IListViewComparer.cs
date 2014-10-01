using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Collections
{
    /// <summary>
    ///     A comparison interface used to sort the contents of a ListView.
    /// </summary>
    [ComVisible(false)]
    public interface IListViewComparer : IComparer
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the sort direction.
        /// </summary>
        /// <value>The sort direction.</value>
        ListSortDirection Direction { get; set; }

        /// <summary>
        ///     Gets or sets the sort by column name.
        /// </summary>
        /// <value>The sort by.</value>
        string PropertyName { get; set; }

        #endregion
    }
}