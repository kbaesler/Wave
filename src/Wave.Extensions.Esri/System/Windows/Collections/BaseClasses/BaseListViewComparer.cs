using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Collections
{
    /// <summary>
    ///     Represents an abstract list view comparer.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class BaseListViewComparer<T> : IListViewComparer
        where T : class
    {
        #region IListViewComparer Members

        /// <summary>
        ///     Gets or sets the sort direction.
        /// </summary>
        /// <value>
        ///     The sort direction.
        /// </value>
        public ListSortDirection Direction { get; set; }

        /// <summary>
        ///     Gets or sets the sort by column name.
        /// </summary>
        /// <value>
        ///     The sort by.
        /// </value>
        public string PropertyName { get; set; }

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        ///     Value
        ///     Condition
        ///     Less than zero
        ///     <paramref name="x" /> is less than <paramref name="y" />.
        ///     Zero
        ///     <paramref name="x" /> equals <paramref name="y" />.
        ///     Greater than zero
        ///     <paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     Neither <paramref name="x" /> nor <paramref name="y" /> implements the <see cref="T:System.IComparable" />
        ///     interface.
        ///     -or-
        ///     <paramref name="x" /> and <paramref name="y" /> are of different types and neither one can handle comparisons with
        ///     the other.
        /// </exception>
        public int Compare(object x, object y)
        {
            int compare = this.Compare(this.PropertyName, x as T, y as T);

            if (this.Direction == ListSortDirection.Descending)
                compare = compare*-1;

            return compare;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        ///     Value
        ///     Condition
        ///     Less than zero
        ///     <paramref name="x" /> is less than <paramref name="y" />.
        ///     Zero
        ///     <paramref name="x" /> equals <paramref name="y" />.
        ///     Greater than zero
        ///     <paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     Neither <paramref name="x" /> nor <paramref name="y" /> implements the <see cref="T:System.IComparable" />
        ///     interface.
        ///     -or-
        ///     <paramref name="x" /> and <paramref name="y" /> are of different types and neither one can handle comparisons with
        ///     the other.
        /// </exception>
        protected abstract int Compare(string propertyName, T x, T y);

        #endregion
    }
}