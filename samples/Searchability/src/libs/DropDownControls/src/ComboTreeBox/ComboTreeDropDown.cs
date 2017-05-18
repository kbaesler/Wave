// A ComboBox with a TreeView Drop-Down
// Bradley Smith - 2010/11/04 (updated 2015/03/28)

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.IO;
using System.Drawing.Drawing2D;

/// <summary>
/// Represents the dropdown portion of the ComboTreeBox control. The nodes are displayed in a 
/// manner similar to the TreeView control.
/// </summary>
[ToolboxItem(false), DesignerCategory("")]
public class ComboTreeDropDown : ToolStripDropDown {

	const int GLYPH_SIZE = 16;
	const int INDENT_WIDTH = 16;
	const int MIN_ITEM_HEIGHT = 16;
	const int MIN_THUMB_HEIGHT = 16;
	const int SCROLLBAR_WIDTH = 17;
	readonly Size SCROLLBUTTON_SIZE = new Size(SCROLLBAR_WIDTH, SCROLLBAR_WIDTH);
	const TextFormatFlags TEXT_FORMAT_FLAGS = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;

	private static Bitmap _collapsed;
	private static Bitmap _expanded;

	private Dictionary<BitmapInfo, Image> _bitmaps;
	private int _dropDownHeight;
	private int _highlightedItemIndex;
	private Rectangle _interior;
	private int _itemHeight;
	private int _numItemsDisplayed;
	private bool _scrollBarVisible;
	private bool _nodeLinesNeeded;
	private ScrollBarInfo _scrollBar;
	private bool _scrollDragging;
	private int _scrollOffset;
	private Timer _scrollRepeater;
	private ComboTreeBox _sourceControl;
	private List<NodeInfo> _visibleItems;

	/// <summary>
	/// Gets the collapsed (+) glyph to paint on the dropdown.
	/// </summary>
	private Image Collapsed {
		get {
			if (_collapsed == null) {
				_collapsed = new Bitmap(16, 16);
				Graphics g = Graphics.FromImage(_collapsed);
				Rectangle r = new Rectangle(4, 4, 8, 8);
				g.FillRectangle(Brushes.White, r);
				g.DrawRectangle(Pens.Gray, r);
				g.DrawLine(Pens.Black, Point.Add(r.Location, new Size(2, 4)), Point.Add(r.Location, new Size(6, 4)));
				g.DrawLine(Pens.Black, Point.Add(r.Location, new Size(4, 2)), Point.Add(r.Location, new Size(4, 6)));
			}
			return _collapsed;
		}
	}
	/// <summary>
	/// Removes extraneous default padding from the dropdown.
	/// </summary>
	protected override Padding DefaultPadding {
		get {
			return new Padding(0, 1, 0, 1);
		}
	}
	/// <summary>
	/// Gets or sets the maximum height of the dropdown.
	/// </summary>
	public int DropDownHeight {
		get { return _dropDownHeight; }
		set {
			_dropDownHeight = value;
			UpdateVisibleItems();
		}
	}
	/// <summary>
	/// Gets the expanded (-) glyph to paint on the dropdown.
	/// </summary>
	private Image Expanded {
		get {
			if (_expanded == null) {
				_expanded = new Bitmap(16, 16);
				Graphics g = Graphics.FromImage(_expanded);
				Rectangle r = new Rectangle(4, 4, 8, 8);
				g.FillRectangle(Brushes.White, r);
				g.DrawRectangle(Pens.Gray, r);
				g.DrawLine(Pens.Black, Point.Add(r.Location, new Size(2, 4)), Point.Add(r.Location, new Size(6, 4)));
			}
			return _expanded;
		}
	}
	/// <summary>
	/// Gets or sets the first visible ComboTreeNode in the drop-down portion of the control.
	/// </summary>
	public ComboTreeNode TopNode {
		get {
			return _visibleItems[_scrollOffset].Node;
		}
		set {
			for (int i = 0; i < _visibleItems.Count; i++) {
				if (_visibleItems[i].Node == value) {
					if ((i < _scrollOffset) || (i >= (_scrollOffset + _numItemsDisplayed))) {
						_scrollOffset = Math.Min(Math.Max(0, i - _numItemsDisplayed + 1), _visibleItems.Count - _numItemsDisplayed);
						UpdateScrolling();
					}
					break;
				}
			}
		}
	}
	/// <summary>
	/// Gets the number of ComboTreeNodes visible in the drop-down portion of the control.
	/// </summary>
	public int VisibleCount {
		get {
			return _numItemsDisplayed;
		}
	}

	/// <summary>
	/// Initialises a new instance of ComboTreeDropDown and associates it with its parent ComboTreeBox.
	/// </summary>
	/// <param name="sourceControl"></param>
	public ComboTreeDropDown(ComboTreeBox sourceControl) {
		_visibleItems = new List<NodeInfo>();
		_bitmaps = new Dictionary<BitmapInfo, Image>();
		_scrollBar = new ScrollBarInfo();
		AutoSize = false;
		this._sourceControl = sourceControl;
        RenderMode = ToolStripRenderMode.System;
		BackColor = Color.White;
		_dropDownHeight = 150;
		_itemHeight = MIN_ITEM_HEIGHT;
		Items.Add("");		

		_scrollRepeater = new Timer();
		_scrollRepeater.Tick += new EventHandler(scrollRepeater_Tick);
	}

