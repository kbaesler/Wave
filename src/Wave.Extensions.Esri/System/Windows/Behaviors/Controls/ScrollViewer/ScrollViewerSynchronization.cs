using System.Collections.Generic;
using System.Windows.Controls;

namespace System.Windows.Behaviors
{
    /// <summary>
    ///     Synchronize the scroll position of various scrollable controls.
    /// </summary>
    /// <example>
    ///     <![CDATA[
    ///                <Grid.Resources>
    ///                    <Style TargetType="ScrollViewer">
    ///                         <Setter Property="behaviors:ScrollViewerSynchronization.VerticalScrollGroup"
    ///                                Value="Group1" />
    ///                    </Style>
    ///                </Grid.Resources>
    /// ]]></example>
    /// <remarks>https://www.codeproject.com/kb/wpf/scrollsynchronization.aspx</remarks>
    /// <seealso cref="System.Windows.DependencyObject" />
    public sealed class ScrollViewerSynchronization
    {
        #region Constants

        private const string HorizontalScrollGroupPropertyName = "HorizontalScrollGroup";
        private const string ScrollSyncTypePropertyName = "ScrollSyncType";


        private const string VerticalScrollGroupPropertyName = "VerticalScrollGroup";

        #endregion

        #region Fields

        /// <summary>
        ///     The horizontal scroll group property
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollGroupProperty =
            DependencyProperty.RegisterAttached(HorizontalScrollGroupPropertyName, typeof(string), typeof(ScrollViewerSynchronization), new PropertyMetadata(string.Empty, OnHorizontalScrollGroupChanged));

        /// <summary>
        ///     The vertical scroll group property
        /// </summary>
        public static readonly DependencyProperty VerticalScrollGroupProperty =
            DependencyProperty.RegisterAttached(VerticalScrollGroupPropertyName, typeof(string), typeof(ScrollViewerSynchronization), new PropertyMetadata(string.Empty, OnVerticalScrollGroupChanged));

        /// <summary>
        ///     The scroll synchronize type property
        /// </summary>
        public static readonly DependencyProperty ScrollSyncTypeProperty =
            DependencyProperty.RegisterAttached(ScrollSyncTypePropertyName, typeof(ScrollSyncType), typeof(ScrollViewerSynchronization), new PropertyMetadata(ScrollSyncType.None, OnScrollSyncTypeChanged));

        private static readonly Dictionary<string, OffSetContainer> HorizontalScrollGroups = new Dictionary<string, OffSetContainer>();
        private static readonly Dictionary<ScrollViewer, ScrollSyncType> RegisteredScrollViewers = new Dictionary<ScrollViewer, ScrollSyncType>();


        private static readonly Dictionary<string, OffSetContainer> VerticalScrollGroups = new Dictionary<string, OffSetContainer>();

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the horizontal scroll group.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string GetHorizontalScrollGroup(DependencyObject obj)
        {
            return (string) obj.GetValue(HorizontalScrollGroupProperty);
        }

        /// <summary>
        ///     Gets the type of the scroll synchronize.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static ScrollSyncType GetScrollSyncType(DependencyObject obj)
        {
            return (ScrollSyncType) obj.GetValue(ScrollSyncTypeProperty);
        }

        /// <summary>
        ///     Gets the vertical scroll group.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string GetVerticalScrollGroup(DependencyObject obj)
        {
            return (string) obj.GetValue(VerticalScrollGroupProperty);
        }

        /// <summary>
        ///     Sets the horizontal scroll group.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="horizontalScrollGroup">The horizontal scroll group.</param>
        public static void SetHorizontalScrollGroup(DependencyObject obj, string horizontalScrollGroup)
        {
            obj.SetValue(HorizontalScrollGroupProperty, horizontalScrollGroup);
        }

        /// <summary>
        ///     Sets the type of the scroll synchronize.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="scrollSyncType">Type of the scroll synchronize.</param>
        public static void SetScrollSyncType(DependencyObject obj, ScrollSyncType scrollSyncType)
        {
            obj.SetValue(ScrollSyncTypeProperty, scrollSyncType);
        }


