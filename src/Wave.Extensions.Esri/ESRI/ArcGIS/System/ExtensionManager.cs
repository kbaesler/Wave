using System;

using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.ESRI.ArcGIS.System
{
    /// <summary>
    ///     Provides accessor to the <see cref="IExtensionManager" /> interface.
    /// </summary>
    public static class ExtensionManager
    {
        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static IExtensionManager Instance
        {
            get
            {
                Type type = Type.GetTypeFromProgID("esriSystem.ExtensionManager");
                object obj = Activator.CreateInstance(type);
                IExtensionManager extensionManager = (IExtensionManager) obj;
                return extensionManager;
            }
        }

        #endregion
    }
}