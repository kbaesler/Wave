using System;
using System.Diagnostics.CodeAnalysis;

namespace ESRI.ArcGIS.Framework
{
    /// <summary>
    ///     Provides access to the <see cref="IApplication" /> singleton instance for ArcCatalog.
    /// </summary>
    public static class ArcCatalog
    {
        #region Fields

        private static IApplication _Application;

        #endregion

        #region Public Properties

        /// <summary>
        ///     This returns the current running instance of application.
        /// </summary>
        /// <value>The application.</value>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static IApplication Application
        {
            get
            {
                try
                {
                    if (_Application == null)
                    {
                        Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
                        object obj = Activator.CreateInstance(type);
                        _Application = (IApplication) obj;
                    }

                    return _Application;
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