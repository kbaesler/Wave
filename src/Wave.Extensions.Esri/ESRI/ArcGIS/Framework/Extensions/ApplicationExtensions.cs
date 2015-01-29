using System;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Framework
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Framework.IApplication" /> interface.
    /// </summary>
    public static class ApplicationExtensions
    {
        private static ProgressAnimationMonitor _ProgressAnimationMonitor;

        #region Public Methods

        /// <summary>
        ///     Returns the reference to the map object that currently has focus.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IMap" /> representing the map; otherwise <c>null</c>
        /// </returns>
        /// <remarks>
        ///     In ArcMap, the focus map is the one visible in data view and the one selected in layout view.
        /// </remarks>
        public static IMap GetActiveMap(this IApplication source)
        {
            if (source == null)
                return null;

            IMxDocument document = source.Document as IMxDocument;
            if (document != null)
            {
                return document.FocusMap;
            }

            return null;
        }

        /// <summary>
        ///     Returns the command that has the specified <paramref name="commandName" />.
        /// </summary>
        /// <param name="source">The application reference.</param>
        /// <param name="commandName">Name of the command.</param>
        /// <returns>
        ///     Returns the <see cref="ESRI.ArcGIS.Framework.ICommandItem" /> representing the command item; otherwise
        ///     <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentException">The application framework has not been fully initialized.</exception>
        public static ICommandItem GetCommandItem(this IApplication source, string commandName)
        {
            if (source == null)
                return null;

            var status = (IApplicationStatus) source;
            if (!status.Initialized)
                throw new ArgumentException("The application framework has not been fully initialized.");

            ICommandBars commandBars = source.Document.CommandBars;
            UID uid = new UID();
            uid.Value = commandName;
            ICommandItem commandItem = commandBars.Find(uid, false, true);
            return commandItem;
        }

        /// <summary>
        ///     Returns the command that has the specified <paramref name="type" />.
        /// </summary>
        /// <param name="source">The application reference.</param>
        /// <param name="type">Type of the command.</param>
        /// <returns>
        ///     Returns the <see cref="ESRI.ArcGIS.Framework.ICommandItem" /> representing the command item; otherwise
        ///     <c>null</c>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static ICommandItem GetCommandItem(this IApplication source, Type type)
        {
            if (source == null) return null;

            if (type == null)
                throw new ArgumentNullException("type");

            object[] attributes = type.GetCustomAttributes(typeof (GuidAttribute), false);
            if (attributes.Length == 0) return null;

            string value = "{" + ((GuidAttribute) attributes[0]).Value + "}";
            return GetCommandItem(source, value);
        }


        /// <summary>
        ///     Gets the dockable window for the given type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The dockable window type.</param>
        /// <returns>
        ///     Returns the <see cref="ESRI.ArcGIS.Framework.IDockableWindow" /> representing the window; otherwise
        ///     <c>null</c>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dockable")]
        public static IDockableWindow GetDockableWindow(this IApplication source, Type type)
        {
            if (source == null)
                return null;

            if (type == null)
                throw new ArgumentNullException("type");

            object[] attributes = type.GetCustomAttributes(typeof (ProgIdAttribute), false);
            if (attributes.Length == 0) return null;

            UID uid = new UID();
            uid.Value = ((ProgIdAttribute) attributes[0]).Value;

            var windowManager = source as IDockableWindowManager;
            if (windowManager == null) return null;

            return windowManager.GetDockableWindow(uid);
        }

        /// <summary>
        ///     Returns the <see cref="IEditor" /> interface used for edit operations within ArcMap.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IEditor" /> representing the the editor object.
        /// </returns>
        public static IEditor GetEditor(this IApplication source)
        {
            if (source == null)
                return null;

            return source.FindExtensionByName(ArcMap.Extensions.Name.Editor) as IEditor;
        }

        /// <summary>
        /// Starts spinning globe in ArcMap.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// Returns a <see cref="IDisposable" /> representing the a disposable object that will stop the spinning globle
        /// when disposed.
        /// </returns>
        public static IDisposable PlayAnimation(this IApplication source)
        {
            return source.PlayAnimation(null);
        }

        /// <summary>
        ///     Starts spinning globe in ArcMap and updates the status bar message.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="statusMessage">The status message.</param>
        /// <returns>
        ///     Returns a <see cref="IDisposable" /> representing the a disposable object that will stop the spinning globle
        ///     when disposed.
        /// </returns>
        public static IDisposable PlayAnimation(this IApplication source, string statusMessage)
        {
            if (source == null)
                return null;

            if(_ProgressAnimationMonitor != null)
                _ProgressAnimationMonitor.Dispose();

            _ProgressAnimationMonitor = new ProgressAnimationMonitor(source);
            _ProgressAnimationMonitor.Play(MouseCursorImage.Wait, statusMessage);

            return _ProgressAnimationMonitor;
        }

        #endregion
    }

    /// <summary>
    ///     An internal class used to handle starting and stopping the ArcMap progress animation.
    /// </summary>
    internal class ProgressAnimationMonitor : IDisposable
    {
        #region Fields

        private readonly IApplication _Application;
        private MouseCursorReverter _MouseCursorReverter;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressAnimationMonitor" /> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public ProgressAnimationMonitor(IApplication application)
        {
            _Application = application;
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

        #region Public Methods

        /// <summary>
        ///     Starts the global spinning in ArcMap and updates the message on the status bar.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        /// <param name="statusMessage">The status message.</param>
        public void Play(MouseCursorImage cursor, string statusMessage)
        {
            _MouseCursorReverter = new MouseCursorReverter(cursor);

            IAnimationProgressor animation = _Application.StatusBar.ProgressAnimation;
            animation.Show();

            _Application.StatusBar.PlayProgressAnimation(true);
            _Application.StatusBar.Message[0] = statusMessage;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_MouseCursorReverter != null)
                {
                    _MouseCursorReverter.Dispose();
                }

                _Application.StatusBar.PlayProgressAnimation(false);
                _Application.StatusBar.Message[0] = null;

                IAnimationProgressor animation = _Application.StatusBar.ProgressAnimation;
                animation.Hide();
            }
        }

        #endregion
    }    
}