using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace System.Forms.Controls
{
    /// <summary>
    ///     Represents the dropdown portion of the ComboTreeBox control. The nodes are displayed in a
    ///     manner similar to the TreeView control.
    /// </summary>
    /// <remarks>http://www.brad-smith.info/blog/archives/193</remarks>
    [ToolboxItem(false)]
    public class ComboTreeDropDown : ToolStripDropDown
    {
        #region Constants

        private const TextFormatFlags FormatFlags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;
        private const int GlythSize = 16;
        private const int IndentWidth = 16;
        private const int MinItemHeight = 16;
        private const int MinThumbHeight = 20;
        private const int ScrollbarWidth = 17;

        #endregion

        #region Fields

        private readonly Dictionary<BitmapInfo, Image> _Bitmaps;
        private readonly ScrollBarInfo _ScrollBar;
        private readonly Size _ScrollButtonSize = new Size(ScrollbarWidth, ScrollbarWidth);
        private readonly Timer _ScrollRepeater;
        private readonly ComboTreeBox _SourceControl;
        private readonly List<NodeInfo> _VisibleItems;
        private static Bitmap _Collapsed;

        private int _DropDownHeight;
        private static Bitmap _Expanded;
        private int _HighlightedItemIndex;
        private Rectangle _Interior;
        private int _ItemHeight;
        private int _NumItemsDisplayed;
        private bool _ScrollBarVisible;
        private bool _ScrollDragging;
        private int _ScrollOffset;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialises a new instance of ComboTreeDropDown and associates it with its parent ComboTreeBox.
        /// </summary>
        /// <param name="sourceControl"></param>
        public ComboTreeDropDown(ComboTreeBox sourceControl)
        {
            _VisibleItems = new List<NodeInfo>();
            _Bitmaps = new Dictionary<BitmapInfo, Image>();
            _ScrollBar = new ScrollBarInfo();
            AutoSize = false;
            _SourceControl = sourceControl;
            RenderMode = ToolStripRenderMode.System;
            BackColor = Color.White;
            _DropDownHeight = 150;
            _ItemHeight = MinItemHeight;
            Items.Add("");

            _ScrollRepeater = new Timer();
            _ScrollRepeater.Tick += scrollRepeater_Tick;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the maximum height of the dropdown.
        /// </summary>
        public int DropDownHeight
        {
            get { return _DropDownHeight; }
            set
            {
                _DropDownHeight = value;
                UpdateVisibleItems();
            }
        }

        /// <summary>
        ///     Gets or sets the first visible ComboTreeNode in the drop-down portion of the control.
        /// </summary>
        public ComboTreeNode TopNode
        {
            get
            {
                if (_ScrollOffset >= _VisibleItems.Count || _ScrollOffset < 0)
                    return null;

                return _VisibleItems[_ScrollOffset].Node;
            }
            set
            {
                for (int i = 0; i < _VisibleItems.Count; i++)
                {
                    if (_VisibleItems[i].Node == value)
                    {
                        if ((i < _ScrollOffset) || (i >= (_ScrollOffset + _NumItemsDisplayed)))
                        {
                            _ScrollOffset = Math.Min(Math.Max(0, i - _NumItemsDisplayed + 1), _VisibleItems.Count - _NumItemsDisplayed);
                            UpdateScrolling();
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the number of ComboTreeNodes visible in the drop-down portion of the control.
        /// </summary>
        public int VisibleCount
        {
            get { return _NumItemsDisplayed; }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Removes extraneous default padding from the dropdown.
        /// </summary>
        protected override Padding DefaultPadding
        {
            get { return new Padding(0, 1, 0, 1); }
        }

        #endregion

        #region Private Properties

        /// <summary>
        ///     Gets the collapsed (+) glyph to paint on the dropdown.
        /// </summary>
        private Image Collapsed
        {
            get
            {
                if (_Collapsed == null)
                {
                    _Collapsed = new Bitmap(16, 16);

                    DrawGlyph(_Collapsed);
                }

                return _Collapsed;
            }
        }

        /// <summary>
        ///     Gets the expanded (-) glyph to paint on the dropdown.
        /// </summary>
        private Image Expanded
        {
            get
            {
                if (_Expanded == null)
                {
                    _Expanded = new Bitmap(16, 16);

                    DrawGlyph(_Expanded);
                }
                return _Expanded;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Displays the dropdown beneath its owning ComboTreeBox control.
        /// </summary>
        public void Open()
        {
            if (_SourceControl.SelectedNode != null)
            {
                ComboTreeNode node = _SourceControl.SelectedNode;

                // the selected node must have a clear path (i.e. all parents expanded)
                while ((node = node.Parent) != null) node.Expanded = true;
            }

            UpdateVisibleItems();

            // highlight and scroll to the selected node
            if (_SourceControl.SelectedNode != null)
            {
                for (int i = 0; i < _VisibleItems.Count; i++)
                {
                    if (_VisibleItems[i].Node == _SourceControl.SelectedNode)
                    {
                        _HighlightedItemIndex = i;
                        if ((_HighlightedItemIndex < _ScrollOffset) || (_HighlightedItemIndex >= (_ScrollOffset + _NumItemsDisplayed)))
                        {
                            _ScrollOffset = Math.Min(Math.Max(0, _HighlightedItemIndex - _NumItemsDisplayed + 1), _VisibleItems.Count - _NumItemsDisplayed);
                            UpdateScrolling();
                        }
                        break;
                    }
                }
            }

            // show below the source control
            Show(_SourceControl, new Point(0, _SourceControl.ClientRectangle.Height - 1));
        }

        /// <summary>
        ///     Scrolls the drop-down up or down by the specified number of items.
        /// </summary>
        /// <param name="offset"></param>
        public void ScrollDropDown(int offset)
        {
            if (offset < 0)
            {
                // up/left
                _ScrollOffset = Math.Max(_ScrollOffset + offset, 0);
                UpdateScrolling();
                Invalidate();
            }
            else if (offset > 0)
            {
                // down/right
                _ScrollOffset = Math.Min(_ScrollOffset + offset, _VisibleItems.Count - _NumItemsDisplayed);
                UpdateScrolling();
                Invalidate();
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     Regenerates the items on the dropdown. This method is called whenever
        ///     a significant change occurs to the dropdown, such as a change in the
        ///     tree or changes to the layout of the owning control.
        /// </summary>
        internal void UpdateVisibleItems()
        {
            SuspendLayout();

            // clear bitmap cache
            _Bitmaps.Clear();

            // populate the collection with the displayable items only
            _VisibleItems.Clear();
            foreach (ComboTreeNode node in _SourceControl.AllNodes)
            {
                if (_SourceControl.IsNodeVisible(node)) _VisibleItems.Add(new NodeInfo(node));
            }

            _HighlightedItemIndex = Math.Max(0, Math.Min(_HighlightedItemIndex, _VisibleItems.Count - 1));

            _NumItemsDisplayed = Math.Min((_DropDownHeight/_ItemHeight) + 1, _VisibleItems.Count);
            int maxHeight = ((((_DropDownHeight - 2)/_ItemHeight) + 1)*_ItemHeight) + 2;

            Size = new Size(_SourceControl.ClientRectangle.Width, Math.Min(maxHeight, (_VisibleItems.Count*_ItemHeight) + 2));

            // represents the entire paintable area
            _Interior = ClientRectangle;
            _Interior.Inflate(-1, -1);

            _ScrollBarVisible = (_NumItemsDisplayed < _VisibleItems.Count);
            if (_ScrollBarVisible)
            {
                _ScrollOffset = Math.Max(0, Math.Min(_ScrollOffset, (_VisibleItems.Count - _NumItemsDisplayed)));
                _Interior.Width -= 17;
                _ScrollBar.DisplayRectangle = new Rectangle(_Interior.Right, _Interior.Top, 17, _Interior.Height);
                _ScrollBar.UpArrow = new Rectangle(_ScrollBar.DisplayRectangle.Location, _ScrollButtonSize);
                _ScrollBar.DownArrow = new Rectangle(new Point(_ScrollBar.DisplayRectangle.X, _ScrollBar.DisplayRectangle.Bottom - 17), _ScrollButtonSize);
            }

            UpdateScrolling();

            ResumeLayout();
            Invalidate();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Registers the arrow keys as input keys.
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Home:
                case Keys.End:
                case Keys.Enter:
                    return true;
                default:
                    return base.IsInputKey(keyData);
            }
        }

        /// <summary>
        ///     Updates the status of the dropdown on the owning ComboTreeBox control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
        {
            base.OnClosed(e);

            // update DroppedDown on ComboTreeBox after close
            _SourceControl.SetDroppedDown(false, false);
        }

        /// <summary>
        ///     Prevents the clicking of items from closing the dropdown.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked) e.Cancel = true;
            if (e.CloseReason == ToolStripDropDownCloseReason.AppClicked)
            {
                if (_SourceControl.ClientRectangle.Contains(_SourceControl.PointToClient(Cursor.Position))) e.Cancel = true;
            }

            base.OnClosing(e);
        }

        /// <summary>
        ///     Updates the font on the items when the drop-down's font changes.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _ItemHeight = Math.Max(MinItemHeight, Font.Height);
        }

        /// <summary>
        ///     Handles keyboard shortcuts.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = e.SuppressKeyPress = true;

            if ((e.KeyCode == Keys.Enter) || (e.Alt && (e.KeyCode == Keys.Up)))
            {
                _SourceControl.SelectedNode = _VisibleItems[_HighlightedItemIndex].Node;
                Close();
            }
            else if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Left))
            {
                _HighlightedItemIndex = Math.Max(0, _HighlightedItemIndex - 1);
                _SourceControl.SelectedNode = _VisibleItems[_HighlightedItemIndex].Node;
                ScrollToHighlighted(true);
                Refresh();
            }
            else if ((e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Right))
            {
                _HighlightedItemIndex = Math.Min(_HighlightedItemIndex + 1, _VisibleItems.Count - 1);
                _SourceControl.SelectedNode = _VisibleItems[_HighlightedItemIndex].Node;
                ScrollToHighlighted(false);
                Refresh();
            }
            else if (e.KeyCode == Keys.Home)
            {
                _HighlightedItemIndex = _ScrollOffset = 0;
                UpdateScrolling();
                Invalidate();
            }
            else if (e.KeyCode == Keys.End)
            {
                _ScrollOffset = _VisibleItems.Count - _NumItemsDisplayed;
                _HighlightedItemIndex = _VisibleItems.Count - 1;
                UpdateScrolling();
                Invalidate();
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                _ScrollOffset = Math.Min(_ScrollOffset + _NumItemsDisplayed, _VisibleItems.Count - _NumItemsDisplayed);
                _HighlightedItemIndex = Math.Min(_ScrollOffset + _NumItemsDisplayed - 1, _VisibleItems.Count - 1);
                UpdateScrolling();
                Refresh();
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                _HighlightedItemIndex = _ScrollOffset = Math.Max(_ScrollOffset - _NumItemsDisplayed, 0);
                UpdateScrolling();
                Refresh();
            }
            else
            {
                e.Handled = e.SuppressKeyPress = false;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        ///     Handles keyboard shortcuts.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            e.Handled = true;

            if (e.KeyChar == '+')
            {
                NodeInfo info = _VisibleItems[_HighlightedItemIndex];
                if (info.Node.Nodes.Count > 0)
                {
                    info.Node.Expanded = true;
                    UpdateVisibleItems();
                }
            }
            else if (e.KeyChar == '-')
            {
                NodeInfo info = _VisibleItems[_HighlightedItemIndex];
                if (info.Node.Nodes.Count > 0)
                {
                    info.Node.Expanded = false;
                    UpdateVisibleItems();
                }
            }
            if (e.KeyChar == '*')
            {
                _SourceControl.ExpandAll();
                UpdateVisibleItems();
            }
            else if (e.KeyChar == '/')
            {
                _SourceControl.CollapseAll();
                UpdateVisibleItems();
            }
            else
            {
                e.Handled = false;
            }

            base.OnKeyPress(e);
        }

        /// <summary>
        ///     Handles the expand/collapse of nodes and selection in response to the
        ///     mouse being clicked.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (_ScrollDragging) return;

            if (e.Button == MouseButtons.Left)
            {
                for (int i = _ScrollOffset; i < (_ScrollOffset + _NumItemsDisplayed); i++)
                {
                    NodeInfo info = _VisibleItems[i];

                    if (info.DisplayRectangle.Contains(e.Location))
                    {
                        if (info.GlyphRectangle.Contains(e.Location))
                        {
                            info.Node.Expanded = !info.Node.Expanded;
                            UpdateVisibleItems();
                        }
                        else
                        {
                            _SourceControl.SelectedNode = _VisibleItems[i].Node;
                            Close();
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Handles scrolling in response to the left mouse button being clicked.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left) return;

            // mouse pointer within the scrollbar's bounds
            if (_ScrollBarVisible && _ScrollBar.DisplayRectangle.Contains(e.Location))
            {
                if (e.Y > _ScrollBar.Thumb.Bottom)
                {
                    // any point below the thumb button requires scrolling - on bar = pagedown, on button = next
                    int step = (_ScrollBar.DownArrow.Contains(e.Location)) ? 1 : _NumItemsDisplayed;
                    ScrollDropDown(step);

                    // if the button is held, start auto-repeat behaviour
                    if (!_ScrollRepeater.Enabled)
                    {
                        _ScrollRepeater.Interval = 1000;
                        _ScrollRepeater.Start();
                    }
                    return;
                }
                if (e.Y < _ScrollBar.Thumb.Top)
                {
                    // any point above the thumb button requires scrolling - on bar = pagedown, on button = next
                    int step = (_ScrollBar.UpArrow.Contains(e.Location)) ? 1 : _NumItemsDisplayed;
                    ScrollDropDown(-step);

                    // if the button is held, start auto-repeat behaviour
                    if (!_ScrollRepeater.Enabled)
                    {
                        _ScrollRepeater.Interval = 1000;
                        _ScrollRepeater.Start();
                    }
                    return;
                }
                if (_ScrollBar.Thumb.Contains(e.Location))
                {
                    // assume the thumb button is being dragged
                    _ScrollDragging = true;
                }

                Invalidate();
            }
        }

        /// <summary>
        ///     Terminates dragging of the scrollbar in response to the mouse
        ///     returning to the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if ((MouseButtons & MouseButtons.Left) != MouseButtons.Left) _ScrollDragging = false;
        }

        /// <summary>
        ///     Terminates dragging of the scrollbar in response to the mouse leaving
        ///     the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if ((MouseButtons & MouseButtons.Left) != MouseButtons.Left) _ScrollDragging = false;
        }

        /// <summary>
        ///     Handles dragging of the scrollbar and hot-tracking in response to movement of the mouSE.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // dragging with the scrollbar's 'thumb' button
            if (_ScrollDragging)
            {
                double availableHeight = _ScrollBar.DisplayRectangle.Height - (2*_ScrollButtonSize.Height) - _ScrollBar.Thumb.Height;
                double position = Math.Min(e.Location.Y - _ScrollBar.DisplayRectangle.Top - _ScrollButtonSize.Height - (_ScrollBar.Thumb.Height/2), availableHeight);

                // measure the scroll offset based on the location of the mouse pointer, relative to the scrollbar's bounds
                _ScrollOffset = Math.Max(0, Math.Min(
                    (int) ((position/availableHeight)*(_VisibleItems.Count - _NumItemsDisplayed)),
                    (_VisibleItems.Count - _NumItemsDisplayed)
                    ));

                UpdateScrolling();
                Refresh();
                return;
            }

            // moving the mouse over the scrollbar
            if (_ScrollBarVisible && _ScrollBar.DisplayRectangle.Contains(e.Location))
            {
                Invalidate();
                return;
            }

            // not within scrollbar's bounds, end auto-repeat behaviour
            _ScrollRepeater.Stop();

            // hit-test each displayed item's bounds to determine the highlighted item
            for (int i = _ScrollOffset; i < (_ScrollOffset + _NumItemsDisplayed); i++)
            {
                if (i >= 0 && i < _VisibleItems.Count)
                {
                    if (_VisibleItems[i].DisplayRectangle.Contains(e.Location))
                    {
                        _HighlightedItemIndex = i;
                        Invalidate();
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Disengages dragging of the scrollbar and handles hot-tracking in
        ///     response to the mouse button being released.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _ScrollRepeater.Stop();
            _ScrollDragging = false;

            if (_ScrollBarVisible && _ScrollBar.DisplayRectangle.Contains(e.Location))
            {
                Invalidate();
            }
        }

        /// <summary>
        ///     Paints the drop-down, including all items within the scrolled region
        ///     and, if appropriate, the scrollbar.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_ScrollBarVisible)
            {
                var upper = new Rectangle(_ScrollBar.DisplayRectangle.Left, _ScrollBar.DisplayRectangle.Top, _ScrollBar.DisplayRectangle.Width, _ScrollBar.Thumb.Top - _ScrollBar.DisplayRectangle.Top);
                var lower = new Rectangle(_ScrollBar.DisplayRectangle.Left, _ScrollBar.Thumb.Bottom, _ScrollBar.DisplayRectangle.Width, _ScrollBar.DisplayRectangle.Bottom - _ScrollBar.Thumb.Bottom);

                if (_SourceControl.DrawWithVisualStyles && ScrollBarRenderer.IsSupported)
                {
                    ScrollBarRenderer.DrawUpperVerticalTrack(e.Graphics, upper, GetScrollBarState(upper));
                    ScrollBarRenderer.DrawLowerVerticalTrack(e.Graphics, lower, GetScrollBarState(lower));
                    ScrollBarRenderer.DrawArrowButton(e.Graphics, _ScrollBar.UpArrow, GetScrollBarStateUp());
                    ScrollBarRenderer.DrawArrowButton(e.Graphics, _ScrollBar.DownArrow, GetScrollBarStateDown());
                    ScrollBarRenderer.DrawVerticalThumb(e.Graphics, _ScrollBar.Thumb, GetScrollBarThumbState());
                    ScrollBarRenderer.DrawVerticalThumbGrip(e.Graphics, _ScrollBar.Thumb, GetScrollBarThumbState());
                }
                else
                {
                    Rectangle bounds = _ScrollBar.DisplayRectangle;
                    bounds.Offset(1, 0);
                    Rectangle up = _ScrollBar.UpArrow;
                    up.Offset(1, 0);
                    Rectangle down = _ScrollBar.DownArrow;
                    down.Offset(1, 0);
                    Rectangle thumb = _ScrollBar.Thumb;
                    thumb.Offset(1, 0);

                    var brush = new HatchBrush(HatchStyle.Percent50, SystemColors.ControlLightLight, SystemColors.Control);

                    e.Graphics.FillRectangle(brush, bounds);
                    ControlPaint.DrawScrollButton(e.Graphics, up, ScrollButton.Up, GetButtonState(_ScrollBar.UpArrow));
                    ControlPaint.DrawScrollButton(e.Graphics, down, ScrollButton.Down, GetButtonState(_ScrollBar.DownArrow));
                    ControlPaint.DrawButton(e.Graphics, thumb, ButtonState.Normal);
                }
            }

            for (int i = _ScrollOffset; i < (_ScrollOffset + _NumItemsDisplayed); i++)
            {
                bool highlighted = (_HighlightedItemIndex == i);
                NodeInfo item = _VisibleItems[i];

                // background
                if (highlighted) e.Graphics.FillRectangle(SystemBrushes.Highlight, item.DisplayRectangle);

                // image and glyphs
                if (item.Image != null) e.Graphics.DrawImage(item.Image, new Rectangle(item.DisplayRectangle.Location, item.Image.Size));

                var font = new Font(Font, _VisibleItems[i].Node.FontStyle);

                if (item.Image != null)
                {
                    var textBounds = new Rectangle(item.DisplayRectangle.X + item.Image.Width + 2, item.DisplayRectangle.Y, item.DisplayRectangle.Width - item.Image.Width - 4, _ItemHeight);
                    TextRenderer.DrawText(e.Graphics, item.Node.Text, font, textBounds, highlighted ? SystemColors.HighlightText : ForeColor, FormatFlags);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Draws the glyph.
        /// </summary>
        /// <param name="image">The image.</param>
        private void DrawGlyph(Bitmap image)
        {
            Graphics g = Graphics.FromImage(image);
            var r = new Rectangle(4, 4, 8, 8);
            g.FillRectangle(Brushes.White, r);
            g.DrawRectangle(Pens.Gray, r);
            g.DrawLine(Pens.Black, Point.Add(r.Location, new Size(2, 4)), Point.Add(r.Location, new Size(6, 4)));
            g.DrawLine(Pens.Black, Point.Add(r.Location, new Size(4, 2)), Point.Add(r.Location, new Size(4, 6)));
        }

        /// <summary>
        ///     Generates a bitmap to display beside the ToolStripItem representation of the specified node.
        /// </summary>
        /// <param name="bitmapInfo"></param>
        /// <param name="nodeImage"></param>
        /// <returns></returns>
        private Image GenerateBitmap(BitmapInfo bitmapInfo, Image nodeImage)
        {
            int indentation = IndentWidth*bitmapInfo.NodeDepth;
            int halfIndent = IndentWidth/2;
            int halfHeight = _ItemHeight/2;

            // create a bitmap that will be composed of the node's image and the glyphs/lines/indentation
            var composite = new Bitmap(IndentWidth + indentation + ((nodeImage != null) ? nodeImage.Width : 0), _ItemHeight);
            Graphics g = Graphics.FromImage(composite);

            var dotted = new Pen(Color.Gray);
            dotted.DashStyle = DashStyle.Dot;

            // horizontal dotted line
            g.DrawLine(dotted, indentation + halfIndent, halfHeight, indentation + IndentWidth, halfHeight);

            // vertical dotted line to peers
            g.DrawLine(dotted, indentation + halfIndent, bitmapInfo.IsFirst ? halfHeight : 0, indentation + halfIndent, bitmapInfo.IsLastPeer ? halfHeight : _ItemHeight);

            // vertical dotted line to subtree
            if (bitmapInfo.NodeExpanded) g.DrawLine(dotted, IndentWidth + indentation + halfIndent, halfHeight, IndentWidth + indentation + halfIndent, _ItemHeight);

            // outer vertical dotted lines
            for (int i = 0; i < bitmapInfo.VerticalLines.Length; i++)
            {
                if (bitmapInfo.VerticalLines[i])
                {
                    int parentIndent = (IndentWidth*(bitmapInfo.NodeDepth - (i + 1)));
                    g.DrawLine(dotted, parentIndent + halfIndent, 0, parentIndent + halfIndent, _ItemHeight);
                }
            }

            // composite the image associated with node (appears at far right)
            if (nodeImage != null)
                g.DrawImage(nodeImage, new Rectangle(
                    IndentWidth + indentation,
                    composite.Height/2 - nodeImage.Height/2,
                    nodeImage.Width,
                    nodeImage.Height
                    ));

            // render plus/minus glyphs
            if (bitmapInfo.HasChildren)
            {
                var glyphBounds = new Rectangle(indentation, composite.Height/2 - GlythSize/2, GlythSize, GlythSize);
                VisualStyleElement elem = bitmapInfo.NodeExpanded ? VisualStyleElement.TreeView.Glyph.Opened : VisualStyleElement.TreeView.Glyph.Closed;

                if (_SourceControl.DrawWithVisualStyles && VisualStyleRenderer.IsSupported && VisualStyleRenderer.IsElementDefined(elem))
                {
                    // visual style support, render using visual styles
                    var r = new VisualStyleRenderer(elem);
                    r.DrawBackground(g, glyphBounds);
                }
                else
                {
                    // no visual style support, render using bitmap
                    Image glyph = bitmapInfo.NodeExpanded ? Expanded : Collapsed;
                    g.DrawImage(glyph, glyphBounds);
                }
            }

            return composite;
        }

        /// <summary>
        ///     Determines how to draw a scrollbar button.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private ButtonState GetButtonState(Rectangle bounds)
        {
            var state = ButtonState.Normal;
            if (bounds.Contains(PointToClient(Cursor.Position)) && !_ScrollDragging)
            {
                if ((MouseButtons & MouseButtons.Left) == MouseButtons.Left) state = ButtonState.Pushed;
            }
            return state;
        }

        /// <summary>
        ///     Returns the ComboTreeNodeCollection to which the specified node belongs.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private ComboTreeNodeCollection GetCollectionContainingNode(ComboTreeNode node)
        {
            return (node.Parent != null) ? node.Parent.Nodes : _SourceControl.Nodes;
        }

        /// <summary>
        ///     Determines all of the parameters for drawing the bitmap beside the
        ///     specified node. If they represent a unique combination, the bitmap is
        ///     generated and returned. Otherwise, the appropriate cached bitmap is
        ///     returned.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Image GetItemBitmap(ComboTreeNode node)
        {
            var bitmapInfo = new BitmapInfo();

            // the following factors determine the bitmap drawn:
            ComboTreeNodeCollection collection = GetCollectionContainingNode(node);
            bitmapInfo.HasChildren = (node.Nodes.Count > 0);
            bitmapInfo.IsLastPeer = (collection.IndexOf(node) == (collection.Count - 1));
            bitmapInfo.IsFirst = (Equals(node, _SourceControl.Nodes[0]));
            bitmapInfo.NodeDepth = node.Depth;
            bitmapInfo.NodeExpanded = node.Expanded && bitmapInfo.HasChildren;
            bitmapInfo.ImageIndex = bitmapInfo.NodeExpanded ? node.ExpandedImageIndex : node.ImageIndex;
            bitmapInfo.ImageKey = bitmapInfo.NodeExpanded ? node.ExpandedImageKey : node.ImageKey;

            bitmapInfo.VerticalLines = new bool[bitmapInfo.NodeDepth];
            ComboTreeNode parent = node;
            int i = 0;
            while ((parent = parent.Parent) != null)
            {
                // vertical line required if parent is expanded (and not last peer)
                ComboTreeNodeCollection parentCollection = GetCollectionContainingNode(parent);
                bitmapInfo.VerticalLines[i] = (parent.Expanded && (parentCollection.IndexOf(parent) != (parentCollection.Count - 1)));
                i++;
            }

            if (_Bitmaps.ContainsKey(bitmapInfo))
                return _Bitmaps[bitmapInfo];
            return (_Bitmaps[bitmapInfo] = GenerateBitmap(bitmapInfo, _SourceControl.GetNodeImage(node)));
        }

        /// <summary>
        ///     Determines how to draw the main part of the scrollbar.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private ScrollBarState GetScrollBarState(Rectangle bounds)
        {
            var state = ScrollBarState.Normal;
            Point local = PointToClient(Cursor.Position);
            if (bounds.Contains(local)
                && !_ScrollDragging
                && !_ScrollBar.DownArrow.Contains(local)
                && !_ScrollBar.UpArrow.Contains(local)
                && !_ScrollBar.Thumb.Contains(local))
            {
                if ((MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                    state = ScrollBarState.Pressed;
                else
                    state = ScrollBarState.Hot;
            }
            return state;
        }

        /// <summary>
        ///     Determines how to draw the down arrow on the scrollbar.
        /// </summary>
        /// <returns></returns>
        private ScrollBarArrowButtonState GetScrollBarStateDown()
        {
            var state = ScrollBarArrowButtonState.DownNormal;
            if (_ScrollBar.DownArrow.Contains(PointToClient(Cursor.Position)) && !_ScrollDragging)
            {
                if ((MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                    state = ScrollBarArrowButtonState.DownPressed;
                else
                    state = ScrollBarArrowButtonState.DownHot;
            }
            return state;
        }

        /// <summary>
        ///     Determines how to draw the up arrow on the scrollbar.
        /// </summary>
        /// <returns></returns>
        private ScrollBarArrowButtonState GetScrollBarStateUp()
        {
            var state = ScrollBarArrowButtonState.UpNormal;
            if (_ScrollBar.UpArrow.Contains(PointToClient(Cursor.Position)) && !_ScrollDragging)
            {
                if ((MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                    state = ScrollBarArrowButtonState.UpPressed;
                else
                    state = ScrollBarArrowButtonState.UpHot;
            }
            return state;
        }

        /// <summary>
        ///     Determines how to draw the 'thumb' button on the scrollbar.
        /// </summary>
        /// <returns></returns>
        private ScrollBarState GetScrollBarThumbState()
        {
            var state = ScrollBarState.Normal;
            if (_ScrollBar.Thumb.Contains(PointToClient(Cursor.Position)))
            {
                if ((MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                    state = ScrollBarState.Pressed;
                else
                    state = ScrollBarState.Hot;
            }
            return state;
        }

        /// <summary>
        ///     Scrolls the drop-down so as to ensure that the highlighted item is at
        ///     either the top or bottom of the scrolled region.
        /// </summary>
        /// <param name="highlightedAtTop"></param>
        private void ScrollToHighlighted(bool highlightedAtTop)
        {
            if ((_HighlightedItemIndex < _ScrollOffset) || (_HighlightedItemIndex >= (_ScrollOffset + _NumItemsDisplayed)))
            {
                if (highlightedAtTop)
                    _ScrollOffset = Math.Min(_HighlightedItemIndex, _VisibleItems.Count - _NumItemsDisplayed);
                else
                    _ScrollOffset = Math.Min(Math.Max(0, _HighlightedItemIndex - _NumItemsDisplayed + 1), _VisibleItems.Count - _NumItemsDisplayed);

                UpdateScrolling();
            }
        }

        /// <summary>
        ///     Updates the items in the scrolled region. This method is called
        ///     whenever the scroll offset is changed.
        /// </summary>
        private void UpdateScrolling()
        {
            if (_ScrollBarVisible)
            {
                // calculate the bounds of the scrollbar's 'thumb' button
                int availableHeight = _ScrollBar.DisplayRectangle.Height - (2*_ScrollButtonSize.Height);

                double percentSize = _NumItemsDisplayed/(double) _VisibleItems.Count;
                int size = Math.Max((int) (percentSize*availableHeight), MinThumbHeight);
                int diff = Math.Max(0, MinThumbHeight - (int) (percentSize*availableHeight));

                double percentStart = (double) _ScrollOffset/_VisibleItems.Count;
                int start = Math.Min((int) Math.Ceiling(percentStart*(availableHeight - diff)), availableHeight - MinThumbHeight);

                _ScrollBar.Thumb = new Rectangle(new Point(_ScrollBar.DisplayRectangle.X, _ScrollBar.DisplayRectangle.Top + _ScrollButtonSize.Height + start), new Size(ScrollbarWidth, size));
            }

            // calculate display rectangles and assign images for each item in the scroll range
            for (int i = _ScrollOffset; i < (_ScrollOffset + _NumItemsDisplayed); i++)
            {
                if (i >= 0 && i < _VisibleItems.Count)
                {
                    NodeInfo info = _VisibleItems[i];
                    if (info.Image == null) info.Image = GetItemBitmap(info.Node);
                    info.DisplayRectangle = new Rectangle(_Interior.X, _Interior.Y + (_ItemHeight*(i - _ScrollOffset)), _Interior.Width, _ItemHeight);
                    int identation = (info.Node.Depth*IndentWidth);
                    info.GlyphRectangle = new Rectangle(identation, info.DisplayRectangle.Top, info.Image.Width - identation, info.Image.Height);
                }
            }
        }

        /// <summary>
        ///     Handles the Tick event of the scrollRepeater control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void scrollRepeater_Tick(object sender, EventArgs e)
        {
            // Simulate another click
            Point local = PointToClient(Cursor.Position);
            OnMouseDown(new MouseEventArgs(MouseButtons, 1, local.X, local.Y, 0));
        }

        #endregion

        #region Nested Type: BitmapInfo

        /// <summary>
        ///     Represents the variables which determine the bitmap to draw beside an
        ///     item. In a drop-down with a large number of items, there may be only a
        ///     small number of distinct bitmaps. This structure serves as a key to
        ///     aid in identifying the bitmap to uSE.
        /// </summary>
        private struct BitmapInfo : IEquatable<BitmapInfo>
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets whether the node has children. This is used to
            ///     determine if the plus/minus glyph is drawn.
            /// </summary>
            public bool HasChildren { get; set; }

            /// <summary>
            ///     Gets or sets the index of the image in the ComboTreeNode's
            ///     ImageList component to draw beside this node.
            /// </summary>
            public int ImageIndex { private get; set; }

            /// <summary>
            ///     Gets or sets the name of the image in the ComboTreeNode's
            ///     ImageList component to draw beside this node.
            /// </summary>
            public string ImageKey { private get; set; }

            /// <summary>
            ///     Gets or sets whether the node is the first in the entire tree. The
            ///     very first node does not draw a connector to its predecessor.
            /// </summary>
            public bool IsFirst { get; set; }

            /// <summary>
            ///     Gets or sets whether the node is the last peer in its branch of
            ///     the tree. These nodes do not draw a connector to their successor.
            /// </summary>
            public bool IsLastPeer { get; set; }

            /// <summary>
            ///     Gets or sets the zero-based depth of the node in the tree. This is
            ///     used to calculate indents.
            /// </summary>
            public int NodeDepth { get; set; }

            /// <summary>
            ///     Gets or sets whether the node has children and is expanded. This
            ///     will cause a connector to be drawn to the sub-tree.
            /// </summary>
            public bool NodeExpanded { get; set; }

            /// <summary>
            ///     Gets or sets whether outer vertical connectors are to be drawn for
            ///     each successive parent of the node.
            /// </summary>
            public bool[] VerticalLines { get; set; }

            #endregion

            #region IEquatable<BitmapInfo> Members

            /// <summary>
            ///     Used as the comparison function in the bitmap cache; ensures that
            ///     bitmaps are only created for distinct combinations of these
            ///     variables.
            /// </summary>
            /// <param name="that"></param>
            /// <returns></returns>
            public bool Equals(BitmapInfo that)
            {
                if (HasChildren != that.HasChildren)
                    return false;
                if (IsLastPeer != that.IsLastPeer)
                    return false;
                if (IsFirst != that.IsFirst)
                    return false;
                if (NodeDepth != that.NodeDepth)
                    return false;
                if (NodeExpanded != that.NodeExpanded)
                    return false;
                if (VerticalLines.Length != that.VerticalLines.Length)
                    return false;
                if (ImageIndex != that.ImageIndex)
                    return false;
                if (ImageKey != that.ImageKey)
                    return false;

                for (int i = 0; i < VerticalLines.Length; i++)
                {
                    if (VerticalLines[i] != that.VerticalLines[i]) return false;
                }

                return true;
            }

            #endregion
        }

        #endregion

        #region Nested Type: NodeInfo

        /// <summary>
        ///     Represents the information needed to draw and interact with a node in the drop-down.
        /// </summary>
        private class NodeInfo
        {
            #region Constructors

            /// <summary>
            ///     Creates a new instance of the NodeInfo class to represent the
            ///     specified ComboTreeNode.
            /// </summary>
            /// <param name="node"></param>
            public NodeInfo(ComboTreeNode node)
            {
                Node = node;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets or sets the current bounds of the item in the drop-down.
            /// </summary>
            public Rectangle DisplayRectangle { get; set; }

            /// <summary>
            ///     Gets or sets the current bounds of the glyph section of the
            ///     item, which is clickable.
            /// </summary>
            public Rectangle GlyphRectangle { get; set; }

            /// <summary>
            ///     Gets or sets a reference to the bitmap shown beside this item,
            ///     containing the node's image, plus/minus glyph and lines.
            /// </summary>
            public Image Image { get; set; }

            /// <summary>
            ///     Gets the node represented by this item.
            /// </summary>
            public ComboTreeNode Node { get; private set; }

            #endregion
        }

        #endregion

        #region Nested Type: ScrollBarInfo

        /// <summary>
        ///     Represents the information needed to draw and interact with the scroll
        ///     bar.
        /// </summary>
        private class ScrollBarInfo
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the bounds of the entire scrollbar.
            /// </summary>
            public Rectangle DisplayRectangle { get; set; }

            /// <summary>
            ///     Gets or sets the bounds of the down arrow.
            /// </summary>
            public Rectangle DownArrow { get; set; }

            /// <summary>
            ///     Gets or sets the bounds of the 'thumb' button.
            /// </summary>
            public Rectangle Thumb { get; set; }

            /// <summary>
            ///     Gets or sets the bounds of the up arrow.
            /// </summary>
            public Rectangle UpArrow { get; set; }

            #endregion
        }

        #endregion
    }
}