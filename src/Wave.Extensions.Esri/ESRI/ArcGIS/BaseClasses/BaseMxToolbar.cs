using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;

namespace ESRI.ArcGIS.BaseClasses
{
    /// <summary>
    ///     An abstract ArcMap Toolbar.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseMxToolbar : BaseToolbar
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMxToolbar" /> class.
        /// </summary>
        /// <param name="caption">The caption </param>
        /// <param name="name">The name </param>
        protected BaseMxToolbar(string caption, string name)
        {
            m_barCaption = caption;
            m_barID = name;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMxToolbar" /> class.
        /// </summary>
        protected BaseMxToolbar()
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
            MxCommandBars.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            MxCommandBars.Unregister(registryKey);
        }

        #endregion
    }
}