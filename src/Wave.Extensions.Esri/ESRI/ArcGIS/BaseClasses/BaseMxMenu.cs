using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;

namespace ESRI.ArcGIS.ADF.BaseClasses
{
    /// <summary>
    ///     An abstract ArcMap Menu.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseMxMenu : BaseMenu, IRootLevelMenu
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMxMenu" /> class.
        /// </summary>
        /// <param name="caption">The caption </param>
        /// <param name="name">The name </param>
        protected BaseMxMenu(string caption, string name)
        {
            m_barCaption = caption;
            m_barID = name;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMxMenu" /> class.
        /// </summary>
        protected BaseMxMenu()
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