        /// <summary>
        ///     Sets the vertical scroll group.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="verticalScrollGroup">The vertical scroll group.</param>
        public static void SetVerticalScrollGroup(DependencyObject obj, string verticalScrollGroup)
        {
            obj.SetValue(VerticalScrollGroupProperty, verticalScrollGroup);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Adds to horizontal scroll group.
        /// </summary>
        /// <param name="horizontalGroupName">Name of the horizontal group.</param>
        /// <param name="scrollViewer">The scroll viewer.</param>
        private static void AddToHorizontalScrollGroup(string horizontalGroupName, ScrollViewer scrollViewer)
        {
            if (HorizontalScrollGroups.ContainsKey(horizontalGroupName))
            {
                scrollViewer.ScrollToHorizontalOffset(HorizontalScrollGroups[horizontalGroupName].Offset);
                HorizontalScrollGroups[horizontalGroupName].ScrollViewers.Add(scrollViewer);
            }
            else
            {
                HorizontalScrollGroups.Add(horizontalGroupName, new OffSetContainer {ScrollViewers = new List<ScrollViewer> {scrollViewer}, Offset = scrollViewer.HorizontalOffset});
            }

            scrollViewer.ScrollChanged += ScrollViewer_HorizontalScrollChanged;
        }

        /// <summary>
        ///     Adds to vertical scroll group.
        /// </summary>
        /// <param name="verticalGroupName">Name of the vertical group.</param>
        /// <param name="scrollViewer">The scroll viewer.</param>
        private static void AddToVerticalScrollGroup(string verticalGroupName, ScrollViewer scrollViewer)
        {
            if (VerticalScrollGroups.ContainsKey(verticalGroupName))
            {
                scrollViewer.ScrollToVerticalOffset(VerticalScrollGroups[verticalGroupName].Offset);
                VerticalScrollGroups[verticalGroupName].ScrollViewers.Add(scrollViewer);
            }
            else
            {
                VerticalScrollGroups.Add(verticalGroupName, new OffSetContainer {ScrollViewers = new List<ScrollViewer> {scrollViewer}, Offset = scrollViewer.VerticalOffset});
            }

            scrollViewer.ScrollChanged += ScrollViewer_VerticalScrollChanged;
        }

        /// <summary>
        ///     Called when [horizontal scroll group changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnHorizontalScrollGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            if (scrollViewer == null)
                return;

            var newHorizontalGroupName = e.NewValue == DependencyProperty.UnsetValue ? string.Empty : (string) e.NewValue;
            var oldHorizontalGroupName = e.NewValue == DependencyProperty.UnsetValue ? string.Empty : (string) e.OldValue;

            RemoveFromHorizontalScrollGroup(oldHorizontalGroupName, scrollViewer);
            AddToHorizontalScrollGroup(newHorizontalGroupName, scrollViewer);

            var currentScrollSyncValue = ReadSyncTypeValue(d, ScrollSyncTypeProperty);
            if (currentScrollSyncValue == ScrollSyncType.None)
                d.SetValue(ScrollSyncTypeProperty, ScrollSyncType.Horizontal);
            else if (currentScrollSyncValue == ScrollSyncType.Vertical)
                d.SetValue(ScrollSyncTypeProperty, ScrollSyncType.Both);
        }

        /// <summary>
        ///     Called when [scroll synchronize type changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnScrollSyncTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            if (scrollViewer == null)
                return;

            var verticalGroupName = ReadStringValue(d, VerticalScrollGroupProperty);
            var horizontalGroupName = ReadStringValue(d, HorizontalScrollGroupProperty);

            var scrollSyncType = ScrollSyncType.None;
            try
            {
                scrollSyncType = (ScrollSyncType) e.NewValue;
            }
            catch
            {
            }

