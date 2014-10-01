using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection.Internal;
using System.Windows.Collections;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace System.Windows.Behaviors
{
    /// <summary>
    ///     Additional attached behaviors for the <see cref="ListView" /> control.
    /// </summary>
    public static class ListViewBehavior
    {
        #region Fields

        /// <summary>
        ///     A dependency property that indiciates if the columns will be sorted when their headers are clicked.
        /// </summary>
        public static readonly DependencyProperty IsSortedWithColumnHeaderProperty =
            DependencyProperty.RegisterAttached("IsSortedWithColumnHeader", typeof (bool), typeof (ListViewBehavior),
                new FrameworkPropertyMetadata(false,
                    OnIsSortedWithColumnHeaderChanged));

        /// <summary>
        ///     A dependency property for the property name on the column that is used by the sorting.
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.RegisterAttached("PropertyName", typeof (string), typeof (ListViewBehavior));

        /// <summary>
        ///     A dependency property for the comparer that is used for the sorting of the columns.
        /// </summary>
        public static readonly DependencyProperty SortComparerProperty =
            DependencyProperty.RegisterAttached("SortComparer", typeof (IListViewComparer), typeof (ListViewBehavior),
                new PropertyMetadata(new StringListViewComparer()));

        /// <summary>
        ///     The last column clicked.
        /// </summary>
        private static GridViewColumnHeader SortedColumnHeader;

        /// <summary>
        ///     The last sorted direction.
        /// </summary>
        private static ListSortDirection SortedDirection;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a <see cref="DataTable" /> containing the data within the specified <see cref="ListView" />.
        /// </summary>
        /// <param name="listView">The list view.</param>
        /// <param name="eventHandler">The event handler.</param>
        /// <returns>
        ///     A <see cref="DataTable" /> of the contents of the list view.
        /// </returns>
        public static DataTable AsDataTable(this ListView listView, ProgressChangedEventHandler eventHandler)
        {
            DataTable table = new DataTable();
            table.Locale = CultureInfo.InvariantCulture;

            if (listView == null)
                return null;

            GridView gridView = listView.View as GridView;
            if (gridView == null)
                return null;

            // Create the binding dictionary.
            Dictionary<DataColumn, string> bindings = new Dictionary<DataColumn, string>();
            foreach (var column in gridView.Columns)
            {
                string propertyName;
                string columnName;

                // Determine the property for the column binding.
                Binding binding = column.DisplayMemberBinding as Binding;
                if (binding == null || binding.Path == null)
                {
                    propertyName = GetPropertyName(column);
                }
                else
                {
                    propertyName = binding.Path.Path;
                }

                if (string.IsNullOrEmpty(propertyName))
                    continue;

                if (column.Header == null)
                    columnName = propertyName;
                else
                    columnName = string.Format(CultureInfo.InvariantCulture, "{0}", column.Header);

                // Create the new binding.
                DataColumn header = table.Columns.Add(columnName);
                bindings.Add(header, propertyName);
            }

            int count = 0;

            // Iterate through all of the items.
            foreach (var item in listView.Items)
            {
                // Increment the counter.
                count++;

                // Create the new row and update the values.
                DataRow row = table.NewRow();

                // Use reflection to determine which columns get which values.
                foreach (KeyValuePair<DataColumn, string> entry in bindings)
                {
                    object value = PropertyBinding.GetValue(item, entry.Value);
                    row[entry.Key] = value;
                }

                // Add the new row.
                table.Rows.Add(row);

                if (eventHandler != null)
                {
                    int progressPercentage = (int) (count/(float) listView.Items.Count*100);
                    eventHandler(listView, new ProgressChangedEventArgs(progressPercentage, row));
                }
            }

            return table;
        }

        /// <summary>
        ///     Gets the name of the property.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static string GetPropertyName(DependencyObject o)
        {
            return (string) o.GetValue(PropertyNameProperty);
        }

        /// <summary>
        ///     Gets the sort comparer.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        /// <value>
        ///     The sort comparer.
        /// </value>
        public static IListViewComparer GetSortComparer(DependencyObject o)
        {
            return (IListViewComparer) o.GetValue(SortComparerProperty);
        }

        /// <summary>
        ///     Sets the is sorted with column header.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="value">The value.</param>
        public static void SetIsSortedWithColumnHeader(DependencyObject o, bool value)
        {
            o.SetValue(IsSortedWithColumnHeaderProperty, value);
        }

        /// <summary>
        ///     Sets the name of the property.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="value">The value.</param>
        public static void SetPropertyName(DependencyObject o, string value)
        {
            o.SetValue(PropertyNameProperty, value);
        }

        /// <summary>
        ///     Sets the sort comparer.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="value">The value.</param>
        public static void SetSortComparer(DependencyObject o, IListViewComparer value)
        {
            o.SetValue(SortComparerProperty, value);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Handles the Click event of the ColumnHeader control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private static void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader columnHeader = e.OriginalSource as GridViewColumnHeader;
            if (columnHeader != null)
            {
                ListView listView = sender as ListView;
                if (listView != null)
                {
                    string propertyName;

                    // Determine the binding to use for the sorting.
                    Binding binding = columnHeader.Column.DisplayMemberBinding as Binding;
                    if (binding == null || binding.Path == null)
                    {
                        propertyName = GetPropertyName(columnHeader.Column);
                    }
                    else
                    {
                        propertyName = binding.Path.Path;
                    }

                    // Apply the sorting for the given property.
                    if (string.IsNullOrEmpty(propertyName)) return;

                    // Determine the direction of the sorting.
                    ListSortDirection direction = GetSortingDirection(columnHeader);

                    // The use of the comparer is much faster than using the sort descriptions.
                    IListViewComparer comparer = GetSortComparer(listView);
                    if (comparer != null)
                    {
                        // Update the properties.
                        comparer.PropertyName = propertyName;
                        comparer.Direction = direction;

                        // Apply the comparison.
                        ListCollectionView collectionView = (ListCollectionView) CollectionViewSource.GetDefaultView(listView.ItemsSource);
                        collectionView.CustomSort = comparer;
                    }
                    else if (!propertyName.Contains("."))
                    {
                        listView.Items.SortDescriptions.Clear();
                        listView.Items.SortDescriptions.Add(new SortDescription(propertyName, direction));
                    }

                    // Update the sort adorner.
                    UpdateAdorner(columnHeader, direction);
                }
            }
        }

        /// <summary>
        ///     Gets the sorting direction.
        /// </summary>
        /// <param name="columnHeader">The column header.</param>
        /// <returns></returns>
        private static ListSortDirection GetSortingDirection(GridViewColumnHeader columnHeader)
        {
            if (!Equals(columnHeader, SortedColumnHeader))
                return ListSortDirection.Ascending;

            return (SortedDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }

        /// <summary>
        ///     Called when the IsSortedWithColumnHeader property changes.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private static void OnIsSortedWithColumnHeaderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ListView listView = o as ListView;
            if (listView != null)
            {
                bool oldValue = (bool) e.OldValue;
                bool newValue = (bool) e.NewValue;

                if (oldValue && !newValue)
                {
                    listView.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                }

                if (!oldValue && newValue)
                {
                    listView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                }
            }
        }

        /// <summary>
        ///     Updates the adorner.
        /// </summary>
        /// <param name="columnHeader">The column header.</param>
        /// <param name="direction">The direction.</param>
        private static void UpdateAdorner(GridViewColumnHeader columnHeader, ListSortDirection direction)
        {
            // Remove the adorner.
            if (SortedColumnHeader != null)
                SortedColumnHeader.TryRemoveAdorners<SortAdorner>();

            // Resize the column header to fit the adorner.
            if (columnHeader.Column.ActualWidth < 80)
                columnHeader.Column.Width = 80;

            // Add the sort adorner.
            columnHeader.TryAddAdorner<SortAdorner>(new SortAdorner(columnHeader, direction));

            // Cache the last column.
            SortedColumnHeader = columnHeader;

            // Cache the sort direction.
            SortedDirection = direction;
        }

        #endregion
    }
}