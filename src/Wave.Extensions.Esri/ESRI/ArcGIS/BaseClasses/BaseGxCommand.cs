using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.CatalogUI;

namespace ESRI.ArcGIS.BaseClasses
{
    /// <summary>
    ///     An abstract class used to create a buttons in ArcCatalog.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseGxCommand : BaseCommand
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseGxCommand" /> class.
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="caption">The caption</param>
        /// <param name="category"></param>
        /// <param name="message">The message</param>
        /// <param name="toolTip">The tool tip</param>
        protected BaseGxCommand(string name, string caption, string category, string message, string toolTip)
        {
            m_name = name;
            m_caption = caption;
            m_category = category;
            m_message = message;
            m_toolTip = toolTip;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the application reference.
        /// </summary>
        protected IGxApplication GxApplication { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Called when the command is created inside the application.
        /// </summary>
        /// <param name="hook">
        ///     A reference to the application in which the command was created.
        ///     The hook may be an IApplication reference (for commands created in ArcGIS Desktop applications)
        ///     or an IHookHelper reference (for commands created on an Engine ToolbarControl).
        /// </param>
        /// <remarks>
        ///     Note to inheritors: classes inheriting from BaseCommand must always
        ///     override the OnCreate method. Use this method to store a reference to the host
        ///     application, passed in via the hook parameter.
        /// </remarks>
        public override void OnCreate(object hook)
        {
            this.GxApplication = (IGxApplication) hook;
        }

        #endregion
    }
}