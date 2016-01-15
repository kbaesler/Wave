using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;

namespace ESRI.ArcGIS.BaseClasses
{
    /// <summary>
    ///     An abstract class used to create a dockable window in ArcMap.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseMxDockableWindow : BaseDockableWindow
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMxDockableWindow" /> class.
        /// </summary>
        /// <param name="name">The name of the window.</param>
        /// <param name="caption">The caption for the window.</param>
        protected BaseMxDockableWindow(string name, string caption)
            : base(name, caption)
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
            MxDockableWindows.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            MxDockableWindows.Unregister(registryKey);
        }

        #endregion
    }
}