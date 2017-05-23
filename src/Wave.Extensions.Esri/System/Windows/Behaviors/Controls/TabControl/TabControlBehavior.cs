using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using System.Windows.Threading;

namespace System.Windows.Behaviors
{
    /// <summary>
    ///     Behaviors for TabControl.
    /// </summary>
    public static class TabControlBehavior
    {
        #region Fields

        /// <summary>
        ///     Whether to focus the first visible tab.
        /// </summary>
        /// <remarks>
        ///     Setting the FocusFirstVisibleTab
        ///     attached property to true will focus the next visible tab when
        ///     the current selected tab's Visibility property is set to Collapsed or Hidden.
        /// </remarks>
        public static readonly DependencyProperty FocusFirstVisibleTabProperty =
            DependencyProperty.RegisterAttached("FocusFirstVisibleTab", typeof (bool),
                typeof (TabControlBehavior),
                new FrameworkPropertyMetadata(OnFocusFirstVisibleTabPropertyChanged));

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the focus first visible tab value of the given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool GetFocusFirstVisibleTab(TabControl element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (bool) element.GetValue(FocusFirstVisibleTabProperty);
        }

        /// <summary>
        ///     Sets the focus first visible tab value of the given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">
        ///     if set to <c>true</c> [value].
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetFocusFirstVisibleTab(TabControl element, bool value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(FocusFirstVisibleTabProperty, value);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Determines whether the value of the dependency property <c>IsFocused</c> has change.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" />
        ///     instance containing the event data.
        /// </param>
        private static void OnFocusFirstVisibleTabPropertyChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tabControl = d as TabControl;
            if (tabControl != null)
            {
                // Attach or detach the event handlers.

                if ((bool) e.NewValue)
                {
                    // Enable the attached behavior.
                    tabControl.Items.CurrentChanged += TabControl_Items_CurrentChanged;
                    var collection = tabControl.Items as INotifyCollectionChanged;
                    if (collection != null)
                    {
                        collection.CollectionChanged += TabControl_Items_CollectionChanged;
                    }
                }
                else
                {
                    // Disable the attached behavior.
                    tabControl.Items.CurrentChanged -= TabControl_Items_CurrentChanged;
                    var collection = tabControl.Items as INotifyCollectionChanged;
                    if (collection != null)
                    {
                        collection.CollectionChanged -= TabControl_Items_CollectionChanged;
                    }

                    // Detach handlers from the tab items.

                    foreach (object item in tabControl.Items)
                    {
                        var tab = item as TabItem;
                        if (tab != null)
                        {
                            tab.IsVisibleChanged -= TabItem_IsVisibleChanged;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Handles the CollectionChanged event of the TabControl.Items collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        ///     The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs" />
        ///     instance containing the event data.
        /// </param>
        private static void TabControl_Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Attach event handlers to each tab so that when the Visibility property changes of the selected tab,
            // the focus can be shifted to the next (or previous, if not next tab available) tab.
            var collection = sender as ItemCollection;
            if (collection != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Replace:

                        // Attach event handlers to the Visibility and IsEnabled properties.
                        if (e.NewItems != null)
                        {
                            foreach (object item in e.NewItems)
                            {
                                var tab = item as TabItem;
                                if (tab != null)
                                {
                                    tab.IsVisibleChanged +=
                                        TabItem_IsVisibleChanged;
                                }
                            }
                        }

                        // Detach event handlers from old items.
                        if (e.OldItems != null)
                        {
                            foreach (object item in e.OldItems)
                            {
                                var tab = item as TabItem;
                                if (tab != null)
                                {
                                    tab.IsVisibleChanged -=
                                        TabItem_IsVisibleChanged;
                                }
                            }
                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:

                        // Attach event handlers to the Visibility and IsEnabled properties.
                        foreach (object item in collection)
                        {
                            var tab = item as TabItem;
                            if (tab != null)
                            {
                                tab.IsVisibleChanged +=
                                    TabItem_IsVisibleChanged;
                            }
                        }
                        break;
                    default:
                        break;
                }

                // Select the first element if necessary.
                if (collection.Count > 0 && collection.CurrentItem == null)
                {
                    collection.MoveCurrentToFirst();
                }
            }
        }

        /// <summary>
        ///     Handles the CurrentChanged event of the TabControl.Items collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        ///     The <see cref="System.EventArgs" />
        ///     instance containing the event data.
        /// </param>
        private static void TabControl_Items_CurrentChanged(object sender, EventArgs e)
        {
            var collection = sender as ItemCollection;
            if (collection != null)
            {
                var element = collection.CurrentItem as UIElement;
                if (element != null && element.Visibility != Visibility.Visible)
                {
                    element.Dispatcher.BeginInvoke(new Action(() => collection.MoveCurrentToNext()),
                        DispatcherPriority.Input);
                }
            }
        }

        /// <summary>
        ///     Handles the IsVisibleChanged event of the tab item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" />
        ///     instance containing the event data.
        /// </param>
        private static void TabItem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tab = sender as TabItem;
            if (tab != null && tab.IsSelected && tab.Visibility != Visibility.Visible)
            {
                // Move to the next tab item.
                var tabControl = tab.Parent as TabControl;
                if (tabControl != null)
                {
                    if (!tabControl.Items.MoveCurrentToNext())
                    {
                        // Could not move to next, try previous.
                        tabControl.Items.MoveCurrentToPrevious();
                    }
                }
            }
        }

        #endregion
    }
}