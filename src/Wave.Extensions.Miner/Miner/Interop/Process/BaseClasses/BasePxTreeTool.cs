using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.ADF.COMSupport;

using Miner.Framework;

using stdole;

namespace Miner.Interop.Process
{
    /// <summary>
    ///     Base class for Tree Tools used to execute Px Tasks in either WorkflowManager or SessionManager.
    ///     You must provide your own COM Registration because they can be registered in multiple categories.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Px")]
    [ComVisible(true)]
    public abstract class BasePxTreeTool : IMMTreeViewTool, IMMTreeViewToolEx, IMMTreeViewTool2, IMMTreeViewToolEx2
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BasePxTreeTool" /> class.
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="category">The category.</param>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="extensionName">Name of the extension. (i.e MMSessionManager or MMWorkflowManager)</param>
        protected BasePxTreeTool(string taskName, int priority, int category, string categoryName, string extensionName)
        {
            this.Name = taskName;
            this.Priority = priority;
            this.Category = category;
            this.CategoryName = categoryName;
            this.ExtensionName = extensionName;
            this.ToolTip = taskName;
            this.TaskName = taskName;
            this.SubCategory = 0;
            this.AllowAsDefaultTool = false;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the process application reference.
        /// </summary>
        /// <value>
        ///     The process application reference.
        /// </value>
        protected IMMPxApplication PxApplication { get; set; }

        /// <summary>
        ///     Gets the name of the task.
        /// </summary>
        /// <value>
        ///     The name of the task.
        /// </value>
        protected string TaskName { get; set; }

        #endregion

        #region IMMTreeViewTool Members

        /// <summary>
        ///     Gets the category.
        /// </summary>
        /// <value>The category.</value>
        public int Category { get; protected set; }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; protected set; }

        /// <summary>
        ///     Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority { get; protected set; }

        /// <summary>
        ///     Executes the specified tree tool using the selected items.
        /// </summary>
        /// <param name="pSelection">The selection.</param>
        public void Execute(IMMTreeViewSelection pSelection)
        {
            try
            {
                this.InternalExecute(pSelection);
            }
            catch (Exception e)
            {
                Log.Error(this, "Error Executing Tree Tool " + this.Name, e);
            }
        }

        /// <summary>
        ///     Returns <c>true</c> if the tool should be enabled for the specified selection of items.
        /// </summary>
        /// <param name="pSelection">The selection.</param>
        /// <returns>
        ///     <c>true</c> if the tool should be enabled; otherwise <c>false</c>
        /// </returns>
        public int get_Enabled(IMMTreeViewSelection pSelection)
        {
            try
            {
                return this.InternalEnabled(pSelection);
            }
            catch (Exception e)
            {
                Log.Error(this, "Error Enabling Tree Tool " + this.Name, e);
            }

            return 0;
        }

        #endregion

        #region IMMTreeViewTool2 Members

        /// <summary>
        ///     Gets a value indicating whether there are allow as default tool.
        /// </summary>
        /// <value>
        ///     <c>true</c> if there are allow as default tool; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAsDefaultTool { get; protected set; }

        #endregion

        #region IMMTreeViewToolEx Members

        /// <summary>
        ///     Gets the bitmap.
        /// </summary>
        /// <value>The bitmap.</value>
        public IPictureDisp Bitmap { get; protected set; }

        /// <summary>
        ///     Gets the name of the category.
        /// </summary>
        /// <value>The name of the category.</value>
        public string CategoryName { get; protected set; }

        /// <summary>
        ///     Gets the sub category.
        /// </summary>
        /// <value>The sub category.</value>
        public int SubCategory { get; protected set; }

        /// <summary>
        ///     Gets the tool tip.
        /// </summary>
        /// <value>The tool tip.</value>
        public string ToolTip { get; protected set; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="BasePxTreeTool" /> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        public virtual bool Checked { get; protected set; }

        /// <summary>
        ///     Returns <c>true</c> if the tool should be enabled for the specified selection of items.
        /// </summary>
        /// <param name="vSelection">The selection.</param>
        /// <returns>
        ///     <c>true</c> if the tool should be enabled; otherwise <c>false</c>
        /// </returns>
        public int EnabledEx(object vSelection)
        {
            return get_Enabled((IMMTreeViewSelection) vSelection);
        }

        /// <summary>
        ///     Executes the specified tree tool using the selected items.
        /// </summary>
        /// <param name="vSelection">The selection.</param>
        /// <returns><c>true</c> if the tool executed; otherwise <c>false</c>.</returns>
        public bool ExecuteEx(object vSelection)
        {
            this.Execute((IMMTreeViewSelection) vSelection);
            return true;
        }

        /// <summary>
        ///     Initializes the specified v init data.
        /// </summary>
        /// <param name="vInitData">The v init data.</param>
        public void Initialize(object vInitData)
        {
            this.PxApplication = vInitData as IMMPxApplication;
        }

        #endregion

        #region IMMTreeViewToolEx2 Members

        /// <summary>
        ///     Gets the name of the extension.
        /// </summary>
        /// <value>The name of the extension.</value>
        public string ExtensionName { get; protected set; }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Determines of the tree tool is enabled for the specified selection of items.
        /// </summary>
        /// <param name="selection">The selection.</param>
        /// <returns>
        ///     Returns bitwise flag combination of the <see cref="mmToolState" /> to specify if enabled.
        /// </returns>
        protected virtual int InternalEnabled(IMMTreeViewSelection selection)
        {
            if (selection == null)
                return 0;

            if (selection.Count != 1)
                return 0;

            selection.Reset();
            IMMPxNode node = (IMMPxNode) selection.Next;
            IMMPxTask task = ((IMMPxNode3) node).GetTaskByName(this.Name);

            if (task == null)
                return 0;

            if (task.get_Enabled(node))
                return 3;

            return 0;
        }

        /// <summary>
        ///     Executes the tree tool within error handling.
        /// </summary>
        /// <param name="selection">The selection.</param>
        protected virtual void InternalExecute(IMMTreeViewSelection selection)
        {
            // Only enable if 1 item is selected.
            if (selection == null || selection.Count != 1) return;

            // Execute the Task for the specified node.
            selection.Reset();
            IMMPxNode node = (IMMPxNode) selection.Next;
            IMMPxTask task = ((IMMPxNode3) node).GetTaskByName(this.Name);
            if (task == null) return;

            task.Execute(node);
        }

        /// <summary>
        ///     Updates the bitmap image with the image from the <paramref name="stream" />.
        /// </summary>
        /// <param name="stream">Stream of bitmap to load.</param>
        protected void UpdateBitmap(Stream stream)
        {
            try
            {
                Bitmap bitmap = new Bitmap(stream);
                bitmap.MakeTransparent(bitmap.GetPixel(0, 0));

                this.Bitmap = OLE.GetIPictureDispFromBitmap(bitmap) as IPictureDisp;
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    Log.Error(this, Document.ParentWindow, "Error Updating Bitmap " + this.Name, e);
                else
                    Log.Error(this, "Error Updating Bitmap " + this.Name, e);
            }
        }

        /// <summary>
        ///     Updates the bitmap image with the image from the <paramref name="bitmap" />.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        protected void UpdateBitmap(Bitmap bitmap)
        {
            try
            {
                this.Bitmap = OLE.GetIPictureDispFromBitmap(bitmap) as IPictureDisp;
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    Log.Error(this, Document.ParentWindow, "Error Updating Bitmap " + this.Name, e);
                else
                    Log.Error(this, "Error Updating Bitmap " + this.Name, e);
            }
        }

        #endregion
    }
}