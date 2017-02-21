// A ComboBox with a TreeView Drop-Down
// Bradley Smith - 2010/11/04

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.VisualStyles;
using BufferedPainting;

/// <summary>
/// Abstract base class for a control which behaves like a dropdown but does not contain 
/// logic for displaying a popup window.
/// </summary>
[Designer("System.Windows.Forms.Design.UpDownBaseDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ToolboxItem(false), DesignerCategory("")]
public abstract class DropDownControlBase : Control {

    const int CONTROL_HEIGHT = 7;
    const int DROPDOWNBUTTON_WIDTH = 17;
    
    private bool _drawWithVisualStyles;
    private Rectangle _dropDownButtonBounds;
    private bool _droppedDown;
    private BufferedPainter<ComboBoxState> _bufferedPainter;

	/// <summary>
	/// Determines whether to draw the control with visual styles.
	/// </summary>
	[DefaultValue(true), Description("Determines whether to draw the control with visual styles."), Category("Appearance")]
	public bool DrawWithVisualStyles {
		get { return _drawWithVisualStyles; }
		set {
			_drawWithVisualStyles = value;
			Invalidate();
		}
	}
	/// <summary>
	/// Opens or closes the dropdown portion of the control.
	/// </summary>
	[Browsable(false)]
	public virtual bool DroppedDown {
		get { return _droppedDown; }
		set {
			_droppedDown = value;
            _bufferedPainter.State = GetComboBoxState();
		}
	}
	/// <summary>
	/// Gets or sets the background color to use for this control.
	/// </summary>
	[DefaultValue(typeof(Color), "Window")]
	public override Color BackColor {
		get {
			return base.BackColor;
		}
		set {
			base.BackColor = value;
		}
	}
	/// <summary>
	/// Hides the BackgroundImage property on the designer.
	/// </summary>
	[Browsable(false)]
	public override Image BackgroundImage {
		get {
			return base.BackgroundImage;
		}
		set {
			base.BackgroundImage = value;
		}
	}
	/// <summary>
	/// Hides the BackgroundImageLayout property on the designer.
	/// </summary>
	[Browsable(false)]
	public override ImageLayout BackgroundImageLayout {
		get {
			return base.BackgroundImageLayout;
		}
		set {
			base.BackgroundImageLayout = value;
		}
	}

	/// <summary>
	/// Fired when the user clicks the dropdown button at the right edge of the control.
	/// </summary>
	[Description("Occurs when the user clicks the dropdown button at the right edge of the control.")]
	protected event EventHandler DropDownButtonClick;
	/// <summary>
	/// Fired when the drop-down portion of the control is displayed.
	/// </summary>
	[Description("Occurs when the drop-down portion of the control is displayed.")]
	public event EventHandler DropDown;
	/// <summary>
	/// Fired when the drop-down portion of the control is closed.
	/// </summary>
	[Description("Occurs when the drop-down portion of the control is closed.")]
	public event EventHandler DropDownClosed;
	/// <summary>
	/// Fired when the content of the editable portion of the control is painted.
	/// </summary>
	[Description("Occurs when the content of the editable portion of the control is painted.")]
	public event EventHandler<DropDownPaintEventArgs> PaintContent;

	/// <summary>
	/// Creates a new instance of DropDownControlBase.
	/// </summary>
	public DropDownControlBase() {
		// control styles
		DoubleBuffered = true;
		SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		SetStyle(ControlStyles.Opaque, true);
		SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
		SetStyle(ControlStyles.ResizeRedraw, true);
		SetStyle(ControlStyles.Selectable, true);
		SetStyle(ControlStyles.StandardClick, true);
		SetStyle(ControlStyles.UserPaint, true);

		// default values
        _drawWithVisualStyles = Application.RenderWithVisualStyles;
		BackColor = SystemColors.Window;

        // buffered painting
        _bufferedPainter = new BufferedPainter<ComboBoxState>(this);
        _bufferedPainter.PaintVisualState += new EventHandler<BufferedPaintEventArgs<ComboBoxState>>(bufferedPainter_PaintVisualState);
        _bufferedPainter.State = _bufferedPainter.DefaultState = ComboBoxState.Normal;
        _bufferedPainter.AddTransition(ComboBoxState.Normal, ComboBoxState.Hot, 250);
        _bufferedPainter.AddTransition(ComboBoxState.Hot, ComboBoxState.Normal, 350);
        _bufferedPainter.AddTransition(ComboBoxState.Pressed, ComboBoxState.Normal, 350);
	}

	/// <summary>
	/// Gets the bounds of the textbox portion of the control by subtracting the dropdown button bounds from the client rectangle.
	/// </summary>
	/// <returns></returns>
	private Rectangle GetTextBoxBounds() {
		return new Rectangle(0, 0, _dropDownButtonBounds.Left, ClientRectangle.Height);
	}

    /// <summary>
    /// Converts a ComboBoxState value into its equivalent PushButtonState value.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private PushButtonState GetPushButtonState(ComboBoxState state) {
        switch (state) {
            case ComboBoxState.Disabled:
                return PushButtonState.Disabled;
            case ComboBoxState.Hot:
                return PushButtonState.Hot;
            case ComboBoxState.Pressed:
                return PushButtonState.Pressed;
        }

        return PushButtonState.Normal;
    }

	/// <summary>
	/// Determines the state in which to render the textbox portion of the control (when using visual styles).
	/// </summary>
	/// <returns></returns>
	private ComboBoxState GetTextBoxState() {
		if (!Enabled)
			return ComboBoxState.Disabled;
		else if (Focused || ClientRectangle.Contains(PointToClient(Cursor.Position)))
			return ComboBoxState.Hot;
		else
			return ComboBoxState.Normal;
	}

	/// <summary>
	/// Determines the state in which to render the dropdown button portion of the control (when using visual styles).
	/// </summary>
	/// <returns></returns>
	private ComboBoxState GetComboBoxState() {
		if (!Enabled)
			return ComboBoxState.Disabled;
		else if (_droppedDown || ClientRectangle.Contains(PointToClient(Cursor.Position)))
			return ((_droppedDown && _drawWithVisualStyles) || ((MouseButtons & MouseButtons.Left) == MouseButtons.Left)) ? ComboBoxState.Pressed : ComboBoxState.Hot;
		else
			return ComboBoxState.Normal;
	}

	/// <summary>
	/// Determines the state in which to render the dropdown button portion of the control (when not using visual styles).
	/// </summary>
	/// <returns></returns>
	private ButtonState GetPlainButtonState() {
		if (!Enabled)
			return ButtonState.Inactive;
		else if (_dropDownButtonBounds.Contains(PointToClient(Cursor.Position)) && ((MouseButtons & MouseButtons.Left) == MouseButtons.Left))
			return ButtonState.Pushed;
		else
			return ButtonState.Normal;
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
	/// Raised the DropDownButtonClick event.
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnDropDownButtonClick(EventArgs e) {
		if (DropDownButtonClick != null) DropDownButtonClick(this, e);
	}

	/// <summary>
	/// Raises the DropDown event.
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnDropDown(EventArgs e) {
		if (DropDown != null) DropDown(this, e);
	}

	/// <summary>
	/// Raises the DropDownClosed event.
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnDropDownClosed(EventArgs e) {
		if (DropDownClosed != null) DropDownClosed(this, e);
	}

	/// <summary>
	/// Recalculates the fixed height of the control when the font changes.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnFontChanged(EventArgs e) {
		base.OnFontChanged(e);
		SetHeight();
	}

	/// <summary>
	/// Repaints the focus rectangle when focus changes.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnGotFocus(EventArgs e) {
		base.OnGotFocus(e);
		if (ShowFocusCues) Invalidate();
	}

	/// <summary>
	/// Repaints the focus rectangle when focus changes.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnLostFocus(EventArgs e) {
		base.OnLostFocus(e);
		if (ShowFocusCues) Invalidate();
	}

    /// <summary>
    /// Prevents the control's background from painting normally.
    /// </summary>
    /// <param name="pevent"></param>
    protected override void OnPaintBackground(PaintEventArgs pevent) {
        if (!_drawWithVisualStyles || !_bufferedPainter.BufferedPaintSupported || !_bufferedPainter.Enabled) {
            base.OnPaintBackground(pevent);
        }
    }

	/// <summary>
	/// Paints the content in the editable portion of the control, providing additional measurements and operations.
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnPaintContent(DropDownPaintEventArgs e) {
		if (PaintContent != null) PaintContent(this, e);
	}

	/// <summary>
	/// Repaints the control when the mouse enters its bounds.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseEnter(EventArgs e) {
		base.OnMouseEnter(e);
        _bufferedPainter.State = GetComboBoxState();
	}

	/// <summary>
	/// Repaints the control when the mouse leaves its bounds.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseLeave(EventArgs e) {
		base.OnMouseLeave(e);
        _bufferedPainter.State = GetComboBoxState();
	}

	/// <summary>
	/// Repaints the control when a mouse button is pressed.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseDown(MouseEventArgs e) {
		base.OnMouseDown(e);
		Focus();
        _bufferedPainter.State = GetComboBoxState();
	}

	/// <summary>
	/// Repaints the control when a mouse button is released.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseUp(MouseEventArgs e) {
		base.OnMouseUp(e);
        _bufferedPainter.State = GetComboBoxState();
	}

	/// <summary>
	/// Repaints the control when the mouse is moved over the control.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseMove(MouseEventArgs e) {
		base.OnMouseMove(e);
        _bufferedPainter.State = GetComboBoxState();
	}

	/// <summary>
	/// Determines when to raise the DropDownButtonClick event.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnMouseClick(MouseEventArgs e) {
		base.OnMouseClick(e);
		if (_dropDownButtonBounds.Contains(e.Location)) OnDropDownButtonClick(e);
	}

	/// <summary>
	/// Recalculates the bounds for the dropdown button when the control's size changes.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnSizeChanged(EventArgs e) {
		base.OnSizeChanged(e);
		_dropDownButtonBounds = GetComboButtonBounds(ClientRectangle);
	}

	/// <summary>
	/// Repaints the control when the control is enabled/disabled.
	/// </summary>
	/// <param name="e"></param>
	protected override void OnEnabledChanged(EventArgs e) {
		base.OnEnabledChanged(e);
		_bufferedPainter.State = GetComboBoxState();
	}

	/// <summary>
	/// Sets the fixed height of the control, based on the font size.
	/// </summary>
	private void SetHeight() {
		Height = CONTROL_HEIGHT + Font.Height;	
	}

	/// <summary>
	/// Calculates and returns the bounds for the dropdown button for a dropdown control.
	/// </summary>
	/// <param name="bounds"></param>
	/// <returns></returns>
	internal static Rectangle GetComboButtonBounds(Rectangle bounds) {
		return new Rectangle(bounds.Right - DROPDOWNBUTTON_WIDTH, bounds.Top, DROPDOWNBUTTON_WIDTH, bounds.Height);
	}

	/// <summary>
	/// Draws a legacy style combo box control.
	/// </summary>
	/// <param name="graphics"></param>
	/// <param name="bounds"></param>
	/// <param name="buttonBounds"></param>
	/// <param name="backColor"></param>
	/// <param name="state"></param>
	internal static void DrawLegacyComboBox(Graphics graphics, Rectangle bounds, Rectangle buttonBounds, Color backColor, ButtonState state) {
		Rectangle borderRect = bounds;
		borderRect.Height++;
		graphics.FillRectangle(new SolidBrush(backColor), bounds);
		ControlPaint.DrawBorder3D(graphics, borderRect, Border3DStyle.Sunken);
		Rectangle buttonRect = buttonBounds;
		buttonRect.X -= 2;
		buttonRect.Y += 2;
		buttonRect.Height -= 3;
		ControlPaint.DrawComboButton(graphics, buttonRect, state);
	}

    /// <summary>
    /// Paints the control using the Buffered Paint API.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void bufferedPainter_PaintVisualState(object sender, BufferedPaintEventArgs<ComboBoxState> e) {
        if (_drawWithVisualStyles && _bufferedPainter.BufferedPaintSupported && _bufferedPainter.Enabled) {
            // draw in the vista/win7 style
            VisualStyleRenderer r = new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal);
            r.DrawParentBackground(e.Graphics, ClientRectangle, this);

            Rectangle buttonBounds = ClientRectangle;
            buttonBounds.Inflate(1, 1);
            ButtonRenderer.DrawButton(e.Graphics, buttonBounds, GetPushButtonState(e.State));

            Rectangle clipBounds = _dropDownButtonBounds;
            clipBounds.Inflate(-2, -2);
            e.Graphics.SetClip(clipBounds);
            ComboBoxRenderer.DrawDropDownButton(e.Graphics, _dropDownButtonBounds, e.State);
            e.Graphics.SetClip(ClientRectangle);
        }
        else if (_drawWithVisualStyles && ComboBoxRenderer.IsSupported) {
            // draw using the visual style renderer
            ComboBoxRenderer.DrawTextBox(e.Graphics, ClientRectangle, GetTextBoxState());
            ComboBoxRenderer.DrawDropDownButton(e.Graphics, _dropDownButtonBounds, e.State);
        }
        else {
            // draw using the legacy technique
			DrawLegacyComboBox(e.Graphics, ClientRectangle, _dropDownButtonBounds, BackColor, GetPlainButtonState());
        }

        OnPaintContent(new DropDownPaintEventArgs(e.Graphics, ClientRectangle, GetTextBoxBounds()));
    }
}

/// <summary>
/// EventArgs class for the 
/// </summary>
public class DropDownPaintEventArgs : PaintEventArgs {

	/// <summary>
	/// Gets the display rectangle for the editable portion of the control.
	/// </summary>
	public Rectangle Bounds {
		get;
		private set;
	}

	/// <summary>
	/// Creates a new instance of the DropDownPaintEventArgs class.
	/// </summary>
	/// <param name="graphics"></param>
	/// <param name="clipRect"></param>
	/// <param name="bounds"></param>
	public DropDownPaintEventArgs(Graphics graphics, Rectangle clipRect, Rectangle bounds) : base(graphics, clipRect) {
		Bounds = bounds;
	}

	/// <summary>
	/// Draws a focus rectangle on the editable portion of the control.
	/// </summary>
	public void DrawFocusRectangle() {
		Rectangle focus = Bounds;
		focus.Inflate(-3, -3);
		//focus.Width++;
		ControlPaint.DrawFocusRectangle(Graphics, focus);
	}
}