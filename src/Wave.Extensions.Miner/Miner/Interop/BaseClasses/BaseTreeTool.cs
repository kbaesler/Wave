using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Miner.ComCategories;
using Miner.Framework;

namespace Miner.Interop
{
    /// <summary>
    ///     Provides an abstract implementation for Tree Tools used to execute in ArcMap and ArcFM Viewer.
    /// </summary>
    /// <remarks>
    ///     You must provide your own COM Registration because they can be registered in multiple categories. For instance,
    ///     the <see cref="D8CUTreeViewTool" />, <see cref="D8SelectedCuTreeTool" />, <see cref="D8SelectionTreeTool" /> and/or
    ///     <see cref="D8DesignTreeViewTool" /> categories.
    /// </remarks>
    public abstract class BaseTreeTool : IMMTreeTool, IDisposable
    {
        private static readonly ILog Log = LogProvider.For<BaseTreeTool>();

        #region Fields

        private Bitmap _Bitmap;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseTreeTool" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="category">The category.</param>
        protected BaseTreeTool(string name, int priority, int category)
            : this(name, priority, category, false, mmShortCutKey.mmShortcutNone)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseTreeTool" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="category">The category.</param>
        /// <param name="allowAsDefault">
        ///     if set to <c>true</c> when the menu item as the default action to be performed when the
        ///     user double-clicks the TreeView item.
        /// </param>
        /// <param name="shortCutKey">The short cut key.</param>
        protected BaseTreeTool(string name, int priority, int category, bool allowAsDefault, mmShortCutKey shortCutKey)
        {
            this.Name = name;
            this.Priority = priority;
            this.Category = category;
            this.AllowAsDefault = allowAsDefault;
            this.ShortCut = shortCutKey;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion

        #region IMMTreeTool Members

        /// <summary>
        ///     Gets a value indicating whether the menu item as the default action to be performed when the user double-clicks the
        ///     TreeView item.
        /// </summary>
        /// <value>
        ///     <c>true</c> if there the menu item as the default action to be performed when the user double-clicks the TreeView
        ///     item.; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAsDefault { get; protected set; }

        /// <summary>
        ///     Gets the bitmap.
        /// </summary>
        /// <value>The bitmap.</value>
        public int Bitmap
        {
            get { return (_Bitmap != null) ? _Bitmap.GetHbitmap().ToInt32() : 0; }
        }

        /// <summary>
        ///     Gets the category.
        /// </summary>
        /// <value>
        ///     The category.
        /// </value>
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
        ///     Gets the short cut.
        /// </summary>
        /// <value>The short cut.</value>
        public mmShortCutKey ShortCut { get; protected set; }

        /// <summary>
        ///     Executes the specified tree tool using the selected items.
        /// </summary>
        /// <param name="pEnumItems">The enumeration of selected items.</param>
        /// <param name="lItemCount">The number of items selected.</param>
        public virtual void Execute(ID8EnumListItem pEnumItems, int lItemCount)
        {
            try
            {
                InternalExecute(pEnumItems, lItemCount);
            }
            catch (Exception e)
            {
                Log.Error(this.Name, e);
            }
        }

        /// <summary>
        ///     Returns <c>true</c> if the tool should be enabled for the specified selection of items.
        /// </summary>
        /// <param name="pEnumItems">The enumeration of items.</param>
        /// <param name="lItemCount">The item count.</param>
        /// <returns><c>true</c> if the tool should be enabled; otherwise <c>false</c></returns>
        public virtual int get_Enabled(ID8EnumListItem pEnumItems, int lItemCount)
        {
            try
            {
                return InternalEnabled(pEnumItems, lItemCount);
            }
            catch (Exception e)
            {
                Log.Error(this.Name, e);
            }

            return 0;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_Bitmap != null)
                    _Bitmap.Dispose();
            }
        }

        /// <summary>
        ///     Determines of the tree tool is enabled for the specified selection of items.
        /// </summary>
        /// <param name="enumItems">The enumeration of items.</param>
        /// <param name="itemCount">The item count.</param>
        /// <returns>Returns bitwise flag combination of the <see cref="mmToolState" /> to specify if enabled.</returns>
        protected abstract int InternalEnabled(ID8EnumListItem enumItems, int itemCount);

        /// <summary>
        ///     Executes the tree tool within error handling.
        /// </summary>
        /// <param name="enumItems">The enumeration of items.</param>
        /// <param name="itemCount">The item count.</param>
        protected abstract void InternalExecute(ID8EnumListItem enumItems, int itemCount);

        /// <summary>
        ///     Loads the specified bitmap for command and sets the transparence for the X and Y location.
        /// </summary>
        /// <param name="stream">The stream of the bitmap.</param>
        /// <param name="x">The x pixel that is used to make the transparent color.</param>
        /// <param name="y">The y pixel that is used to make the transparent color.</param>
        protected void UpdateBitmap(Stream stream, int x, int y)
        {
            try
            {
                _Bitmap = new Bitmap(stream);
                _Bitmap.MakeTransparent(_Bitmap.GetPixel(x, y));
            }
            catch (Exception e)
            {
                if (MinerRuntimeEnvironment.IsUserInterfaceSupported)
                    MessageBox.Show(Document.ParentWindow, e.Message, string.Format("Error Updating Bitmap {0}", this.Name), MessageBoxButtons.OK, MessageBoxIcon.Error);

                Log.Error("Error Updating Bitmap " + this.Name, e);
            }
        }

        #endregion
    }
}