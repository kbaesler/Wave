using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

namespace ESRI.ArcGIS.ADF.BaseClasses
{
    /// <summary>
    ///     An abstract class that is used for creating dockable windows.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseDockableWindow : IDockableWindowDef
        , IDockableWindowImageDef
        , IDockableWindowInitialPlacement
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseDockableWindow" /> class.
        /// </summary>
        /// <param name="name">The name of the window.</param>
        /// <param name="caption">The caption for the window.</param>
        protected BaseDockableWindow(string name, string caption)
            : this(name, caption, esriDockFlags.esriDockFloat)
        {
            Name = name;
            Caption = caption;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseDockableWindow" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="dockPosition">The dock position.</param>
        protected BaseDockableWindow(string name, string caption, esriDockFlags dockPosition)
        {
            Name = name;
            Caption = caption;
            DockPosition = dockPosition;
        }

        #endregion

        #region Public Properties

        #region IDockableWindowImageDef

        /// <summary>
        ///     The bitmap for the dockable window.
        /// </summary>
        public abstract int Bitmap { get; }

        #endregion IDockableWindowImageDef

        /// <summary>
        ///     Gets the child window handle
        /// </summary>
        /// <value>The child window handle.</value>
        public abstract int ChildHWND { get; }

        /// <summary>
        ///     Gets the user data.
        /// </summary>
        /// <value>The user data.</value>
        public virtual object UserData
        {
            get { return null; }
        }

        /// <summary>
        ///     Gets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption { get; protected set; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; protected set; }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the application.
        /// </summary>
        /// <value>The application.</value>
        protected IApplication Application { get; set; }

        #endregion

        #region IDockableWindowDef Members

        /// <summary>
        ///     Called when the <see cref="BaseDockableWindow" /> is created.
        /// </summary>
        /// <param name="hook">The hook </param>
        public virtual void OnCreate(object hook)
        {
            Application = hook as IApplication;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void OnDestroy()
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Docks window using the specified dock flag.
        /// </summary>
        /// <param name="dockFlag">The dock flag.</param>
        protected void Dock(esriDockFlags dockFlag)
        {
            UID uid = new UIDClass {Value = GetType().GUID.ToString("B")};
            var windowManager = Application as IDockableWindowManager;
            if (windowManager != null)
            {
                var dockableWindow = windowManager.GetDockableWindow(uid);
                dockableWindow.Dock(dockFlag);
            }
        }

        /// <summary>
        ///     Hides the dockable window.
        /// </summary>
        protected void Hide()
        {
            UID uid = new UIDClass {Value = GetType().GUID.ToString("B")};
            var windowManager = Application as IDockableWindowManager;
            if (windowManager != null)
            {
                var dockableWindow = windowManager.GetDockableWindow(uid);
                dockableWindow.Show(false);
            }
        }

        /// <summary>
        ///     Determines whether the specified dockable window is visible.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the specified dockable window is visible; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsVisible()
        {
            UID uid = new UIDClass {Value = GetType().GUID.ToString("B")};
            var windowManager = Application as IDockableWindowManager;
            if (windowManager != null)
            {
                var dockableWindow = windowManager.GetDockableWindow(uid);
                return dockableWindow.IsVisible();
            }

            return false;
        }

        /// <summary>
        ///     Shows the dockable window.
        /// </summary>
        protected void Show()
        {
            UID uid = new UIDClass {Value = GetType().GUID.ToString("B")};
            var windowManager = Application as IDockableWindowManager;
            if (windowManager != null)
            {
                var dockableWindow = windowManager.GetDockableWindow(uid);
                dockableWindow.Show(true);
            }
        }

        #endregion

        #region IDockableWindowInitialPlacement

        /// <summary>
        ///     The location where this dockable window should dock by default.
        /// </summary>
        public esriDockFlags DockPosition { get; protected set; }

        /// <summary>
        ///     The default height of the dockable window in pixels.
        /// </summary>
        public abstract int Height { get; }

        /// <summary>
        ///     An alternate dockable window this dockable window should dock relative to.
        /// </summary>
        public virtual UID Neighbor { get; protected set; }

        /// <summary>
        ///     The default width of the dockable window in pixels.
        /// </summary>
        public abstract int Width { get; }

        #endregion IDockableWindowInitialPlacement
    }
}