	/// <summary>
	/// Generates a bitmap to display beside the ToolStripItem representation of the specified node.
	/// </summary>
	/// <param name="bitmapInfo"></param>
	/// <param name="nodeImage"></param>
	/// <returns></returns>
	private Image GenerateBitmap(BitmapInfo bitmapInfo, Image nodeImage) {
		int indentation = INDENT_WIDTH * bitmapInfo.NodeDepth;
		int halfIndent = INDENT_WIDTH / 2;
		int halfHeight = _itemHeight / 2;

		int bmpWidth = indentation;
		
		if (_nodeLinesNeeded)
			bmpWidth += INDENT_WIDTH;
		else
			bmpWidth += 1;

		if (_sourceControl.ShowCheckBoxes)
			bmpWidth += 16;
		else if (nodeImage != null)
			bmpWidth += nodeImage.Width;

		if (bmpWidth == 0) return null;

		// create a bitmap that will be composed of the node's image and the glyphs/lines/indentation
		Bitmap composite = new Bitmap(bmpWidth, _itemHeight);
		using (Graphics g = Graphics.FromImage(composite)) {
			if (_nodeLinesNeeded) {
				using (Pen dotted = new Pen(Color.Gray)) {
					dotted.DashStyle = DashStyle.Dot;

					// horizontal dotted line
					g.DrawLine(dotted, indentation + halfIndent, halfHeight, indentation + INDENT_WIDTH, halfHeight);

					// vertical dotted line to peers
					g.DrawLine(dotted, indentation + halfIndent, bitmapInfo.IsFirst ? halfHeight : 0, indentation + halfIndent, bitmapInfo.IsLastPeer ? halfHeight : _itemHeight);

					// vertical dotted line to subtree
					if (bitmapInfo.NodeExpanded) g.DrawLine(dotted, INDENT_WIDTH + indentation + halfIndent, halfHeight, INDENT_WIDTH + indentation + halfIndent, _itemHeight);

					// outer vertical dotted lines
					for (int i = 0; i < bitmapInfo.VerticalLines.Length; i++) {
						if (bitmapInfo.VerticalLines[i]) {
							int parentIndent = (INDENT_WIDTH * (bitmapInfo.NodeDepth - (i + 1)));
							g.DrawLine(dotted, parentIndent + halfIndent, 0, parentIndent + halfIndent, _itemHeight);
						}
					}
				}
			}

			if (_sourceControl.ShowCheckBoxes) {
				// leave space for checkbox glyph
			}
			else if (nodeImage != null) {
				// composite the image associated with node (appears at far right)
				g.DrawImage(nodeImage, new Rectangle(
					INDENT_WIDTH + indentation,
					composite.Height / 2 - nodeImage.Height / 2,
					nodeImage.Width,
					nodeImage.Height
				));
			}

			// render plus/minus glyphs
			if (bitmapInfo.HasChildren) {
				Rectangle glyphBounds = new Rectangle(indentation, composite.Height / 2 - GLYPH_SIZE / 2, GLYPH_SIZE, GLYPH_SIZE);
				VisualStyleElement elem = bitmapInfo.NodeExpanded ? VisualStyleElement.TreeView.Glyph.Opened : VisualStyleElement.TreeView.Glyph.Closed;

				if (_sourceControl.DrawWithVisualStyles && VisualStyleRenderer.IsSupported && VisualStyleRenderer.IsElementDefined(elem)) {
					// visual style support, render using visual styles
					VisualStyleRenderer r = new VisualStyleRenderer(elem);
					r.DrawBackground(g, glyphBounds);
				}
				else {
					// no visual style support, render using bitmap
					Image glyph = bitmapInfo.NodeExpanded ? Expanded : Collapsed;
					g.DrawImage(glyph, glyphBounds);
				}
			}
		}

		return composite;
	}

	/// <summary>
	/// Determines how to draw a scrollbar button.
	/// </summary>
	/// <param name="bounds"></param>
	/// <returns></returns>
	private ButtonState GetButtonState(Rectangle bounds) {
		ButtonState state = ButtonState.Normal;
		if (bounds.Contains(PointToClient(Cursor.Position)) && !_scrollDragging) {
			if ((MouseButtons & MouseButtons.Left) == MouseButtons.Left) state = ButtonState.Pushed;
		}
		return state;
	}

	/// <summary>
	/// Determines how to draw a checkbox glyph.
	/// </summary>
	/// <param name="checkState"></param>
	/// <param name="bounds"></param>
	/// <returns></returns>
	private CheckBoxState GetCheckBoxState(CheckState checkState, Rectangle bounds) {
		bool mouseOver = bounds.Contains(PointToClient(Cursor.Position));
		bool mouseDown = ((MouseButtons & MouseButtons.Left) == MouseButtons.Left);

		switch (checkState) {
			case CheckState.Checked:
				if (mouseOver && mouseDown)
					return CheckBoxState.CheckedPressed;
				else if (mouseOver)
					return CheckBoxState.CheckedHot;
				else
					return CheckBoxState.CheckedNormal;

			case CheckState.Indeterminate:
				if (mouseOver && mouseDown)
					return CheckBoxState.MixedPressed;
				else if (mouseOver)
					return CheckBoxState.MixedHot;
				else
					return CheckBoxState.MixedNormal;

			default:
				if (mouseOver && mouseDown)
					return CheckBoxState.UncheckedPressed;
				else if (mouseOver)
					return CheckBoxState.UncheckedHot;
				else
					return CheckBoxState.UncheckedNormal;
		}
	}

	/// <summary>
	/// Returns the ComboTreeNodeCollection to which the specified node belongs.
	/// </summary>
	/// <param name="node"></param>
	/// <returns></returns>
	private ComboTreeNodeCollection GetCollectionContainingNode(ComboTreeNode node) {
		return (node.Parent != null) ? node.Parent.Nodes : _sourceControl.Nodes;
	}

