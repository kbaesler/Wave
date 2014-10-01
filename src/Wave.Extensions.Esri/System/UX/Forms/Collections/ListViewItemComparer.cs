using System.Collections;
using System.Windows.Forms;

namespace System.Forms.Collections
{
    /// <summary>
    ///     A comparer that is used to compare the columns of a list view control.
    /// </summary>
    public class ListViewItemComparer : IComparer
    {
        #region Fields

        /// <summary>
        ///     Case insensitive comparer object
        /// </summary>
        private readonly CaseInsensitiveComparer _CaseInsensitiveComparer;

        /// <summary>
        ///     Specifies the column to be sorted
        /// </summary>
        private readonly int _SortColumn;

        /// <summary>
        ///     Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private readonly SortOrder _SortOrder;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ListViewItemComparer" /> class.
        /// </summary>
        public ListViewItemComparer()
            : this(0, SortOrder.Ascending)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ListViewItemComparer" /> class.
        /// </summary>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortOrder">The sort order.</param>
        public ListViewItemComparer(int sortColumn, SortOrder sortOrder)
        {
            _SortColumn = sortColumn;
            _SortOrder = sortOrder;
            _CaseInsensitiveComparer = new CaseInsensitiveComparer();
        }

        #endregion

        #region IComparer Members

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
        public virtual int Compare(object x, object y)
        {
            // Cast the objects to be compared to ListViewItem objects
            ListViewItem listviewX = (ListViewItem) x;
            ListViewItem listviewY = (ListViewItem) y;

            // Compare the two items
            int compare = _CaseInsensitiveComparer.Compare(listviewX.SubItems[_SortColumn].Text, listviewY.SubItems[_SortColumn].Text);

            // Calculate correct return value based on object comparison
            if (_SortOrder == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compare;
            }

            if (_SortOrder == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compare);
            }

            // Return '0' to indicate they are equal
            return 0;
        }

        #endregion
    }
}