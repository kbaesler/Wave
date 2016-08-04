using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

namespace ESRI.ArcGIS.ADF.BaseClasses
{
    /// <summary>
    ///     Provides access to IFinder interface for the MxFind routine. Implement this interface to create a custom find dialog page.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseFinder : IFinder
    {
        #region Fields

        private readonly string _Name;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseFinder" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected BaseFinder(string name)
        {
            _Name = name;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the application.
        /// </summary>
        /// <value>
        ///     The application.
        /// </value>
        protected IApplication Application { get; set; }

        /// <summary>
        ///     Gets or sets the find events call back.
        /// </summary>
        /// <value>
        ///     The find events call back.
        /// </value>
        protected IFindPanelEvents FindEventsCallBack { get; set; }

        #endregion

        #region IFinder Members

        /// <summary>
        ///     Number of columns to display in list box.
        /// </summary>
        public abstract int ColumnCount { get; }

        /// <summary>
        ///     Perform find functionality.
        /// </summary>
        /// <param name="pFindCallBack"></param>
        public virtual void Find(IFindCallBack pFindCallBack)
        {
            try
            {
                this.InternalFind(pFindCallBack);
            }
            catch (Exception e)
            {
                Log.Error(this, NativeWindow.FromHandle(new IntPtr(Application.hWnd)), this.Name, e);
            }
        }

        /// <summary>
        ///     Initializes the control.
        /// </summary>
        /// <param name="pApplication">The application.</param>
        /// <param name="pFindEventsCallBack">The find events call back.</param>
        public virtual void InitializeControl(AppRef pApplication, IFindPanelEvents pFindEventsCallBack)
        {
            try
            {
                this.Application = pApplication;
                this.FindEventsCallBack = pFindEventsCallBack;

                this.InitializeControl();
                this.NewSearch();
            }
            catch (Exception e)
            {
                Log.Error(this, NativeWindow.FromHandle(new IntPtr(Application.hWnd)), this.Name, e);
            }
        }

        /// <summary>
        ///     UID of menu to popup in list box.
        /// </summary>
        public abstract UID MenuUID { get; }

        /// <summary>
        ///     The control name. Used for the FindUI tab.
        /// </summary>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        ///     New search. Clear control input boxes.
        /// </summary>
        public abstract void NewSearch();

        /// <summary>
        ///     User requested find to stop.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        ///     Called whenever ArcMap status changes.
        /// </summary>
        public abstract void UpdateControl();

        /// <summary>
        ///     The column name.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public abstract string get_ColumnName(int column);

        /// <summary>
        ///     The column width in Dialog Units (1/4 of avg. char width).
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public abstract int get_ColumnWidth(int column);

        /// <summary>
        ///     The window handle for the finder.
        /// </summary>
        public abstract int hWnd { get; }

        #endregion

        #region Internal Methods

        [ComRegisterFunction]
        internal static void Register(string regKey)
        {
            MxFinders.Register(regKey);
        }

        [ComUnregisterFunction]
        internal static void Unregister(string regKey)
        {
            MxFinders.Unregister(regKey);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Initializes the control.
        /// </summary>
        protected abstract void InitializeControl();

        /// <summary>
        ///     Perform find functionality.
        /// </summary>
        /// <param name="findCallBack">The find call back.</param>
        protected abstract void InternalFind(IFindCallBack findCallBack);

        #endregion
    }
}