            switch (scrollSyncType)
            {
                case ScrollSyncType.None:
                    if (!RegisteredScrollViewers.ContainsKey(scrollViewer))
                        return;

                    RemoveFromVerticalScrollGroup(verticalGroupName, scrollViewer);
                    RemoveFromHorizontalScrollGroup(horizontalGroupName, scrollViewer);
                    RegisteredScrollViewers.Remove(scrollViewer);

                    break;
                case ScrollSyncType.Horizontal:
                    RemoveFromVerticalScrollGroup(verticalGroupName, scrollViewer);
                    AddToHorizontalScrollGroup(horizontalGroupName, scrollViewer);

                    if (RegisteredScrollViewers.ContainsKey(scrollViewer))
                        RegisteredScrollViewers[scrollViewer] = ScrollSyncType.Horizontal;
                    else
                        RegisteredScrollViewers.Add(scrollViewer, ScrollSyncType.Horizontal);

                    break;
                case ScrollSyncType.Vertical:
                    RemoveFromHorizontalScrollGroup(horizontalGroupName, scrollViewer);
                    AddToVerticalScrollGroup(verticalGroupName, scrollViewer);

                    if (RegisteredScrollViewers.ContainsKey(scrollViewer))
                        RegisteredScrollViewers[scrollViewer] = ScrollSyncType.Vertical;
                    else
                        RegisteredScrollViewers.Add(scrollViewer, ScrollSyncType.Vertical);

                    break;
                case ScrollSyncType.Both:
                    if (RegisteredScrollViewers.ContainsKey(scrollViewer))
                    {
                        if (RegisteredScrollViewers[scrollViewer] == ScrollSyncType.Horizontal)
                            AddToVerticalScrollGroup(verticalGroupName, scrollViewer);
                        else if (RegisteredScrollViewers[scrollViewer] == ScrollSyncType.Vertical)
                            AddToHorizontalScrollGroup(horizontalGroupName, scrollViewer);

                        RegisteredScrollViewers[scrollViewer] = ScrollSyncType.Both;
                    }
                    else
                    {
                        AddToHorizontalScrollGroup(horizontalGroupName, scrollViewer);
                        AddToVerticalScrollGroup(verticalGroupName, scrollViewer);

                        RegisteredScrollViewers.Add(scrollViewer, ScrollSyncType.Both);
                    }

                    break;
            }
        }


        /// <summary>
        ///     Called when [vertical scroll group changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnVerticalScrollGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            if (scrollViewer == null)
                return;

            var newVerticalGroupName = e.NewValue == DependencyProperty.UnsetValue ? string.Empty : (string) e.NewValue;
            var oldVerticalGroupName = e.NewValue == DependencyProperty.UnsetValue ? string.Empty : (string) e.OldValue;

            RemoveFromVerticalScrollGroup(oldVerticalGroupName, scrollViewer);
            AddToVerticalScrollGroup(newVerticalGroupName, scrollViewer);

