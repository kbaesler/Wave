using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace System.Forms.Controls
{
    /// <summary>
    ///     Represents a control which provides ComboBox-like functionality, displaying its
    ///     dropdown items (nodes) in a manner similar to a TreeView control.
    /// </summary>
    /// <remarks>http://www.brad-smith.info/blog/archives/193</remarks>
    [ToolboxItem(true)]
    public class ComboTreeBox : BaseDropDownControl
    {
        #region Constants

        private const TextFormatFlags FormatFlags = TextFormatFlags.TextBoxControl | TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.PathEllipsis;

        #endregion

        #region Fields

        private readonly ComboTreeDropDown _DropDown;
        private readonly ComboTreeNodeCollection _Nodes;

        private int _ExpandedImageIndex;
        private string _ExpandedImageKey;
        private int _ImageIndex;
        private string _ImageKey;
        private ImageList _Images;
        private bool _IsUpdating;
        private string _NullValue;
        private string _PathSeparator;
        private ComboTreeNode _SelectedNode;
        private bool _ShowPath;
        private bool _UseNodeNamesForPath;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initalises a new instance of ComboTreeBox.
        /// </summary>
        public ComboTreeBox()
        {
            // default property values
            _NullValue = String.Empty;
            _PathSeparator = @"\";
            _ExpandedImageIndex = _ImageIndex = 0;
            _ExpandedImageKey = _ImageKey = String.Empty;

            // nodes collection
            _Nodes = new ComboTreeNodeCollection(null);
            _Nodes.CollectionChanged += nodes_CollectionChanged;

            // dropdown portion
            _DropDown = new ComboTreeDropDown(this);
            _DropDown.Opened += dropDown_Opened;
            _DropDown.Closed += dropDown_Closed;
            _DropDown.UpdateVisibleItems();
        }

        #endregion

        #region Events

        /// <summary>
        ///     Fired when the SelectedNode property changes.
        /// </summary>
        [Description("Occurs when the SelectedNode property changes.")]
        public event EventHandler SelectedNodeChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the (recursive) superset of the entire tree of nodes contained within the control.
        /// </summary>
        [Browsable(false)]
        public IEnumerable<ComboTreeNode> AllNodes
        {
            get
            {
                IEnumerator<ComboTreeNode> e = GetNodesRecursive(_Nodes, false);
                while (e.MoveNext()) yield return e.Current;
            }
        }

        /// <summary>
        ///     Gets or sets the height of the dropdown portion of the control.
        /// </summary>
        [DefaultValue(150),
         Description("The height of the dropdown portion of the control."),
         Category("Behavior")]
        public int DropDownHeight
        {
            get { return _DropDown.DropDownHeight; }
            set { _DropDown.DropDownHeight = value; }
        }

        /// <summary>
        ///     Gets or sets the index of the default image to use for nodes when expanded.
        /// </summary>
        [DefaultValue(0),
         Description("The index of the default image to use for nodes when expanded."),
         Category("Appearance")]
        public int ExpandedImageIndex
        {
            get { return _ExpandedImageIndex; }
            set
            {
                _ExpandedImageIndex = value;
                _DropDown.UpdateVisibleItems();
            }
        }

        /// <summary>
        ///     Gets or sets the name of the default image to use for nodes when expanded.
        /// </summary>
        [DefaultValue(""),
         Description("The name of the default image to use for nodes when expanded."),
         Category("Appearance")]
        public string ExpandedImageKey
        {
            get { return _ExpandedImageKey; }
            set
            {
                _ExpandedImageKey = value;
                _DropDown.UpdateVisibleItems();
            }
        }

        /// <summary>
        ///     Gets or sets the index of the default image to use for nodes.
        /// </summary>
        [DefaultValue(0),
         Description("The index of the default image to use for nodes."),
         Category("Appearance")]
        public int ImageIndex
        {
            get { return _ImageIndex; }
            set
            {
                _ImageIndex = value;
                _DropDown.UpdateVisibleItems();
            }
        }

        /// <summary>
        ///     Gets or sets the name of the default image to use for nodes.
        /// </summary>
        [DefaultValue(""),
         Description("The name of the default image to use for nodes."),
         Category("Appearance")]
        public string ImageKey
        {
            get { return _ImageKey; }
            set
            {
                _ImageKey = value;
                _DropDown.UpdateVisibleItems();
            }
        }

        /// <summary>
        ///     Gets or sets an ImageList component which provides the images displayed beside nodes in the control.
        /// </summary>
        [DefaultValue(null),
         Description("An ImageList component which provides the images displayed beside nodes in the control."),
         Category("Appearance")]
        public ImageList Images
        {
            get { return _Images; }
            set
            {
                _Images = value;
                _DropDown.UpdateVisibleItems();
            }
        }

        /// <summary>
        ///     Gets the collection of top-level nodes contained by the control.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
         Description("The collection of top-level nodes contained by the control."),
         Category("Data")]
        public ComboTreeNodeCollection Nodes
        {
            get { return _Nodes; }
        }

        /// <summary>
        ///     Gets or sets the text displayed in the editable portion of the control if the SelectedNode property is null.
        /// </summary>
        [DefaultValue(""),
         Description("The text displayed in the editable portion of the control if the SelectedNode property is null."),
         Category("Appearance")]
        public string NullValue
        {
            get { return _NullValue; }
            set
            {
                _NullValue = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Gets or sets the path to the selected node.
        /// </summary>
        [DefaultValue(""),
         Description("The path to the selected node."),
         Category("Behavior")]
        public string Path
        {
            get
            {
                var s = new StringBuilder();

                if (_SelectedNode != null)
                {
                    s.Append(_UseNodeNamesForPath ? _SelectedNode.Name : _SelectedNode.Text);

                    ComboTreeNode node = _SelectedNode;
                    while ((node = node.Parent) != null)
                    {
                        s.Insert(0, _PathSeparator);
                        s.Insert(0, _UseNodeNamesForPath ? node.Name : node.Text);
                    }
                }

                return s.ToString();
            }
            set
            {
                ComboTreeNode select = null;

                string[] parts = value.Split(new[] {_PathSeparator}, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < parts.Length; i++)
                {
                    ComboTreeNodeCollection collection = ((select == null) ? _Nodes : select.Nodes);
                    if (_UseNodeNamesForPath)
                    {
                        try
                        {
                            select = collection[parts[i]];
                        }
                        catch (KeyNotFoundException ex)
                        {
                            throw new ArgumentException("Invalid path string.", "value", ex);
                        }
                    }
                    else
                    {
                        bool found = false;
                        foreach (ComboTreeNode node in collection)
                        {
                            if (node.Text.Equals(parts[i], StringComparison.OrdinalIgnoreCase))
                            {
                                select = node;
                                found = true;
                                break;
                            }
                        }
                        if (!found) throw new ArgumentException("Invalid path string.", "value");
                    }
                }

                SelectedNode = select;
            }
        }

        /// <summary>
        ///     Gets or sets the string used to separate nodes in the Path property.
        /// </summary>
        [DefaultValue(@"\"),
         Description("The string used to separate nodes in the path string."),
         Category("Behavior")]
        public string PathSeparator
        {
            get { return _PathSeparator; }
            set
            {
                _PathSeparator = value;
                if (_ShowPath) Invalidate();
            }
        }

        /// <summary>
        ///     Gets or sets the node selected in the control.
        /// </summary>
        [Browsable(false)]
        public ComboTreeNode SelectedNode
        {
            get { return _SelectedNode; }
            set
            {
                if (!OwnsNode(value)) throw new ArgumentException("Node does not belong to this control.", "value");
                SetSelectedNode(value);
            }
        }

        /// <summary>
        ///     Determines whether the full path to the selected node is displayed in the editable portion of the control.
        /// </summary>
        [DefaultValue(false),
         Description("Determines whether the path to the selected node is displayed in the editable portion of the control."),
         Category("Appearance")]
        public bool ShowPath
        {
            get { return _ShowPath; }
            set
            {
                _ShowPath = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Hides the Text property from the designer.
        /// </summary>
        [Browsable(false)]
        public override string Text
        {
            get { return string.Empty; }
            set
            {
                value = string.Empty;
                base.Text = value;                 
            }
        }

        /// <summary>
        ///     Gets or sets the first visible ComboTreeNode in the drop-down portion of the control.
        /// </summary>
        [Browsable(false)]
        public ComboTreeNode TopNode
        {
            get { return _DropDown.TopNode; }
            set { _DropDown.TopNode = value; }
        }

        /// <summary>
        ///     Determines whether the Name property of the nodes is used to construct the path string.
        ///     The default behaviour is to use the Text property.
        /// </summary>
        [DefaultValue(false),
         Description("Determines whether the Name property of the nodes is used to construct the path string. The default behaviour is to use the Text property."),
         Category("Behavior")]
        public bool UseNodeNamesForPath
        {
            get { return _UseNodeNamesForPath; }
            set
            {
                _UseNodeNamesForPath = value;
                if (_ShowPath) Invalidate();
            }
        }

        /// <summary>
        ///     Gets the number of ComboTreeNodes visible in the drop-down portion of the control.
        /// </summary>
        [Browsable(false)]
        public int VisibleCount
        {
            get { return _DropDown.VisibleCount; }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets whether the dropdown portion of the control is displayed.
        /// </summary>
        [Browsable(false)]
        protected override bool DroppedDown
        {
            get { return base.DroppedDown; }
            set { SetDroppedDown(value, true); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Prevents the dropdown portion of the control from being updated until the EndUpdate method is called.
        /// </summary>
        public void BeginUpdate()
        {
            _IsUpdating = true;
        }

        /// <summary>
        ///     Collapses all nodes in the tree for when the dropdown portion of the control is reopened.
        /// </summary>
        public void CollapseAll()
        {
            foreach (ComboTreeNode node in AllNodes) node.Expanded = false;
        }

        /// <summary>
        ///     Updates the dropdown portion of the control after being suspended by the BeginUpdate method.
        /// </summary>
        public void EndUpdate()
        {
            _IsUpdating = false;
            if (!OwnsNode(_SelectedNode)) SetSelectedNode(null);
            _DropDown.UpdateVisibleItems();
        }

        /// <summary>
        ///     Expands all nodes in the tree for when the dropdown portion of the control is reopened.
        /// </summary>
        public void ExpandAll()
        {
            foreach (ComboTreeNode node in AllNodes) if (node.Nodes.Count > 0) node.Expanded = true;
        }

        /// <summary>
        ///     Sorts the contents of the tree using the default comparer.
        /// </summary>
        public void Sort()
        {
            Sort(null);
        }

        /// <summary>
        ///     Sorts the contents of the tree using the specified comparer.
        /// </summary>
        /// <param name="comparer"></param>
        public void Sort(IComparer<ComboTreeNode> comparer)
        {
            bool oldIsUpdating = _IsUpdating;
            _IsUpdating = true;
            _Nodes.Sort(comparer);
            if (!oldIsUpdating) EndUpdate();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Returns the image referenced by the specified node in the ImageList component associated with this control.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal Image GetNodeImage(ComboTreeNode node)
        {
            if ((_Images != null) && (node != null))
            {
                if (node.Expanded)
                {
                    if (_Images.Images.ContainsKey(node.ExpandedImageKey))
                        return _Images.Images[node.ExpandedImageKey]; // node's key
                    if (node.ExpandedImageIndex >= 0)
                        return _Images.Images[node.ExpandedImageIndex]; // node's index
                    if (_Images.Images.ContainsKey(_ExpandedImageKey))
                        return _Images.Images[_ExpandedImageKey]; // default key
                    if (_ExpandedImageIndex >= 0)
                        return _Images.Images[_ExpandedImageIndex]; // default index
                }
                else
                {
                    if (_Images.Images.ContainsKey(node.ImageKey))
                        return _Images.Images[node.ImageKey]; // node's key
                    if (node.ImageIndex >= 0)
                        return _Images.Images[node.ImageIndex]; // node's index
                    if (_Images.Images.ContainsKey(_ImageKey))
                        return _Images.Images[_ImageKey]; // default key
                    if (_ImageIndex >= 0)
                        return _Images.Images[_ImageIndex]; // default index
                }
            }

            return null;
        }

        /// <summary>
        ///     Determines whether the specified node should be displayed.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal bool IsNodeVisible(ComboTreeNode node)
        {
            bool displayed = true;
            ComboTreeNode parent = node;
            while ((parent = parent.Parent) != null)
            {
                if (!parent.Expanded)
                {
                    displayed = false;
                    break;
                }
            }
            return displayed;
        }

        /// <summary>
        ///     Sets the value of the DroppedDown property, optionally without raising any events.
        /// </summary>
        /// <param name="droppedDown"></param>
        /// <param name="raiseEvents"></param>
        internal void SetDroppedDown(bool droppedDown, bool raiseEvents)
        {
            base.DroppedDown = droppedDown;

            if (raiseEvents)
            {
                if (droppedDown)
                    _DropDown.Open();
                else
                    _DropDown.Close();
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Disposes of the control and its dropdown.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) _DropDown.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        ///     Updates the dropdown's font when the control's font changes.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _DropDown.Font = Font;
        }

        /// <summary>
        ///     Handles keyboard shortcuts.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = e.SuppressKeyPress = true;

            if (e.Alt && (e.KeyCode == Keys.Down))
            {
                DroppedDown = true;
            }
            else if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Left))
            {
                ComboTreeNode prev = GetPrevDisplayedNode();
                if (prev != null) SetSelectedNode(prev);
            }
            else if ((e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Right))
            {
                ComboTreeNode next = GetNextDisplayedNode();
                if (next != null) SetSelectedNode(next);
            }
            else
            {
                e.Handled = e.SuppressKeyPress = false;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        ///     Closes the dropdown portion of the control when it loses focus.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (!_DropDown.Focused) _DropDown.Close();
        }

        /// <summary>
        ///     Toggles the visibility of the dropdown portion of the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left) DroppedDown = !DroppedDown;
        }

        /// <summary>
        ///     Scrolls between adjacent nodes, or scrolls the drop-down portion of
        ///     the control in response to the mouse wheel.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (DroppedDown)
                _DropDown.ScrollDropDown(-(e.Delta/120)*SystemInformation.MouseWheelScrollLines);
            else if (e.Delta > 0)
            {
                ComboTreeNode prev = GetPrevDisplayedNode();
                if (prev != null) SetSelectedNode(prev);
            }
            else if (e.Delta < 0)
            {
                ComboTreeNode next = GetNextDisplayedNode();
                if (next != null) SetSelectedNode(next);
            }
        }

        /// <summary>
        ///     Paints the selected node in the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintContent(DropDownPaintEventArgs e)
        {
            base.OnPaintContent(e);

            Image img = GetNodeImage(_SelectedNode);
            string text = (_SelectedNode != null) ? ((_ShowPath) ? Path : _SelectedNode.Text) : _NullValue;

            Rectangle imgBounds = (img == null)
                ? new Rectangle(1, 0, 0, 0)
                : new Rectangle(4, (e.Bounds.Height/2 - img.Height/2) + 1, img.Width, img.Height);
            var txtBounds = new Rectangle(imgBounds.Right, 0, e.Bounds.Width - imgBounds.Right - 3, e.Bounds.Height);

            if (img != null) e.Graphics.DrawImage(img, imgBounds);

            TextRenderer.DrawText(e.Graphics, text, Font, txtBounds, ForeColor, FormatFlags);

            // focus rectangle
            if (Focused && ShowFocusCues && !DroppedDown) e.DrawFocusRectangle();
        }

        /// <summary>
        ///     Raises the SelectedNodeChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectedNodeChanged(EventArgs e)
        {
            EventHandler eventHandler = SelectedNodeChanged;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the displayable node, relative to to the selected node from the enumeration.
        /// </summary>
        /// <param name="e">The enumeration of nodes.</param>
        /// <returns>
        ///     Returns the <see cref="ComboTreeNode" /> that is displayed; otherwise <c>null</c>.
        /// </returns>
        private ComboTreeNode GetDisplayableNode(IEnumerator<ComboTreeNode> e)
        {
            bool started = false;
            while (e.MoveNext())
            {
                if (started || (_SelectedNode == null))
                {
                    if (IsNodeVisible(e.Current)) return e.Current;
                }
                else if (e.Current == _SelectedNode)
                {
                    started = true;
                }
            }

            return null;
        }

        /// <summary>
        ///     Returns the next displayable node, relative to the selected node.
        /// </summary>
        /// <returns>
        ///     Returns the <see cref="ComboTreeNode" /> that is displayed; otherwise <c>null</c>.
        /// </returns>
        private ComboTreeNode GetNextDisplayedNode()
        {
            IEnumerator<ComboTreeNode> e = GetNodesRecursive(_Nodes, false);
            return GetDisplayableNode(e);
        }

        /// <summary>
        ///     Helper method for the AllNodes property.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="reverse">if set to <c>true</c> [reverse].</param>
        /// <returns></returns>
        private IEnumerator<ComboTreeNode> GetNodesRecursive(ComboTreeNodeCollection collection, bool reverse)
        {
            if (!reverse)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    yield return collection[i];
                    IEnumerator<ComboTreeNode> e = GetNodesRecursive(collection[i].Nodes, false);
                    while (e.MoveNext()) yield return e.Current;
                }
            }
            else
            {
                for (int i = (collection.Count - 1); i >= 0; i--)
                {
                    IEnumerator<ComboTreeNode> e = GetNodesRecursive(collection[i].Nodes, true);
                    while (e.MoveNext()) yield return e.Current;
                    yield return collection[i];
                }
            }
        }

        /// <summary>
        ///     Returns the previous displayable node, relative to the selected node.
        /// </summary>
        /// <returns>
        ///     Returns the <see cref="ComboTreeNode" /> that is displayed; otherwise <c>null</c>.
        /// </returns>
        private ComboTreeNode GetPrevDisplayedNode()
        {
            IEnumerator<ComboTreeNode> e = GetNodesRecursive(_Nodes, true);
            return GetDisplayableNode(e);
        }

        /// <summary>
        ///     Determines whether the specified node belongs to this ComboTreeBox, and
        ///     hence is a valid selection. For the purposes of this method, a null
        ///     value is always a valid selection.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool OwnsNode(ComboTreeNode node)
        {
            if (node == null) return true;

            ComboTreeNode parent = node;
            while (parent.Parent != null) parent = parent.Parent;
            return _Nodes.Contains(parent);
        }

        /// <summary>
        ///     Sets the value of the SelectedNode property and raises the SelectedNodeChanged event.
        /// </summary>
        /// <param name="node"></param>
        private void SetSelectedNode(ComboTreeNode node)
        {
            if (!Equals(_SelectedNode, node))
            {
                _SelectedNode = node;
                Invalidate();
                OnSelectedNodeChanged(EventArgs.Empty);
            }
        }

        private void dropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            OnDropDownClosed(EventArgs.Empty);
        }

        private void dropDown_Opened(object sender, EventArgs e)
        {
            OnDropDown(EventArgs.Empty);
        }

        /// <summary>
        ///     Handles the CollectionChanged event of the nodes control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_IsUpdating)
            {
                // verify that selected node still belongs to the tree
                if (!OwnsNode(_SelectedNode)) SetSelectedNode(null);

                // rebuild the view
                _DropDown.UpdateVisibleItems();
            }
        }

        #endregion
    }
}