﻿using System;
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
        #region Fields

        private static MouseCursorReverter _MouseCursorReverter;

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
            if (source == null) return null;

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
        ///     Returns the command that has the specified <paramref name="commandType" />.
        /// </summary>
        /// <param name="source">The application reference.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>
        ///     Returns the <see cref="ESRI.ArcGIS.Framework.ICommandItem" /> representing the command item; otherwise
        ///     <c>null</c>.
        /// </returns>
        public static ICommandItem GetCommandItem(this IApplication source, Type commandType)
        {
            if (source == null || commandType == null) return null;

            object[] attributes = commandType.GetCustomAttributes(typeof (GuidAttribute), false);
            if (attributes.Length == 0) return null;

            string value = "{" + ((GuidAttribute) attributes[0]).Value + "}";
            return GetCommandItem(source, value);
        }


        /// <summary>
        ///     Gets the dockable window for the given type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="dockableWindowDef">The dockable window def.</param>
        /// <returns>
        ///     Returns the <see cref="ESRI.ArcGIS.Framework.IDockableWindow" /> representing the window; otherwise
        ///     <c>null</c>.
        /// </returns>
        public static IDockableWindow GetDockableWindow(this IApplication source, Type dockableWindowDef)
        {
            if (source == null) return null;

            object[] attributes = dockableWindowDef.GetCustomAttributes(typeof (ProgIdAttribute), false);
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
            return source.FindExtensionByName(ArcMap.Extensions.Name.Editor) as IEditor;
        }

        /// <summary>
        ///     Starts or stops spinning globe in ArcMap.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="play">if set to <c>true</c> if the animation should be shown; otherwise <c>false</c>.</param>
        /// <param name="statusMessage">The message that will appear in the status bar.</param>
        public static void PlayAnimation(this IApplication source, bool play, string statusMessage = null)
        {
            if (source == null || !source.Visible) return;

            if (play)
            {
                _MouseCursorReverter = new MouseCursorReverter(MouseCursorImage.Wait);

                IAnimationProgressor animation = source.StatusBar.ProgressAnimation;
                animation.Show();
            }
            else
            {
                if (_MouseCursorReverter != null)
                {
                    _MouseCursorReverter.Dispose();
                    _MouseCursorReverter = null;
                }

                IAnimationProgressor animation = source.StatusBar.ProgressAnimation;
                animation.Hide();
            }

            source.StatusBar.PlayProgressAnimation(play);
            source.StatusBar.Message[0] = statusMessage;
        }

        #endregion
    }

    /// <summary>
    ///     Provides access to the <see cref="IApplication" /> singleton instance for ArcCatalog.
    /// </summary>
    public static class ArcCatalog
    {
        #region Public Properties

        /// <summary>
        ///     This returns the current running instance of application.
        /// </summary>
        /// <value>The application.</value>
        public static IApplication Application
        {
            get
            {
                try
                {
                    Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
                    object obj = Activator.CreateInstance(type);
                    return (IApplication) obj;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion
    }

    /// <summary>
    ///     Provides access to the <see cref="IApplication" /> singleton instance for ArcMap.
    /// </summary>
    public static class ArcMap
    {
        #region Public Properties

        /// <summary>
        ///     This returns the current running instance of application.
        /// </summary>
        /// <value>The application.</value>
        public static IApplication Application
        {
            get
            {
                try
                {
                    Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
                    object obj = Activator.CreateInstance(type);
                    return (IApplication) obj;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion

        #region Nested Type: Extensions

        /// <summary>
        ///     Container for the <see cref="ESRI.ArcGIS.Framework.ArcMap.Extensions.Name" /> and
        ///     <see cref="ESRI.ArcGIS.Framework.ArcMap.Extensions.Guid" /> classes used
        ///     to identify the common extensions for the ArcFM extensions which may be used for customization.
        /// </summary>
        public static class Extensions
        {
            #region Nested Type: Guid

            /// <summary>
            ///     Represents the common extension GUIDs for the ESRI extensions which may be used for customization.
            /// </summary>
            public static class Guid
            {
                #region Constants

                /// <summary>
                ///     The GUID of the editor extension.
                /// </summary>
                public const string Editor = "{F8842F20-BB23-11D0-802B-0000F8037368}";

                /// <summary>
                ///     The GUID of the network utility analysis extension.
                /// </summary>
                public const string NetworkUtilityAnalysis = "{98528F9B-B971-11D2-BABD-00C04FA33C20}";

                #endregion
            }

            #endregion

            #region Nested Type: Name

            /// <summary>
            ///     Represents the extension names for the ESRI extensions which may be used for customization.
            /// </summary>
            public static class Name
            {
                #region Constants

                /// <summary>
                ///     The name of the editor extension.
                /// </summary>
                public const string Editor = "ESRI Object Editor";

                /// <summary>
                ///     The name of the network utility analysis extension.
                /// </summary>
                public const string NetworkUtilityAnalysis = "Utility Network Analyst";

                #endregion
            }

            #endregion
        }

        #endregion
    }
}