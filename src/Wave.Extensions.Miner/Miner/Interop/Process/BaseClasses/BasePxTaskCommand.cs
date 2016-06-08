using System;
using System.Diagnostics;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Framework;

using Miner.Framework;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     An abstract ArcMap command that handles executing a Process Framework Task when the button is clicked and is
    ///     enabled or disabled based on the task enabled method.
    /// </summary>
    public abstract class BasePxTaskCommand : BaseMxCommand
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxTaskCommand" /> class.
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="name">The name.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="category">The category.</param>
        /// <param name="message">The message.</param>
        /// <param name="toolTip">The tool tip.</param>
        protected BasePxTaskCommand(string taskName, string name, string caption, string category, string message, string toolTip)
            : base(name, caption, category, message, toolTip)
        {
            this.TaskName = taskName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The enabled state of this command, determines whether the command is usable.
        /// </summary>
        public override bool Enabled
        {
            get
            {
                IMMPxApplication pxApp = this.Application.GetPxApplication();
                if (pxApp == null)
                    return false;

                IMMPxNode node = pxApp.GetCurrentNode();
                if (node == null)
                    return false;

                IMMPxTask task = node.GetTask(this.TaskName, false);
                if (task == null)
                    return false;

                return task.Enabled[node];
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the name of the task.
        /// </summary>
        /// <value>
        ///     The name of the task.
        /// </value>
        protected string TaskName { get; set; }

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
                IMMPxApplication pxApp = this.Application.GetPxApplication();
                IMMPxNode node = pxApp.GetCurrentNode();
                IMMPxTask task = node.GetTask(this.TaskName, true);
                task.Execute(node);
            }
            catch (Exception ex)
            {
                Log.Error(this, Document.ParentWindow, this.Caption, ex);
            }
        }

        #endregion
    }
}