            var currentScrollSyncValue = ReadSyncTypeValue(d, ScrollSyncTypeProperty);
            if (currentScrollSyncValue == ScrollSyncType.None)
                d.SetValue(ScrollSyncTypeProperty, ScrollSyncType.Vertical);
            else if (currentScrollSyncValue == ScrollSyncType.Horizontal)
                d.SetValue(ScrollSyncTypeProperty, ScrollSyncType.Vertical);
        }

        /// <summary>
        ///     Reads the string value.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="dp">The dp.</param>
        /// <returns></returns>
        private static string ReadStringValue(DependencyObject d, DependencyProperty dp)
        {
            var value = d.ReadLocalValue(dp);
            return value == DependencyProperty.UnsetValue ? string.Empty : value.ToString();
        }

        /// <summary>
        ///     Reads the synchronize type value.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="dp">The dp.</param>
        /// <returns></returns>
        private static ScrollSyncType ReadSyncTypeValue(DependencyObject d, DependencyProperty dp)
        {
            var value = d.ReadLocalValue(dp);
            return value == DependencyProperty.UnsetValue ? ScrollSyncType.None : (ScrollSyncType) value;
        }

        /// <summary>
        ///     Removes from horizontal scroll group.
        /// </summary>
        /// <param name="horizontalGroupName">Name of the horizontal group.</param>
        /// <param name="scrollViewer">The scroll viewer.</param>
        private static void RemoveFromHorizontalScrollGroup(string horizontalGroupName, ScrollViewer scrollViewer)
        {
            if (HorizontalScrollGroups.ContainsKey(horizontalGroupName))
            {
                HorizontalScrollGroups[horizontalGroupName].ScrollViewers.Remove(scrollViewer);
                if (HorizontalScrollGroups[horizontalGroupName].ScrollViewers.Count == 0)
                    HorizontalScrollGroups.Remove(horizontalGroupName);
            }

            scrollViewer.ScrollChanged -= ScrollViewer_HorizontalScrollChanged;
        }


        /// <summary>
        ///     Removes from vertical scroll group.
        /// </summary>
        /// <param name="verticalGroupName">Name of the vertical group.</param>
        /// <param name="scrollViewer">The scroll viewer.</param>
        private static void RemoveFromVerticalScrollGroup(string verticalGroupName, ScrollViewer scrollViewer)
        {
            if (VerticalScrollGroups.ContainsKey(verticalGroupName))
            {
                VerticalScrollGroups[verticalGroupName].ScrollViewers.Remove(scrollViewer);
                if (VerticalScrollGroups[verticalGroupName].ScrollViewers.Count == 0)
                    VerticalScrollGroups.Remove(verticalGroupName);
            }

            scrollViewer.ScrollChanged -= ScrollViewer_VerticalScrollChanged;
        }

        /// <summary>
        ///     Handles the HorizontalScrollChanged event of the ScrollViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ScrollChangedEventArgs" /> instance containing the event data.</param>
        private static void ScrollViewer_HorizontalScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var changedScrollViewer = sender as ScrollViewer;
            if (changedScrollViewer == null)
                return;

            if (e.HorizontalChange == 0)
                return;

            var horizontalScrollGroup = ReadStringValue(sender as DependencyObject, HorizontalScrollGroupProperty);
            if (!HorizontalScrollGroups.ContainsKey(horizontalScrollGroup))
                return;

            HorizontalScrollGroups[horizontalScrollGroup].Offset = changedScrollViewer.HorizontalOffset;

            foreach (var scrollViewer in HorizontalScrollGroups[horizontalScrollGroup].ScrollViewers)
            {
                if (scrollViewer.HorizontalOffset == changedScrollViewer.HorizontalOffset)
                    continue;

                scrollViewer.ScrollToHorizontalOffset(changedScrollViewer.HorizontalOffset);
            }
        }


        /// <summary>
        ///     Handles the VerticalScrollChanged event of the ScrollViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ScrollChangedEventArgs" /> instance containing the event data.</param>
        private static void ScrollViewer_VerticalScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var changedScrollViewer = sender as ScrollViewer;
            if (changedScrollViewer == null)
                return;

            if (e.VerticalChange == 0)
                return;

            var verticalScrollGroup = ReadStringValue(sender as DependencyObject, VerticalScrollGroupProperty);
            if (!VerticalScrollGroups.ContainsKey(verticalScrollGroup))
                return;

            VerticalScrollGroups[verticalScrollGroup].Offset = changedScrollViewer.VerticalOffset;

            foreach (var scrollViewer in VerticalScrollGroups[verticalScrollGroup].ScrollViewers)
            {
                if (scrollViewer.VerticalOffset == changedScrollViewer.VerticalOffset)
                    continue;

                scrollViewer.ScrollToVerticalOffset(changedScrollViewer.VerticalOffset);
            }
        }

        #endregion

        #region Nested Type: OffSetContainer

        /// <summary>
        /// </summary>
        private class OffSetContainer
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the offset.
            /// </summary>
            /// <value>
            ///     The offset.
            /// </value>
            public double Offset { get; set; }

            /// <summary>
            ///     Gets or sets the scroll viewers.
            /// </summary>
            /// <value>
            ///     The scroll viewers.
            /// </value>
            public List<ScrollViewer> ScrollViewers { get; set; }

            #endregion
        }

        #endregion
    }

    /// <summary>
    /// </summary>
    public enum ScrollSyncType
    {
        /// <summary>
        ///     Scrolling synced both vertical and horizontal.
        /// </summary>
        Both,

        /// <summary>
        ///     Scrolling synced only horizontal.
        /// </summary>
        Horizontal,

        /// <summary>
        ///     Scrolling synced only vertical.
        /// </summary>
        Vertical,

        /// <summary>
        ///     Default value of this property. Scrolling will be disabled.s
        /// </summary>
        None
    }
}