	/// <summary>
	/// Determines all of the parameters for drawing the bitmap beside the 
	/// specified node. If they represent a unique combination, the bitmap is 
	/// generated and returned. Otherwise, the appropriate cached bitmap is 
	/// returned.
	/// </summary>
	/// <param name="node"></param>
	/// <returns></returns>
	private Image GetItemBitmap(ComboTreeNode node) {
		BitmapInfo bitmapInfo = new BitmapInfo();
	
		// the following factors determine the bitmap drawn:
		ComboTreeNodeCollection collection = GetCollectionContainingNode(node);
		bitmapInfo.HasChildren = (node.Nodes.Count > 0);
		bitmapInfo.IsLastPeer = (collection.IndexOf(node) == (collection.Count - 1));
		bitmapInfo.IsFirst = (node == _sourceControl.Nodes[0]);
		bitmapInfo.NodeDepth = node.Depth;
		bitmapInfo.NodeExpanded = node.Expanded && bitmapInfo.HasChildren;
		bitmapInfo.ImageIndex = bitmapInfo.NodeExpanded ? node.ExpandedImageIndex : node.ImageIndex;
		bitmapInfo.ImageKey = bitmapInfo.NodeExpanded ? node.ExpandedImageKey : node.ImageKey;

		bitmapInfo.VerticalLines = new bool[bitmapInfo.NodeDepth];
		ComboTreeNode parent = node;
		int i = 0;
		while ((parent = parent.Parent) != null) {
			// vertical line required if parent is expanded (and not last peer)
			ComboTreeNodeCollection parentCollection = GetCollectionContainingNode(parent);
			bitmapInfo.VerticalLines[i] = (parent.Expanded && (parentCollection.IndexOf(parent) != (parentCollection.Count - 1)));
			i++;
		}

		if (_bitmaps.ContainsKey(bitmapInfo))
			return _bitmaps[bitmapInfo];
		else
			return (_bitmaps[bitmapInfo] = GenerateBitmap(bitmapInfo, _sourceControl.GetNodeImage(node)));
	}

	/// <summary>
	/// Determines how to draw the main part of the scrollbar.
	/// </summary>
	/// <param name="bounds"></param>
	/// <returns></returns>
	private ScrollBarState GetScrollBarState(Rectangle bounds) {
		ScrollBarState state = ScrollBarState.Normal;
		Point local = PointToClient(Cursor.Position);
		if (bounds.Contains(local)
			&& !_scrollDragging
			&& !_scrollBar.DownArrow.Contains(local)
			&& !_scrollBar.UpArrow.Contains(local)
			&& !_scrollBar.Thumb.Contains(local)) {

			if ((MouseButtons & MouseButtons.Left) == MouseButtons.Left)
				state = ScrollBarState.Pressed;
			else
				state = ScrollBarState.Hot;
		}
		return state;
	}

	/// <summary>
	/// Determines how to draw the down arrow on the scrollbar.
	/// </summary>
	/// <returns></returns>
	private ScrollBarArrowButtonState GetScrollBarStateDown() {
		ScrollBarArrowButtonState state = ScrollBarArrowButtonState.DownNormal;
		if (_scrollBar.DownArrow.Contains(PointToClient(Cursor.Position)) && !_scrollDragging) {
			if ((MouseButtons & MouseButtons.Left) == MouseButtons.Left)
				state = ScrollBarArrowButtonState.DownPressed;
			else
				state = ScrollBarArrowButtonState.DownHot;
		}
		return state;
	}

	/// <summary>
	/// Determines how to draw the up arrow on the scrollbar.
	/// </summary>
	/// <returns></returns>
	private ScrollBarArrowButtonState GetScrollBarStateUp() {
		ScrollBarArrowButtonState state = ScrollBarArrowButtonState.UpNormal;
		if (_scrollBar.UpArrow.Contains(PointToClient(Cursor.Position)) && !_scrollDragging) {
			if ((MouseButtons & MouseButtons.Left) == MouseButtons.Left)
				state = ScrollBarArrowButtonState.UpPressed;
			else
				state = ScrollBarArrowButtonState.UpHot;
		}
		return state;
	}

	/// <summary>
	/// Determines how to draw the 'thumb' button on the scrollbar.
	/// </summary>
	/// <returns></returns>
	private ScrollBarState GetScrollBarThumbState() {
		ScrollBarState state = ScrollBarState.Normal;
		if (_scrollBar.Thumb.Contains(PointToClient(Cursor.Position))) {
			if ((MouseButtons & MouseButtons.Left) == MouseButtons.Left)
				state = ScrollBarState.Pressed;
			else
				state = ScrollBarState.Hot;
		}
		return state;
	}

	/// <summary>
	/// Registers the arrow keys as input keys.
	/// </summary>
	/// <param name="keyData"></param>
	/// <returns></returns>
	protected override bool IsInputKey(Keys keyData) {
		switch (keyData) {
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
	/// Updates the status of the dropdown on the owning ComboTreeBox control.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnClosed(ToolStripDropDownClosedEventArgs e) {
		base.OnClosed(e);

		// update DroppedDown on ComboTreeBox after close
		_sourceControl.SetDroppedDown(false, false);
	}

	/// <summary>
	/// Prevents the clicking of items from closing the dropdown.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnClosing(ToolStripDropDownClosingEventArgs e) {
		if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked) e.Cancel = true;
		if (e.CloseReason == ToolStripDropDownCloseReason.AppClicked) {
			if (_sourceControl.ClientRectangle.Contains(_sourceControl.PointToClient(Cursor.Position))) e.Cancel = true;
		}

