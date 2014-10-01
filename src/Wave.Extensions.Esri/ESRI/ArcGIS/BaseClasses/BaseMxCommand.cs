using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Framework;

namespace ESRI.ArcGIS.BaseClasses
{
    /// <summary>
    ///     An abstract class used to create a buttons in ArcMap.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseMxCommand : BaseCommand
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMxCommand" /> class.
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="caption">The caption</param>
        /// <param name="category">The category.</param>
        /// <param name="message">The message</param>
        /// <param name="toolTip">The tool tip</param>
        protected BaseMxCommand(string name, string caption, string category, string message, string toolTip)
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
        ///     Gets the application.
        /// </summary>
        protected IApplication Application { get; private set; }

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
        public override void OnCreate(object hook)
        {
            this.Application = (IApplication) hook;
        }

        #endregion
    }
}