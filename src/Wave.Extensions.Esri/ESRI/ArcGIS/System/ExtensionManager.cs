using System;
using System.Collections.Generic;

namespace ESRI.ArcGIS.esriSystem
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

        #region Public Methods

        /// <summary>
        ///     Creates an enumeration of the <see cref="IExtension" /> objects.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Returns a <see cref="IEnumerable{IExtension}" /> representing the extensions.</returns>
        public static IEnumerable<IExtension> AsEnumerable(this IExtensionManager source)
        {
            for (int i = 0; i < source.ExtensionCount; i++)
            {
                yield return source.Extension[i];
            }
        }

        #endregion
    }
}