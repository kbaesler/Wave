using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Framework;

namespace ESRI.ArcGIS.ADF.BaseClasses
{
    /// <summary>
    ///     An abstract tool that is used in the engine environment.
    /// </summary>
    public abstract class BaseMxTool : BaseTool
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMxTool" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="category">The category.</param>
        /// <param name="message">The message.</param>
        /// <param name="tooltip">The tooltip.</param>
        protected BaseMxTool(string name, string caption, string category, string message, string tooltip)
            : base(null, caption, category, Cursors.Arrow, 0, null, message, name, tooltip)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMxTool" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="category">The category.</param>
        /// <param name="message">The message.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <param name="bitmap">The bitmap.</param>
        protected BaseMxTool(string name, string caption, string category, string message, string tooltip, Bitmap bitmap)
            : base(bitmap, caption, category, Cursors.Arrow, 0, null, message, name, tooltip)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMxTool" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="category">The category.</param>
        /// <param name="message">The message.</param>
        /// <param name="tooltip">The tooltip.</param>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="cursor">The cursor.</param>
        protected BaseMxTool(string name, string caption, string category, string message, string tooltip, Bitmap bitmap, Cursor cursor)
            : base(bitmap, caption, category, cursor, 0, null, message, name, tooltip)
        {
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the application.
        /// </summary>
        /// <value>
        ///     The application.
        /// </value>
        protected IApplication Application { get; set; }

        /// <summary>
        ///     Gets or sets the hook helper.
        /// </summary>
        /// <value>The hook helper.</value>
        protected IHookHelper HookHelper { get; set; }

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
            if (hook is IHookHelper)
                this.HookHelper = hook as IHookHelper;
            else
                this.HookHelper = new HookHelperClass {Hook = hook};

            this.Application = hook as IApplication;
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
            MxCommands.Register(registryKey);
        }

        /// <summary>
        ///     Unregisters the specified registry key.
        /// </summary>
        /// <param name="registryKey">The registry key.</param>
        [ComUnregisterFunction]
        internal static void Unregister(string registryKey)
        {
            MxCommands.Unregister(registryKey);
        }

        #endregion
    }
}