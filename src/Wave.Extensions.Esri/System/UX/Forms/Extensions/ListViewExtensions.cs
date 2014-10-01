using System.Collections;
using System.Drawing;
using System.Forms.Collections;
using System.Linq;
using System.Windows.Forms;

namespace System.Forms
{
    /// <summary>
    ///     Provides extension methods for the <see cref="System.Windows.Forms.ListView" /> control.
    /// </summary>
    public static class ListViewExtensions
    {
        #region Constants

        /// <summary>
        ///     A character representing an ascending arrow.
        /// </summary>
        private const char AscendingOrder = '\u25B2';

        /// <summary>
        ///     A character rperesenting a descending arrow.
        /// </summary>
        private const char DescendingOrder = '\u25BC';

        #endregion

        #region Public Methods

        /// <summary>
        ///     Automatically resizes the columns to the appropriate width to either contain all of the contents or size of the
        ///     header.
        /// </summary>
        /// <param name="source">The list view.</param>
        public static void AutoResizeAllColumns(this ListView source)
        {
            if (source.Columns.Count == 0)
            {
                source.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            else
            {
                using (Graphics graphics = source.CreateGraphics())
                {
                    foreach (ColumnHeader columnHeader in source.Columns)
                    {
                        SizeF columnHeaderTextSize = graphics.MeasureString(columnHeader.Text, source.Font);
                        bool contents = source.Items
                            .Cast<ListViewItem>()
                            .Select(x => graphics.MeasureString(x.SubItems[columnHeader.Index].Text, x.Font))
                            .Any(x => x.Width > columnHeaderTextSize.Width);

                        columnHeader.AutoResize(contents ? ColumnHeaderAutoResizeStyle.ColumnContent : ColumnHeaderAutoResizeStyle.HeaderSize);
                    }
                }
            }
        }

        /// <summary>
        ///     Sorts the items in the list on the specified <paramref name="columnIndex" />
        ///     and draws a sort direction arrow to the column header.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="sortOrder">The sort order.</param>
        public static void Sort(this ListView source, int columnIndex, SortOrder sortOrder)
        {
            if (source == null)
                return;

            source.Sort(columnIndex, new ListViewItemComparer(columnIndex, sortOrder));
        }

        /// <summary>
        ///     Sorts the items in the list on the specified <paramref name="columnIndex" />
        ///     and draws a sort direction arrow to the column header.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="comparer">The list item comparer used for the sorting.</param>
        public static void Sort(this ListView source, int columnIndex, IComparer comparer)
        {
            if (source == null)
                return;

            // Remove any existing direction arrows.
            if (columnIndex != -1)
            {
                source.Columns[columnIndex].Text = source.Columns[columnIndex].Text.TrimEnd(DescendingOrder, AscendingOrder);
                source.Columns[columnIndex].Text = source.Columns[columnIndex].Text.Trim();
            }

            // Set the arrow characters to show the sort order
            if (source.Sorting == SortOrder.Ascending)
            {
                // Set the sort column to the new column.
                source.Sorting = SortOrder.Descending;
                source.Columns[columnIndex].Text = string.Format("{0} {1}", source.Columns[columnIndex].Text, AscendingOrder);
            }
            else
            {
                source.Sorting = SortOrder.Ascending;
                source.Columns[columnIndex].Text = string.Format("{0} {1}", source.Columns[columnIndex].Text, DescendingOrder);
            }

            // Set the ListViewItemSorter property to a new ListViewItemComparer object.
            source.ListViewItemSorter = comparer;

            // Call the sort method to manually sort.
            source.Sort();

            // Resize the columns to fit the contents.
            source.AutoResizeAllColumns();
        }

        #endregion
    }
}