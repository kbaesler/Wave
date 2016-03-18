using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.Framework;

using Miner.Desktop;
using Miner.Desktop.Designer;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides access to the <see cref="IMMAttributeEditor" />
    /// </summary>
    public static class AttributeEditor
    {
        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static IMMAttributeEditor Instance
        {
            get { return ArcMap.Application.GetAttributeEditor(); }
        }

        #endregion
    }

    /// <summary>
    ///     Provides extension methods for the <see cref="IMMAttributeEditor" /> interface.
    /// </summary>
    public static class AttributeEditorExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the <see cref="ID8List" /> of the active tab.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="ID8List" /> representing the list of the active tab.</returns>
        public static ID8List GetActiveTab(this IMMAttributeEditor source)
        {
            if (source == null) return null;
            if (source.UI == null) return null;

            return source.GetTabContents((mmAETabIndex) source.UI.ActivePage);
        }

        /// <summary>
        ///     Gets the selected items on the active tab.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{ID8ListItem}" /> representing the selected items of the active tab.</returns>
        public static IEnumerable<ID8ListItem> GetSelectedItems(this IMMAttributeEditor source)
        {
            return source.GetSelectedItems(item => true);
        }

        /// <summary>
        ///     Gets the selected items on the active tab.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate delegate used to filter the selected items.</param>
        /// <returns>
        ///     Returns a <see cref="IEnumerable{ID8ListItem}" /> representing the selected items of the active tab.
        /// </returns>
        public static IEnumerable<ID8ListItem> GetSelectedItems(this IMMAttributeEditor source, Func<ID8ListItem, bool> predicate)
        {
            var list = source.GetActiveTab();
            if (list == null) return null;

            return list.Where(o => o.IsSelected).Select(o => o.Value).Where(predicate);
        }

        /// <summary>
        ///     Gets the <see cref="ID8List" /> of the tab.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <returns>
        ///     Returns a <see cref="ID8List" /> representing the list of the tab.
        /// </returns>
        public static ID8List GetTabContents(this IMMAttributeEditor source, mmAETabIndex tabIndex)
        {
            if (source == null) return null;

            switch (tabIndex)
            {
                case mmAETabIndex.mmAEDesign:
                    return DesignerTopLevel.Instance as ID8List;

                case mmAETabIndex.mmAESelection:
                    return FeSelTopLevel.Instance as ID8List;

                case mmAETabIndex.mmAETarget:
                    return CuSelTopLevel.Instance as ID8List;

                case mmAETabIndex.mmAEQAQC:
                    return QAQCTopLevel.Instance as ID8List;
            }

            return null;
        }

        /// <summary>
        ///     Hides the attribute editor.
        /// </summary>
        /// <param name="source">The source.</param>
        public static void Hide(this IMMAttributeEditor source)
        {
            if (source == null) return;

            var ui = source.UI as IMMAttributeEditorUI;
            if (ui == null) return;

            ui.Hide();
        }

        /// <summary>
        ///     Determines whether the attribute editor is visible.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="bool" /> representing <c>true</c> when the attribute editor is visible.</returns>
        public static bool IsVisible(this IMMAttributeEditor source)
        {
            if (source == null) return false;

            var ui = source.UI as IMMAttributeEditorUI2;
            if (ui == null) return false;

            return true;
        }

        /// <summary>
        ///     Selects the item in the attribute editor when specified tab is visible.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     Returns <see cref="bool" /> representing <c>true</c> when the item was selected.
        /// </returns>
        public static bool SelectItem(this IMMAttributeEditor source, mmAETabIndex tabIndex, ID8ListItem item)
        {
            if (source == null) return false;

            if (source.Show(tabIndex))
            {
                var ui = source.UI as IMMAttributeEditorUI2;
                if (ui == null) return false;

                ui.SelectItem((int) tabIndex, item);

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Shows the specified tab index.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <returns>
        ///     Returns <see cref="bool" /> representing <c>true</c> when the tab is shown.
        /// </returns>
        public static bool Show(this IMMAttributeEditor source, mmAETabIndex tabIndex)
        {
            if (source == null) return false;

            var ui = source.UI as IMMAttributeEditorUI;
            if (ui == null) return false;

            if (!ui.PageVisible[(int) tabIndex])
            {
                return false;
            }

            ui.ActivePage = (int) tabIndex;
            ui.Show();

            return true;
        }

        /// <summary>
        ///     Updates the tab with the contents of the specified list.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="tabIndex">Index of the tab.</param>
        /// <param name="list">The list.</param>
        public static void UpdateTab(this IMMAttributeEditor source, mmAETabIndex tabIndex, ID8List list)
        {
            var tab = source.GetTabContents(tabIndex);
            tab.Clear();

            if (list.HasChildren)
            {
                ID8ListItem item;
                while ((item = list.Next(false)) != null)
                    tab.Add(item);
            }
        }

        #endregion
    }
}