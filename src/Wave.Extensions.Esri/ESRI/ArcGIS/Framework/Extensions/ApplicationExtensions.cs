using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework.Internal;

namespace ESRI.ArcGIS.Framework
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ESRI.ArcGIS.Framework.IApplication" /> interface.
    /// </summary>
    public static class ApplicationExtensions
    {
        #region Fields

        private static ProgressBarAnimation _ProgressBarAnimation;
        private static ProgressGlobeAnimation _ProgressGlobeAnimation;

        #endregion

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
        /// Returns the command that has the specified <paramref name="commandName" />.
        /// </summary>
        /// <param name="source">The application reference.</param>
        /// <param name="commandName">Name of the command.</param>
        /// <returns>
        /// Returns the <see cref="ESRI.ArcGIS.Framework.ICommandItem" /> representing the command item; otherwise
        /// <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">commandName</exception>
        /// <exception cref="System.ArgumentException">The application framework has not been fully initialized.</exception>
        public static ICommandItem GetCommandItem(this IApplication source, string commandName)
        {
            if (source == null) return null;
            if (commandName == null) throw new ArgumentNullException("commandName");

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
        /// Returns the command that has the specified <paramref name="type" />.
        /// </summary>
        /// <param name="source">The application reference.</param>
        /// <param name="type">Type of the command.</param>
        /// <returns>
        /// Returns the <see cref="ESRI.ArcGIS.Framework.ICommandItem" /> representing the command item; otherwise
        /// <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static ICommandItem GetCommandItem(this IApplication source, Type type)
        {
            if (source == null) return null;
            if (type == null) throw new ArgumentNullException("type");

            object[] attributes = type.GetCustomAttributes(typeof (GuidAttribute), false);
            if (attributes.Length == 0) return null;

            string value = "{" + ((GuidAttribute) attributes[0]).Value + "}";
            return GetCommandItem(source, value);
        }


        /// <summary>
        /// Gets the dockable window for the given type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="type">The dockable window type.</param>
        /// <returns>
        /// Returns the <see cref="ESRI.ArcGIS.Framework.IDockableWindow" /> representing the window; otherwise
        /// <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"), SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dockable")]
        public static IDockableWindow GetDockableWindow(this IApplication source, Type type)
        {
            if (source == null) return null;
            if (type == null) throw new ArgumentNullException("type");

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
        ///     Initializes the animation of the progress bar.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="position">The position.</param>
        /// <param name="step">The step.</param>
        /// <returns>
        ///     Returns a <see cref="IProgressBarAnimation" /> representing the object that controls the actions of the progress
        ///     bar.
        /// </returns>
        public static IProgressBarAnimation InitializeAnimation(this IApplication source, int min, int max, int position = 0, int step = 1)
        {
            if(source == null) return null;

            if (_ProgressBarAnimation != null)
                _ProgressBarAnimation.Dispose();

            _ProgressBarAnimation = new ProgressBarAnimation(source);
            _ProgressBarAnimation.Initialize(min, max, position, step);

            return _ProgressBarAnimation;
        }

        /// <summary>
        ///     Starts spinning globe in ArcMap.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        ///     Returns a <see cref="IDisposable" /> representing the disposable object that will stop the spinning globle
        ///     when disposed.
        /// </returns>
        public static IDisposable PlayAnimation(this IApplication source)
        {
            if(source == null) return null;
            
            return source.PlayAnimation(null);
        }

        /// <summary>
        ///     Starts spinning globe in ArcMap and updates the status bar message.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="statusMessage">The status message.</param>
        /// <returns>
        ///     Returns a <see cref="IDisposable" /> representing the disposable object that will stop the spinning globle
        ///     when disposed.
        /// </returns>
        public static IDisposable PlayAnimation(this IApplication source, string statusMessage)
        {
            if (source == null)
                return null;

            if (_ProgressGlobeAnimation != null)
                _ProgressGlobeAnimation.Dispose();

            _ProgressGlobeAnimation = new ProgressGlobeAnimation(source);
            _ProgressGlobeAnimation.Play(MouseCursorImage.Wait, statusMessage);

            return _ProgressGlobeAnimation;
        }

        #endregion
    }
}