		base.OnClosing(e);
	}

	/// <summary>
	/// Updates the font on the items when the drop-down's font changes.
	/// </summary>
	/// <param name="e"></param>
    protected override void OnFontChanged(EventArgs e) {
        base.OnFontChanged(e);
		_itemHeight = Math.Max(MIN_ITEM_HEIGHT, Font.Height);
    }

	#region Keyboard Events

	/// <summary>
    /// Handles keyboard shortcuts.
    /// </summary>
    /// <param name="e"></param>
	protected override void OnKeyPress(KeyPressEventArgs e) {
		e.Handled = true;

		if (e.KeyChar == '+') {
			NodeInfo info = _visibleItems[_highlightedItemIndex];
			if (info.Node.Nodes.Count > 0) {
				info.Node.Expanded = true;
				UpdateVisibleItems();
			}
		}
		else if (e.KeyChar == '-') {
			NodeInfo info = _visibleItems[_highlightedItemIndex];
			if (info.Node.Nodes.Count > 0) {
				info.Node.Expanded = false;
				UpdateVisibleItems();
			}
		}
		if (e.KeyChar == '*') {
			_sourceControl.ExpandAll();
			UpdateVisibleItems();
		}
		else if (e.KeyChar == '/') {
			_sourceControl.CollapseAll();
			UpdateVisibleItems();
		}
		else {
			e.Handled = false;
		}

		base.OnKeyPress(e);
	}

	/// <summary>
	/// Handles keyboard shortcuts.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnKeyDown(KeyEventArgs e) {
		e.Handled = e.SuppressKeyPress = true;

		if ((e.KeyCode == Keys.Enter) || (e.Alt && (e.KeyCode == Keys.Up))) {
		    _sourceControl.SelectedNode = _visibleItems[_highlightedItemIndex].Node;
		    if (!_sourceControl.ShowCheckBoxes) Close();
		}
		else if ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Left)) {
			_highlightedItemIndex = Math.Max(0, _highlightedItemIndex - 1);
			_sourceControl.SelectedNode = _visibleItems[_highlightedItemIndex].Node;
			ScrollToHighlighted(true);
			Refresh();
		}
		else if ((e.KeyCode == Keys.Down) || (e.KeyCode == Keys.Right)) {
			_highlightedItemIndex = Math.Min(_highlightedItemIndex + 1, _visibleItems.Count - 1);
			_sourceControl.SelectedNode = _visibleItems[_highlightedItemIndex].Node;
			ScrollToHighlighted(false);
			Refresh();
		}
		else if (e.KeyCode == Keys.Home) {
			_highlightedItemIndex = _scrollOffset = 0;
			UpdateScrolling();
			Invalidate();
		}
		else if (e.KeyCode == Keys.End) {
			_scrollOffset = _visibleItems.Count - _numItemsDisplayed;
			_highlightedItemIndex = _visibleItems.Count - 1;
			UpdateScrolling();
			Invalidate();
		}
		else if (e.KeyCode == Keys.PageDown) {
			_scrollOffset = Math.Min(_scrollOffset + _numItemsDisplayed, _visibleItems.Count - _numItemsDisplayed);
			_highlightedItemIndex = Math.Min(_scrollOffset + _numItemsDisplayed - 1, _visibleItems.Count - 1);
			UpdateScrolling();
			Refresh();
		}
		else if (e.KeyCode == Keys.PageUp) {
			_highlightedItemIndex = _scrollOffset = Math.Max(_scrollOffset - _numItemsDisplayed, 0);
			UpdateScrolling();
			Refresh();
		}
		else if ((e.KeyCode == Keys.F4) || (e.KeyCode == Keys.Escape)) {
			Close();
		}
		else {
			e.Handled = e.SuppressKeyPress = false;
		}

		base.OnKeyDown(e);
	}

	#endregion

	#region Mouse Events

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Control.MouseWheel"/> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
    protected override void OnMouseWheel(MouseEventArgs e){
        base.OnMouseWheel(e);

        HandledMouseEventArgs he = (HandledMouseEventArgs)e;
        he.Handled = true;

        if (he.Delta > 0)
        {
            ScrollDropDown(-1);
        }
        else
        {
            ScrollDropDown(1);
        }
    }

    /// <summary>
	/// Handles dragging of the scrollbar and hot-tracking in response to movement of the mouse.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseMove(MouseEventArgs e) {
		base.OnMouseMove(e);

		// dragging with the scrollbar's 'thumb' button
		if (_scrollDragging) {
			double availableHeight = _scrollBar.DisplayRectangle.Height - (2 * SCROLLBUTTON_SIZE.Height) - _scrollBar.Thumb.Height;
			double position = Math.Min(e.Location.Y - _scrollBar.DisplayRectangle.Top - SCROLLBUTTON_SIZE.Height - (_scrollBar.Thumb.Height / 2), availableHeight);

			// measure the scroll offset based on the location of the mouse pointer, relative to the scrollbar's bounds
			_scrollOffset = Math.Max(0, Math.Min(
				(int)((position / availableHeight) * (double)(_visibleItems.Count - _numItemsDisplayed)),
				(_visibleItems.Count - _numItemsDisplayed)
			));
			
			UpdateScrolling();
			Refresh();
			return;
		}

		// moving the mouse over the scrollbar
		if (_scrollBarVisible && _scrollBar.DisplayRectangle.Contains(e.Location)) {
			Invalidate();
			return;
		}

		// not within scrollbar's bounds, end auto-repeat behaviour
		_scrollRepeater.Stop();

		// hit-test each displayed item's bounds to determine the highlighted item
		for (int i = _scrollOffset; i < (_scrollOffset + _numItemsDisplayed); i++) {
			if (_visibleItems[i].DisplayRectangle.Contains(e.Location)) {
				_highlightedItemIndex = i;
				Invalidate();
				break;
			}
		}
	}

	/// <summary>
	/// Handles scrolling in response to the left mouse button being clicked.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseDown(MouseEventArgs e) {
		base.OnMouseDown(e);

		if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

		// mouse pointer within the scrollbar's bounds
		if (_scrollBarVisible && _scrollBar.DisplayRectangle.Contains(e.Location)) {
			if (e.Y > _scrollBar.Thumb.Bottom) {
				// any point below the thumb button requires scrolling - on bar = pagedown, on button = next
				int step = (_scrollBar.DownArrow.Contains(e.Location)) ? 1 : _numItemsDisplayed;
				ScrollDropDown(step);

				// if the button is held, start auto-repeat behaviour
				if (!_scrollRepeater.Enabled) {
					_scrollRepeater.Interval = 250;
					_scrollRepeater.Start();
				}
				return;
			}
			else if (e.Y < _scrollBar.Thumb.Top) {
				// any point above the thumb button requires scrolling - on bar = pagedown, on button = next
				int step = (_scrollBar.UpArrow.Contains(e.Location)) ? 1 : _numItemsDisplayed;
				ScrollDropDown(-step);

				// if the button is held, start auto-repeat behaviour
				if (!_scrollRepeater.Enabled) {
					_scrollRepeater.Interval = 250;
					_scrollRepeater.Start();
				}
				return;
			}
			else if (_scrollBar.Thumb.Contains(e.Location)) {
				// assume the thumb button is being dragged
				_scrollDragging = true;
			}

			Invalidate();
		}
	}

	/// <summary>
	/// Disengages dragging of the scrollbar and handles hot-tracking in 
	/// response to the mouse button being released.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseUp(MouseEventArgs e) {
		base.OnMouseUp(e);

		_scrollRepeater.Stop();
		_scrollDragging = false;

		if (_scrollBarVisible && _scrollBar.DisplayRectangle.Contains(e.Location)) {
			Invalidate();
			return;
		}
	}

	/// <summary>
	/// Handles the expand/collapse of nodes and selection in response to the 
	/// mouse being clicked.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseClick(MouseEventArgs e) {
		base.OnMouseClick(e);

		if (_scrollDragging) return;

		if (e.Button == System.Windows.Forms.MouseButtons.Left) {
			for (int i = _scrollOffset; i < (_scrollOffset + _numItemsDisplayed); i++) {
				NodeInfo info = _visibleItems[i];

				if (info.DisplayRectangle.Contains(e.Location)) {
					if (info.GlyphRectangle.Contains(e.Location)) {
						info.Node.Expanded = !info.Node.Expanded;
						UpdateVisibleItems();
					}
					else if (_sourceControl.ShowCheckBoxes && info.CheckRectangle.Contains(e.Location)) {
						if (info.Node.CheckState == CheckState.Unchecked)
							info.Node.CheckState = CheckState.Checked;
						else if ((info.Node.CheckState == CheckState.Checked) && _sourceControl.ThreeState)
							info.Node.CheckState = CheckState.Indeterminate;
						else
							info.Node.CheckState = CheckState.Unchecked;

						Invalidate();
					}
					else {
						_sourceControl.SelectedNode = _visibleItems[i].Node;
						if (!_sourceControl.ShowCheckBoxes) Close();
					}
					break;
				}
			}
		}
	}

	/// <summary>
	/// Terminates dragging of the scrollbar in response to the mouse 
	/// returning to the control.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseEnter(EventArgs e) {
		base.OnMouseEnter(e);
		if ((MouseButtons & MouseButtons.Left) != MouseButtons.Left) _scrollDragging = false;
	}

	/// <summary>
	/// Terminates dragging of the scrollbar in response to the mouse leaving 
	/// the control.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseLeave(EventArgs e) {
		base.OnMouseLeave(e);
		if ((MouseButtons & MouseButtons.Left) != MouseButtons.Left) _scrollDragging = false;
	}

	#endregion

	/// <summary>
	/// Paints the drop-down, including all items within the scrolled region 
	/// and, if appropriate, the scrollbar.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnPaint(PaintEventArgs e) {
		base.OnPaint(e);

		if (_scrollBarVisible) {
			Rectangle upper = new Rectangle(_scrollBar.DisplayRectangle.Left, _scrollBar.DisplayRectangle.Top, _scrollBar.DisplayRectangle.Width, _scrollBar.Thumb.Top - _scrollBar.DisplayRectangle.Top);
			Rectangle lower = new Rectangle(_scrollBar.DisplayRectangle.Left, _scrollBar.Thumb.Bottom, _scrollBar.DisplayRectangle.Width, _scrollBar.DisplayRectangle.Bottom - _scrollBar.Thumb.Bottom);

			if (_sourceControl.DrawWithVisualStyles && ScrollBarRenderer.IsSupported) {
				ScrollBarRenderer.DrawUpperVerticalTrack(e.Graphics, upper, GetScrollBarState(upper));
				ScrollBarRenderer.DrawLowerVerticalTrack(e.Graphics, lower, GetScrollBarState(lower));
				ScrollBarRenderer.DrawArrowButton(e.Graphics, _scrollBar.UpArrow, GetScrollBarStateUp());
				ScrollBarRenderer.DrawArrowButton(e.Graphics, _scrollBar.DownArrow, GetScrollBarStateDown());
				ScrollBarRenderer.DrawVerticalThumb(e.Graphics, _scrollBar.Thumb, GetScrollBarThumbState());
				ScrollBarRenderer.DrawVerticalThumbGrip(e.Graphics, _scrollBar.Thumb, GetScrollBarThumbState());
			}
			else {
				Rectangle bounds = _scrollBar.DisplayRectangle;
				bounds.Offset(1, 0);
				Rectangle up = _scrollBar.UpArrow;
				up.Offset(1, 0);
				Rectangle down = _scrollBar.DownArrow;
				down.Offset(1, 0);
				Rectangle thumb = _scrollBar.Thumb;
				thumb.Offset(1, 0);

				using (HatchBrush brush = new HatchBrush(HatchStyle.Percent50, SystemColors.ControlLightLight, SystemColors.Control)) {
					e.Graphics.FillRectangle(brush, bounds);
				}

				ControlPaint.DrawScrollButton(e.Graphics, up, ScrollButton.Up, GetButtonState(_scrollBar.UpArrow));
				ControlPaint.DrawScrollButton(e.Graphics, down, ScrollButton.Down, GetButtonState(_scrollBar.DownArrow));
				ControlPaint.DrawButton(e.Graphics, thumb, ButtonState.Normal);
			}
		}

		for (int i = _scrollOffset; i < (_scrollOffset + _numItemsDisplayed); i++) {
			bool highlighted = ((_highlightedItemIndex == i) && !_sourceControl.ShowCheckBoxes);
			NodeInfo item = _visibleItems[i];

			// background
			if (highlighted) e.Graphics.FillRectangle(SystemBrushes.Highlight, item.DisplayRectangle);

			// image and glyphs
			if (item.Image != null) {
				Rectangle imgBounds = new Rectangle(item.DisplayRectangle.Location, item.Image.Size);
				e.Graphics.DrawImage(item.Image, imgBounds);

				if (_sourceControl.ShowCheckBoxes) {
					CheckBoxState state = GetCheckBoxState(item.Node.CheckState, item.CheckRectangle);
					Size chkSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, state);
					CheckBoxRenderer.DrawCheckBox(e.Graphics, Point.Add(item.CheckRectangle.Location, new Size((item.CheckRectangle.Width - chkSize.Width) / 2, (item.CheckRectangle.Height - chkSize.Height) / 2)), state);
				}
			}

			Rectangle textBounds = new Rectangle(item.DisplayRectangle.X + item.Image.Width + 2, item.DisplayRectangle.Y, item.DisplayRectangle.Width - item.Image.Width - 4, _itemHeight);

			using (Font font = new Font(Font, _visibleItems[i].Node.FontStyle)) {
				TextRenderer.DrawText(e.Graphics, item.Node.Text, font, textBounds, highlighted ? SystemColors.HighlightText : ForeColor, TEXT_FORMAT_FLAGS);
			}

            if (highlighted && _sourceControl.Focused && _sourceControl.ShowsFocusCues) ControlPaint.DrawFocusRectangle(e.Graphics, item.DisplayRectangle);
		}
	}

	/// <summary>
	/// Displays the dropdown beneath its owning ComboTreeBox control.
	/// </summary>
	public void Open() {
		// the selected node(s) must have a clear path (i.e. all parents expanded)
		if (_sourceControl.ShowCheckBoxes) {
			foreach (ComboTreeNode node in _sourceControl.CheckedNodes) {
				ExpandTo(node);
			}
		}
		else if (_sourceControl.SelectedNode != null) {
			ExpandTo(_sourceControl.SelectedNode);
		}

		UpdateVisibleItems();

		// highlight and scroll to the selected node
		ComboTreeNode scrollToNode = null;
		if (_sourceControl.ShowCheckBoxes) {
			foreach (ComboTreeNode node in _sourceControl.CheckedNodes) {
				scrollToNode = node;
				break;
			}
		}
		else {
			scrollToNode = _sourceControl.SelectedNode;
		}

		if (scrollToNode != null) ScrollTo(scrollToNode);

		// show below the source control
		Show(_sourceControl, new Point(0, _sourceControl.ClientRectangle.Height));
	}

	/// <summary>
	/// Expands parent nodes until the specified node is visible.
	/// </summary>
	/// <param name="node"></param>
	private void ExpandTo(ComboTreeNode node) {
		while ((node = node.Parent) != null) node.Expanded = true;
	}

	/// <summary>
	/// Highlights and scrolls to the specified node.
	/// </summary>
	/// <param name="node"></param>
	private void ScrollTo(ComboTreeNode node) {
		for (int i = 0; i < _visibleItems.Count; i++) {
			if (_visibleItems[i].Node == node) {
				_highlightedItemIndex = i;
				if ((_highlightedItemIndex < _scrollOffset) || (_highlightedItemIndex >= (_scrollOffset + _numItemsDisplayed))) {
					_scrollOffset = Math.Min(Math.Max(0, _highlightedItemIndex - _numItemsDisplayed + 1), _visibleItems.Count - _numItemsDisplayed);
					UpdateScrolling();
				}
				break;
			}
		}
	}

	/// <summary>
	/// Scrolls the drop-down up or down by the specified number of items.
	/// </summary>
	/// <param name="offset"></param>
	public void ScrollDropDown(int offset) {
		if (offset < 0) {
			// up/left
			_scrollOffset = Math.Max(_scrollOffset + offset, 0);
			UpdateScrolling();
			Invalidate();
		}
		else if (offset > 0) {
			// down/right
			_scrollOffset = Math.Min(_scrollOffset + offset, _visibleItems.Count - _numItemsDisplayed);
			UpdateScrolling();
			Invalidate();
		}
	}

	/// <summary>
	/// Scrolls the drop-down so as to ensure that the highlighted item is at 
	/// either the top or bottom of the scrolled region.
	/// </summary>
	/// <param name="highlightedAtTop"></param>
	private void ScrollToHighlighted(bool highlightedAtTop) {
		if ((_highlightedItemIndex < _scrollOffset) || (_highlightedItemIndex >= (_scrollOffset + _numItemsDisplayed))) {
			if (highlightedAtTop)
				_scrollOffset = Math.Min(_highlightedItemIndex, _visibleItems.Count - _numItemsDisplayed);
			else
				_scrollOffset = Math.Min(Math.Max(0, _highlightedItemIndex - _numItemsDisplayed + 1), _visibleItems.Count - _numItemsDisplayed);

			UpdateScrolling();
		}
	}

	/// <summary>
	/// Updates the items in the scrolled region. This method is called 
	/// whenever the scroll offset is changed.
	/// </summary>
	private void UpdateScrolling() {
		if (_scrollBarVisible) {
			// calculate the bounds of the scrollbar's 'thumb' button
			int availableHeight = _scrollBar.DisplayRectangle.Height - (2 * SCROLLBUTTON_SIZE.Height);

			double percentSize = (double)_numItemsDisplayed / (double)_visibleItems.Count;
			int size = Math.Max((int)(percentSize * (double)availableHeight), MIN_THUMB_HEIGHT);
			int diff = Math.Max(0, MIN_THUMB_HEIGHT - (int)(percentSize * (double)availableHeight));

			double percentStart = (double)_scrollOffset / (double)_visibleItems.Count;
			int start = Math.Min((int)Math.Ceiling(percentStart * (double)(availableHeight - diff)), availableHeight - MIN_THUMB_HEIGHT);

			_scrollBar.Thumb = new Rectangle(new Point(_scrollBar.DisplayRectangle.X, _scrollBar.DisplayRectangle.Top + SCROLLBUTTON_SIZE.Height + start), new Size(SCROLLBAR_WIDTH, size));
		}

		// calculate display rectangles and assign images for each item in the scroll range
		for (int i = _scrollOffset; i < (_scrollOffset + _numItemsDisplayed); i++) {
			NodeInfo info = _visibleItems[i];
			if (info.Image == null) info.Image = GetItemBitmap(info.Node);
			info.DisplayRectangle = new Rectangle(_interior.X, _interior.Y + (_itemHeight * (i - _scrollOffset)), _interior.Width, _itemHeight);
			int indentation = (info.Node.Depth * INDENT_WIDTH);

			Size imageSize = (info.Image != null) ? info.Image.Size : Size.Empty;

			Image nodeImage = _sourceControl.GetNodeImage(info.Node);
			int nodeImageWidth = 0;
			if (_sourceControl.ShowCheckBoxes) {
				nodeImageWidth = 16;
				info.CheckRectangle = new Rectangle(imageSize.Width - nodeImageWidth, info.DisplayRectangle.Top, nodeImageWidth, imageSize.Height);
			}
			else if (nodeImage != null) {
				nodeImageWidth = nodeImage.Width;
			}

			info.GlyphRectangle = new Rectangle(indentation, info.DisplayRectangle.Top, imageSize.Width - indentation - nodeImageWidth, imageSize.Height);
		}
	}

	/// <summary>
	/// Releases resources used by the bitmap cache.
	/// </summary>
	private void ClearBitmapCache() {
		foreach (Image img in _bitmaps.Values) {
			if (img != null) img.Dispose();
		}
		_bitmaps.Clear();
	}

	/// <summary>
	/// Regenerates the items on the dropdown. This method is called whenever 
	/// a significant change occurs to the dropdown, such as a change in the 
	/// tree or changes to the layout of the owning control.
	/// </summary>
	internal void UpdateVisibleItems() {
		SuspendLayout();

		ClearBitmapCache();

		_nodeLinesNeeded = _sourceControl.NodeLinesNeeded;

		// populate the collection with the displayable items only
		_visibleItems.Clear();
		foreach (ComboTreeNode node in _sourceControl.AllNodes) {
			if (_sourceControl.IsNodeVisible(node)) _visibleItems.Add(new NodeInfo(node));
		}

		_highlightedItemIndex = Math.Max(0, Math.Min(_highlightedItemIndex, _visibleItems.Count - 1));

		_numItemsDisplayed = Math.Min((_dropDownHeight / _itemHeight) + 1, _visibleItems.Count);
		int maxHeight = ((((_dropDownHeight - 2) / _itemHeight) + 1) * _itemHeight) + 2;

		Size = new Size(_sourceControl.ClientRectangle.Width, Math.Min(maxHeight, (_visibleItems.Count * _itemHeight) + 2));

		// represents the entire paintable area
		_interior = ClientRectangle;
		_interior.Inflate(-1, -1);

		_scrollBarVisible = (_numItemsDisplayed < _visibleItems.Count);
        _scrollOffset = Math.Max(0, Math.Min(_scrollOffset, (_visibleItems.Count - _numItemsDisplayed)));
        if (_scrollBarVisible) {
			_interior.Width -= 17;
			_scrollBar.DisplayRectangle = new Rectangle(_interior.Right, _interior.Top, 17, _interior.Height);
			_scrollBar.UpArrow = new Rectangle(_scrollBar.DisplayRectangle.Location, SCROLLBUTTON_SIZE);
			_scrollBar.DownArrow = new Rectangle(new Point(_scrollBar.DisplayRectangle.X, _scrollBar.DisplayRectangle.Bottom - 17), SCROLLBUTTON_SIZE);
		}

		UpdateScrolling();

		ResumeLayout();
		Invalidate();
	}

	/// <summary>
	/// Releases resources used by the component.
	/// </summary>
	/// <param name="disposing"></param>
	protected override void Dispose(bool disposing) {
		if (disposing) {
			ClearBitmapCache();
		}
		
		base.Dispose(disposing);
	}

	void scrollRepeater_Tick(object sender, EventArgs e) {
		// reduce the interval and simulate another click
		_scrollRepeater.Interval = 50;
		Point local = PointToClient(Cursor.Position);
		OnMouseDown(new MouseEventArgs(MouseButtons, 1, local.X, local.Y, 0));
	}

	#region Inner Classes

	/// <summary>
	/// Represents the information needed to draw and interact with a node in the drop-down.
	/// </summary>
	private class NodeInfo {

		/// <summary>
		/// Gets the node represented by this item.
		/// </summary>
		public ComboTreeNode Node {
			get;
			private set;
		}
		/// <summary>
		/// Gets or sets a reference to the bitmap shown beside this item, 
		/// containing the node's image, plus/minus glyph and lines.
		/// </summary>
		public Image Image {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the current bounds of the item in the drop-down.
		/// </summary>
		public Rectangle DisplayRectangle {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the current bounds of the glyph section of the 
		/// item, which is clickable.
		/// </summary>
		public Rectangle GlyphRectangle {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the current bounds of the checkbox section of the 
		/// item (if node checkboxes are displayed).
		/// </summary>
		public Rectangle CheckRectangle {
			get;
			set;
		}

		/// <summary>
		/// Creates a new instance of the NodeInfo class to represent the 
		/// specified ComboTreeNode.
		/// </summary>
		/// <param name="node"></param>
		public NodeInfo(ComboTreeNode node) {
			Node = node;
		}
	}

	/// <summary>
	/// Represents the information needed to draw and interact with the scroll 
	/// bar.
	/// </summary>
	private class ScrollBarInfo {

		/// <summary>
		/// Gets or sets the bounds of the entire scrollbar.
		/// </summary>
		public Rectangle DisplayRectangle {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the bounds of the up arrow.
		/// </summary>
		public Rectangle UpArrow {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the bounds of the down arrow.
		/// </summary>
		public Rectangle DownArrow {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the bounds of the 'thumb' button.
		/// </summary>
		public Rectangle Thumb {
			get;
			set;
		}
	}

	/// <summary>
	/// Represents the variables which determine the bitmap to draw beside an 
	/// item. In a drop-down with a large number of items, there may be only a
	/// small number of distinct bitmaps. This structure serves as a key to 
	/// aid in identifying the bitmap to use.
	/// </summary>
	private struct BitmapInfo : IEquatable<BitmapInfo> {

		/// <summary>
		/// Gets or sets whether the node has children. This is used to 
		/// determine if the plus/minus glyph is drawn.
		/// </summary>
		public bool HasChildren {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets whether the node is the last peer in its branch of 
		/// the tree. These nodes do not draw a connector to their successor.
		/// </summary>
		public bool IsLastPeer {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets whether the node is the first in the entire tree. The 
		/// very first node does not draw a connector to its predecessor.
		/// </summary>
		public bool IsFirst {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the index of the image in the ComboTreeNode's 
		/// ImageList component to draw beside this node.
		/// </summary>
		public int ImageIndex {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the name of the image in the ComboTreeNode's 
		/// ImageList component to draw beside this node.
		/// </summary>
		public string ImageKey {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the zero-based depth of the node in the tree. This is 
		/// used to calculate indents.
		/// </summary>
		public int NodeDepth {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets whether the node has children and is expanded. This 
		/// will cause a connector to be drawn to the sub-tree.
		/// </summary>
		public bool NodeExpanded {
			get;
			set;
		}
		/// <summary>
		/// Gets or sets whether outer vertical connectors are to be drawn for 
		/// each successive parent of the node.
		/// </summary>
		public bool[] VerticalLines {
			get;
			set;
		}

		#region IEquatable<BitmapInfo> Members

		/// <summary>
		/// Used as the comparison function in the bitmap cache; ensures that 
		/// bitmaps are only created for distinct combinations of these 
		/// variables.
		/// </summary>
		/// <param name="that"></param>
		/// <returns></returns>
		public bool Equals(BitmapInfo that) {
			if (this.HasChildren != that.HasChildren)
				return false;
			if (this.IsLastPeer != that.IsLastPeer)
				return false;
			if (this.IsFirst != that.IsFirst)
				return false;
			if (this.NodeDepth != that.NodeDepth)
				return false;
			if (this.NodeExpanded != that.NodeExpanded)
				return false;
			if (this.VerticalLines.Length != that.VerticalLines.Length)
				return false;
			if (this.ImageIndex != that.ImageIndex)
				return false;
			if (this.ImageKey != that.ImageKey)
				return false;

			for (int i = 0; i < VerticalLines.Length; i++) {
				if (this.VerticalLines[i] != that.VerticalLines[i]) return false;
			}

			return true;
		}

		#endregion
	}

	#endregion
}