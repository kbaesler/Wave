using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.BaseClasses;

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
    }
}