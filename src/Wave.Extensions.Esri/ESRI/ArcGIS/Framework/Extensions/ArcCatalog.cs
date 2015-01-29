using System;

namespace ESRI.ArcGIS.Framework
{
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
    }
}