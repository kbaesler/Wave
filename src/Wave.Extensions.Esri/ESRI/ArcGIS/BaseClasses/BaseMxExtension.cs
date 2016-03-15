using System;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.BaseClasses
{    
    /// <summary>
    ///     An abstract extension for running within ArcGIS Engine or Desktop.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseMxExtensionJIT : BaseExtension
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MxExtensionJIT" /> class.
        /// </summary>
        /// <param name="extensionName">Name of the extension.</param>
        protected BaseMxExtensionJIT(string extensionName)
            : base(extensionName)
        {            
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Registers the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComRegisterFunction]
        internal static void Register(string registryKey)
        {
            MxExtensionJIT.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            MxExtensionJIT.Unregister(registryKey);
        }

        #endregion
    }
}