using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace System.Windows.Behaviors
{
    /// <summary>
    ///     Synchronize the scroll position of various scrollable controls.
    /// </summary>
    /// <example>
    /// <![CDATA[
    ///                <Grid.Resources>
    ///                    <Style TargetType="ScrollViewer">
    ///                         <Setter Property="behaviors:ScrollSynchronizer.ScrollGroup"
    ///                                Value="Group1" />
    ///                    </Style>
    ///                </Grid.Resources>
    /// ]]></example>
    /// <remarks>https://www.codeproject.com/kb/wpf/scrollsynchronization.aspx</remarks>
    /// <seealso cref="System.Windows.DependencyObject" />
    public class ScrollSynchronizer : DependencyObject
    {
        #region Fields

        private static readonly Dictionary<string, double> HorizontalScrollOffsets =
            new Dictionary<string, double>();

        public static readonly DependencyProperty ScrollGroupProperty =
            DependencyProperty.RegisterAttached(
                "ScrollGroup",
                typeof (string),
                typeof (ScrollSynchronizer),
                new PropertyMetadata(OnScrollGroupChanged));

        private static readonly Dictionary<ScrollViewer, string> ScrollViewers =
            new Dictionary<ScrollViewer, string>();

        private static readonly Dictionary<string, double> VerticalScrollOffsets =
            new Dictionary<string, double>();

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the scroll group.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static string GetScrollGroup(DependencyObject obj)
        {
            return (string) obj.GetValue(ScrollGroupProperty);
        }

        /// <summary>
        ///     Sets the scroll group.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="scrollGroup">The scroll group.</param>
        public static void SetScrollGroup(DependencyObject obj, string scrollGroup)
        {
            obj.SetValue(ScrollGroupProperty, scrollGroup);
        }

        #endregion

        #region Private Methods

        private static void OnScrollGroupChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var scrollViewer = d as ScrollViewer;
            if (scrollViewer != null)
            {
                if (!string.IsNullOrEmpty((string) e.OldValue))
                {
                    // Remove scrollviewer
                    if (ScrollViewers.ContainsKey(scrollViewer))
                    {
                        scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
                        ScrollViewers.Remove(scrollViewer);
                    }
                }

                if (!string.IsNullOrEmpty((string) e.NewValue))
                {
                    // If group already exists, set scrollposition of 
                    // new scrollviewer to the scrollposition of the group
                    if (HorizontalScrollOffsets.Keys.Contains((string) e.NewValue))
                    {
                        scrollViewer.ScrollToHorizontalOffset(HorizontalScrollOffsets[(string) e.NewValue]);
                    }
                    else
                    {
                        HorizontalScrollOffsets.Add((string) e.NewValue, scrollViewer.HorizontalOffset);
                    }

                    if (VerticalScrollOffsets.Keys.Contains((string) e.NewValue))
                    {
                        scrollViewer.ScrollToVerticalOffset(VerticalScrollOffsets[(string) e.NewValue]);
                    }
                    else
                    {
                        VerticalScrollOffsets.Add((string) e.NewValue, scrollViewer.VerticalOffset);
                    }

                    // Add scrollviewer
                    ScrollViewers.Add(scrollViewer, (string) e.NewValue);
                    scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                }
            }
        }

        /// <summary>
        ///     Scrolls the specified changed scroll viewer.
        /// </summary>
        /// <param name="changedScrollViewer">The changed scroll viewer.</param>
        private static void Scroll(ScrollViewer changedScrollViewer)
        {
            var group = ScrollViewers[changedScrollViewer];
            VerticalScrollOffsets[group] = changedScrollViewer.VerticalOffset;
            HorizontalScrollOffsets[group] = changedScrollViewer.HorizontalOffset;

            foreach (var scrollViewer in ScrollViewers.Where((s) => s.Value ==
                                                                    group && !Equals(s.Key, changedScrollViewer)))
            {
                if (scrollViewer.Key.VerticalOffset != changedScrollViewer.VerticalOffset)
                {
                    scrollViewer.Key.ScrollToVerticalOffset(changedScrollViewer.VerticalOffset);
                }

                if (scrollViewer.Key.HorizontalOffset != changedScrollViewer.HorizontalOffset)
                {
                    scrollViewer.Key.ScrollToHorizontalOffset(changedScrollViewer.HorizontalOffset);
                }
            }
        }

        /// <summary>
        ///     Handles the ScrollChanged event of the ScrollViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ScrollChangedEventArgs" /> instance containing the event data.</param>
        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange != 0 || e.HorizontalChange != 0)
            {
                var changedScrollViewer = sender as ScrollViewer;
                Scroll(changedScrollViewer);
            }
        }

        #endregion
    }
}