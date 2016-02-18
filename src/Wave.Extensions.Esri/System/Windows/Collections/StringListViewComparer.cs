using System.Reflection.Internal;
using System.Runtime.InteropServices;

namespace System.Windows.Collections
{
    /// <summary>
    ///     A string comparer that compares two specified System.String objects and returns an integer that indicates their
    ///     relationship to one another in the sort order.
    /// </summary>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class StringListViewComparer : BaseListViewComparer<object>
    {
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
        protected override int Compare(string propertyName, object x, object y)
        {
            object xo = PropertyBinding.GetValue(x, propertyName);
            object yo = PropertyBinding.GetValue(y, propertyName);

            if (xo == null && yo == null)
                return 0;

            if (xo == null)
                return -1;

            if (yo == null)
                return 1;

            return string.Compare(xo.ToString(), yo.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}