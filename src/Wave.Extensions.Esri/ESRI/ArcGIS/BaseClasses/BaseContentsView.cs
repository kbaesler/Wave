using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Framework;

namespace ESRI.ArcGIS.BaseClasses
{
    /// <summary>
    ///     An abstract class used to create a contents view in the Table of Contents in ArcMap.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseContentsView : IComPropertySheetEvents
#if V10
        , IContentsView3
#else
        , IContentsView
        , IContentsView2
#endif
    {
        #region Constructors

#if !V10
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseContentsView" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BaseContentsView(string name)
        {
            this.Name = name;
        }

#else
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseContentsView" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="toolTip">The tool tip.</param>
        protected BaseContentsView(string name, string toolTip)
        {
            this.Name = name;
            this.Tooltip = toolTip;
        }
#endif

        #endregion

        #region IComPropertySheetEvents Members

        /// <summary>
        ///     Occurs when changes are applied.
        /// </summary>
        public virtual void OnApply()
        {
        }

        #endregion

        #region IContentsView2 Members

        /// <summary>
        ///     Gets or sets the context item (could be an enumerator).
        /// </summary>
        /// <value>
        ///     The context item.
        /// </value>
        public object ContextItem { get; set; }

        /// <summary>
        ///     Gets or sets the name of the contents view.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; protected set; }

        /// <summary>
        ///     Gets or sets a boolean that indicates if the view is currently responding to events.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the view is currently responding to events; otherwise, <c>false</c>.
        /// </value>
        public bool ProcessEvents { set; protected get; }

        /// <summary>
        ///     Gets or sets the selected item (could be an enumerator).
        /// </summary>
        /// <value>
        ///     The selected item.
        /// </value>
        public object SelectedItem { get; set; }

        /// <summary>
        ///     Gets or sets a boolean that indicates if lines are shown in the TOC tree.
        /// </summary>
        /// <value></value>
        public bool ShowLines { get; set; }

        /// <summary>
        ///     Gets or sets a boolean that indicates if the view is visible.
        /// </summary>
        /// <value>
        ///     <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        public bool Visible { get; set; }

        /// <summary>
        ///     Gets the HWND of the contents view.
        /// </summary>
        public abstract int hWnd { get; }

        /// <summary>
        ///     Called when the contents view is selected.
        /// </summary>
        /// <param name="parentHWnd">The parent window handle.</param>
        /// <param name="document">The document.</param>
        public virtual void Activate(int parentHWnd, IMxDocument document)
        {
        }

        /// <summary>
        ///     Activates the contents view.
        /// </summary>
        /// <param name="parentHWnd">The parent window handle.</param>
        /// <param name="document">The document.</param>
        public virtual void BasicActivate(int parentHWnd, IDocument document)
        {
        }

        /// <summary>
        ///     Adds to the selected items.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void AddToSelectedItems(object item)
        {
        }

        /// <summary>
        ///     Called when the contents view has been deactivated or when the tab in the table of contents has changed.
        /// </summary>
        public virtual void Deactivate()
        {
        }

        /// <summary>
        ///     Refreshes the contents view.  If a non-null item is specified, it refreshes only that item and its children.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void Refresh(object item)
        {
        }

        /// <summary>
        ///     Removes an item from the selected items.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void RemoveFromSelectedItems(object item)
        {
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Registers the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComRegisterFunction]
        internal static void Register(string registryKey)
        {
            ContentsViews.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            ContentsViews.Unregister(registryKey);
        }

        #endregion
#if V10
        #region IContentsView3 Members

        /// <summary>
        ///     Bitmap shown in Table Of Contents window toolbar.
        /// </summary>
        public abstract int Bitmap { get; }

        /// <summary>
        ///     The tool tip for the table of contents window.
        /// </summary>
        public string Tooltip { get; protected set; }

        #endregion
#endif
    }
}