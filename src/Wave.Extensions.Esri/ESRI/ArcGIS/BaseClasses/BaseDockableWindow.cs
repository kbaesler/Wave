using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

namespace ESRI.ArcGIS.BaseClasses
{
    /// <summary>
    ///     An abstract class that is used for creating dockable windows.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseDockableWindow : IDockableWindowDef
#if V10
        , IDockableWindowImageDef
        , IDockableWindowInitialPlacement
#endif
    {
        #region Constructors

#if !V10
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseDockableWindow" /> class.
        /// </summary>
        /// <param name="name">The name of the window.</param>
        /// <param name="caption">The caption for the window.</param>
        protected BaseDockableWindow(string name, string caption)
        {
            this.Name = name;
            this.Caption = caption;
        }
#else
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseDockableWindow" /> class.
        /// </summary>
        /// <param name="name">The name of the window.</param>
        /// <param name="caption">The caption for the window.</param>
        protected BaseDockableWindow(string name, string caption)
            : this(name, caption, esriDockFlags.esriDockFloat)
        {
            this.Name = name;
            this.Caption = caption;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseDockableWindow" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="dockPosition">The dock position.</param>
        protected BaseDockableWindow(string name, string caption, esriDockFlags dockPosition)
        {
            this.Name = name;
            this.Caption = caption;
            this.DockPosition = dockPosition;
        }
#endif
        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the application.
        /// </summary>
        /// <value>The application.</value>
        protected IApplication Application { get; set; }

        /// <summary>
        ///     Gets the UID.
        /// </summary>
        protected abstract UID UID { get; }

        #endregion

        #region IDockableWindowDef Members

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
        ///     Called when the <see cref="BaseDockableWindow" /> is created.
        /// </summary>
        /// <param name="hook">The hook </param>
        public virtual void OnCreate(object hook)
        {
            this.Application = hook as IApplication;
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
            IDockableWindowManager windowManager = (IDockableWindowManager) this.Application;
            IDockableWindow dockableWindow = windowManager.GetDockableWindow(this.UID);
            dockableWindow.Dock(dockFlag);
        }

        /// <summary>
        ///     Hides the dockable window.
        /// </summary>
        protected void Hide()
        {
            IDockableWindowManager windowManager = (IDockableWindowManager) this.Application;
            IDockableWindow dockableWindow = windowManager.GetDockableWindow(this.UID);
            dockableWindow.Show(false);
        }

        /// <summary>
        ///     Determines whether the specified dockable window is visible.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the specified dockable window is visible; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsVisible()
        {
            IDockableWindowManager windowManager = (IDockableWindowManager) this.Application;
            IDockableWindow dockableWindow = windowManager.GetDockableWindow(this.UID);
            return dockableWindow.IsVisible();
        }

        /// <summary>
        ///     Shows the dockable window.
        /// </summary>
        protected void Show()
        {
            IDockableWindowManager windowManager = (IDockableWindowManager) this.Application;
            IDockableWindow dockableWindow = windowManager.GetDockableWindow(this.UID);
            dockableWindow.Show(true);
        }

        #endregion

#if V10
        #region IDockableWindowImageDef
        
        /// <summary>
        ///     The bitmap for the dockable window.
        /// </summary>
        public abstract int Bitmap { get; }
        
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

        #endregion
#endif
    }
}