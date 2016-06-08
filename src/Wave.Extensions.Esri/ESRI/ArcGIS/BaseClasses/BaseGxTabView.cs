using System;
using System.Globalization;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.ADF.BaseClasses
{
    /// <summary>
    ///     An abstract GxView which is used within ArcCatalog and requires a window / control to display the contents through
    ///     the hWnd property.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseGxTabView : IGxView, IDisposable
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseGxTabView" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BaseGxTabView(string name)
        {
            this.Name = name;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region IGxView Members

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the view's window handle.
        /// </summary>
        public abstract int hWnd { get; }

        /// <summary>
        ///     Gets the class ID.
        /// </summary>
        public virtual UID ClassID
        {
            get
            {
                GuidAttribute attribute = (GuidAttribute) Attribute.GetCustomAttribute(this.GetType(), typeof (GuidAttribute));
                if (attribute == null) return null;

                UID guid = new UIDClass();
                guid.Value = string.Format(CultureInfo.InvariantCulture, "{{{0}}}", attribute.Value);
                return guid;
            }
        }

        /// <summary>
        ///     Gets the default toolbar CLSID.
        /// </summary>
        /// <remarks>The class ID of the view's default toolbar. Not currently used.</remarks>
        public virtual UID DefaultToolbarCLSID
        {
            get { return null; }
        }

        /// <summary>
        ///     Gets a value indicating if the view supports tools.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the view supports tools; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        ///     If the SupportsTools property returns True, ArcCatalog will intercept mouse events normally destined for the
        ///     view, and instead send them to the active tool.
        /// </remarks>
        public virtual bool SupportsTools
        {
            get { return false; }
        }

        /// <summary>
        ///     Activates the specified view.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="catalog">The catalog.</param>
        /// <remarks>
        ///     Use the Activate method to hold on to the application reference and GxCatalog objects that are passed in as
        ///     parameters.
        /// </remarks>
        public abstract void Activate(IGxApplication application, IGxCatalog catalog);

        /// <summary>
        ///     Indicates if the view can display the given object.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns>
        ///     <c>true</c> to display the object, otherwise <c>false</c>
        /// </returns>
        public virtual bool Applies(IGxObject selection)
        {
            return (selection != null);
        }

        /// <summary>
        ///     Deactivates the view by releasing references.
        /// </summary>
        public virtual void Deactivate()
        {
        }

        /// <summary>
        ///     Refreshes the view.
        /// </summary>
        public virtual void Refresh()
        {
        }

        /// <summary>
        ///     Informs the view that a system setting has changed.
        /// </summary>
        /// <param name="flag">The flag.</param>
        /// <param name="section">The section.</param>
        public virtual void SystemSettingChanged(int flag, string section)
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
            GxTabViews.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            GxTabViews.Unregister(registryKey);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}