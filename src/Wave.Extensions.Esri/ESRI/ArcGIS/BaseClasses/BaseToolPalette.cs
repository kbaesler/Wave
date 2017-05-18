using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.SystemUI;

namespace ESRI.ArcGIS.ADF.BaseClasses
{
    /// <summary>
    ///     An abstract implementation of a tool palette.
    /// </summary>
    [ComVisible(true)]
    public abstract class BaseToolPalette : BaseMxCommand, IToolPalette
    {
        #region Fields

        private readonly List<string> _Items = new List<string>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseToolPalette" /> class.
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="caption">The caption</param>
        /// <param name="category">The category.</param>
        /// <param name="message">The message</param>
        /// <param name="toolTip">The tool tip</param>
        protected BaseToolPalette(string name, string caption, string category, string message, string toolTip)
            : this(name, caption, category, message, toolTip, true, 1, true)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseToolPalette" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="category">The category.</param>
        /// <param name="message">The message.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="menuStyle">if set to <c>true</c> for menu style otherwise tile style.</param>
        /// <param name="paletteColumns">The number of columns to display.</param>
        /// <param name="tearOff">
        ///     if set to <c>true</c> the tool palette can be torn off by clicking on the drop-down button
        ///     and drag the palette, and the tool palette becomes a seperate toolbar.
        /// </param>
        protected BaseToolPalette(string name, string caption, string category, string message, string toolTip, bool menuStyle, int paletteColumns, bool tearOff)
            : base(name, caption, category, message, toolTip)
        {
            this.MenuStyle = menuStyle;
            this.PaletteColumns = paletteColumns;
            this.TearOff = tearOff;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The number of items in this menu.
        /// </summary>
        public int PaletteItemCount
        {
            get { return _Items.Count; }
        }

        /// <summary>
        ///     The menu style
        /// </summary>
        public bool MenuStyle { get; protected set; }

        /// <summary>
        ///     The Number of Columns to display
        /// </summary>
        public int PaletteColumns { get; protected set; }

        /// <summary>
        ///     The tearoff style
        /// </summary>
        /// <remarks>
        ///     When this property is set to true, the tool palette can be torn off by clicking on the drop-down button
        ///     and drag the palette, and the tool palette becomes a seperate toolbar.
        /// </remarks>
        public bool TearOff { get; protected set; }

        #endregion

        #region IToolPalette Members

        /// <summary>
        ///     Gets the CLSID for the item on this menu at the specified index.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns>
        ///     Returns a <see cref="string" /> representing the CLSID for the item on this menu at the specified index.
        /// </returns>
        public string get_PaletteItem(int pos)
        {
            return _Items[pos];
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Add a command item to the tool palette by the Guid.
        /// </summary>
        /// <param name="guid">The Guid of the command item</param>
        /// <remarks>
        ///     Note to inheritors: Call this method to add an item to
        ///     your tool palette definition. You should define your tool palette
        ///     in the constructor.
        /// </remarks>
        protected void AddItem(Guid guid)
        {
            this.AddItem(guid.ToString("B"));
        }

        /// <summary>
        ///     Add a command item to the tool palette by a type.
        /// </summary>
        /// <param name="type">The type which implements the command item.</param>
        /// <remarks>
        ///     Note to inheritors: Call this method to add an item to
        ///     your tool palette definition. You should define your tool palette
        ///     in the constructor.
        /// </remarks>
        protected void AddItem(Type type)
        {
            this.AddItem(type.GUID);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Add a command item to the tool palette by an Unique Identifier Object (UID).
        /// </summary>
        /// <param name="uid">The UID of the command item</param>
        /// <remarks>
        ///     Note to inheritors: Call this method to add an item to
        ///     your tool palette definition. You should define your tool palette
        ///     in the constructor.
        /// </remarks>
        private void AddItem(string uid)
        {
            _Items.Add(uid);
        }

        #endregion
    }
}