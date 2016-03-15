using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.Framework;

namespace Miner.Interop
{    
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

            IMMAttributeEditorUI2 ui = (IMMAttributeEditorUI2) source.UI;
            for (int i = 0; i < ui.PageCount(); i++)
            {
                if (source.UI.ActivePage == i)
                {
                    switch (ui.PageCaption(i).ToUpperInvariant())
                    {
                        case "DESIGN":
                            return ArcMap.Application.GetDesignTab();

                        case "SELECTION":
                            return ArcMap.Application.GetSelectionTab();

                        case "TARGETS":
                            return ArcMap.Application.GetTargetTab();

                        case "QA/QC":
                            return ArcMap.Application.GetQAQCTab();
                    }
                }
            }

            return null;
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
        ///     Changes the tab of the attribute editor to that of the provide list.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="list">The list.</param>
        public static void Show(this IMMAttributeEditor source, ID8List list)
        {
            if (source == null) return;
            if (source.UI == null) source.Show(0);

            IMMAttributeEditorUI2 ui = (IMMAttributeEditorUI2) source.UI;
            for (int i = 0; i < ui.PageCount(); i++)
            {
                switch (ui.PageCaption(i).ToUpperInvariant())
                {
                    case "DESIGN":
                        if (list is ID8TopLevel)
                            source.UI.ActivePage = i;
                        break;

                    case "SELECTION":
                        if (list is ID8FeSelTopLevel)
                            source.UI.ActivePage = i;
                        break;

                    case "TARGETS":
                        if (list is ID8CuSelTopLevel)
                            source.UI.ActivePage = i;
                        break;

                    case "QA/QC":
                        if (list is IMMQAQCTopLevel)
                            source.UI.ActivePage = i;
                        break;
                }
            }
        }

        #endregion
    }
}