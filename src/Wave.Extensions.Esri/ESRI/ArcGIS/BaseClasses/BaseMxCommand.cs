using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;

namespace ESRI.ArcGIS.ADF.BaseClasses
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
            : this(name, caption, category, message, toolTip, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMxCommand" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="category">The category.</param>
        /// <param name="message">The message.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="bitmap">The bitmap.</param>
        protected BaseMxCommand(string name, string caption, string category, string message, string toolTip, Bitmap bitmap)
        {
            m_name = name;
            m_caption = caption;
            m_category = category;
            m_message = message;
            m_toolTip = toolTip;
            m_bitmap = bitmap;
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
        ///     Called when the user clicks a command.
        /// </summary>
        /// <remarks>
        ///     Note to inheritors: override OnClick and use this method to
        ///     perform the actual work of the custom command.
        /// </remarks>
        public override void OnClick()
        {
            try
            {
                this.InternalClick();
            }
            catch (Exception ex)
            {
                Log.Error(this, ex);
            }
        }

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

        #region Protected Methods

        /// <summary>
        ///     Called when the user clicks a command.
        /// </summary>
        /// <remarks>
        ///     Note to inheritors: override OnClick and use this method to
        ///     perform the actual work of the custom command.
        /// </remarks>
        protected abstract void InternalClick();

        #endregion
    }
}