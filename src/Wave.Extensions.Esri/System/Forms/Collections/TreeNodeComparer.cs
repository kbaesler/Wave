using System.Collections;
using System.Windows.Forms;

namespace System.Forms.Collections
{
    /// <summary>
    ///     Compares the node values against each other based on the text.
    /// </summary>
    public class TreeNodeComparer : IComparer
    {
        #region IComparer Members

        /// <summary>
        ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        ///     Value Condition Less than zero x is less than y. Zero x equals y. Greater than zero x is greater than y.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        ///     Neither x nor y implements the <see cref="T:System.IComparable"></see>
        ///     interface.-or- x and y are of different types and neither one can handle comparisons with the other.
        /// </exception>
        public int Compare(object x, object y)
        {
            TreeNode a = x as TreeNode;
            TreeNode b = y as TreeNode;
            if (a == null || b == null)
                return 0;

            return string.Compare(a.Text, b.Text, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}