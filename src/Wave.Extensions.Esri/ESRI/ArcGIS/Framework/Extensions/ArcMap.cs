using System;

namespace ESRI.ArcGIS.Framework
{
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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
        internal static class Extensions
        {
            #region Nested Type: Guid

            /// <summary>
            ///     Represents the common extension GUIDs for the ESRI extensions which may be used for customization.
            /// </summary>
            internal static class Guid
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
            internal static class Name
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