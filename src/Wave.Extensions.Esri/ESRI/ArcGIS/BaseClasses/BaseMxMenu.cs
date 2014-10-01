using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Framework;

namespace ESRI.ArcGIS.BaseClasses
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
    }
}