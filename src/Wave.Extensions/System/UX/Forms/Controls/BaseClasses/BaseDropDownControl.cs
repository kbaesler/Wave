using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Forms.VisualStyles;

namespace System.Forms.Controls
{
    /// <summary>
    ///     Abstract base class for a control which behaves like a dropdown but does not contain
    ///     logic for displaying a popup window.
    /// </summary>
    /// <remarks>http://www.brad-smith.info/blog/archives/193</remarks>
    [Designer(typeof (DropDownControlDesigner))]
    public abstract class BaseDropDownControl : Control
    {
        #region Constants

        private const int CONTROL_HEIGHT = 7;
        private const int DROPDOWNBUTTON_WIDTH = 17;

        #endregion

        #region Fields

        private bool _DrawWithVisualStyles;
        private Rectangle _DropDownButtonBounds;
        private bool _DroppedDown;

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates a new instance of DropDownControlBaSE.
        /// </summary>
        protected BaseDropDownControl()
        {
            // control styles
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.StandardClick, true);
            SetStyle(ControlStyles.UserPaint, true);

            // default values
            _DrawWithVisualStyles = true;
            BackColor = SystemColors.Window;
        }

        #endregion

        #region Events

        /// <summary>
        ///     Fired when the drop-down portion of the control is displayed.
        /// </summary>
        [Description("Occurs when the drop-down portion of the control is displayed.")]
        public event EventHandler DropDown;

        /// <summary>
        ///     Fired when the user clicks the dropdown button at the right edge of the control.
        /// </summary>
        [Description("Occurs when the user clicks the dropdown button at the right edge of the control.")]
        protected event EventHandler DropDownButtonClick;

        /// <summary>
        ///     Fired when the drop-down portion of the control is closed.
        /// </summary>
        [Description("Occurs when the drop-down portion of the control is closed.")]
        public event EventHandler DropDownClosed;

        /// <summary>
        ///     Fired when the content of the editable portion of the control is painted.
        /// </summary>
        [Description("Occurs when the content of the editable portion of the control is painted.")]
        public event EventHandler<DropDownPaintEventArgs> PaintContent;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the background color to use for this control.
        /// </summary>
        [DefaultValue(typeof (Color), "Window")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        /// <summary>
        ///     Hides the BackgroundImage property on the designer.
        /// </summary>
        [Browsable(false)]
        public override Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }

        /// <summary>
        ///     Hides the BackgroundImageLayout property on the designer.
        /// </summary>
        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout
        {
            get { return base.BackgroundImageLayout; }
            set { base.BackgroundImageLayout = value; }
        }

        /// <summary>
        ///     Determines whether to draw the control with visual styles.
        /// </summary>
        [DefaultValue(true),
         Description("Determines whether to draw the control with visual styles."),
         Category("Appearance")]
        public bool DrawWithVisualStyles
        {
            get { return _DrawWithVisualStyles; }
            set
            {
                _DrawWithVisualStyles = value;
                Invalidate();
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Opens or closes the dropdown portion of the control.
        /// </summary>
        [Browsable(false)]
        protected virtual bool DroppedDown
        {
            get { return _DroppedDown; }
            set
            {
                _DroppedDown = value;
                Invalidate();
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Determines whether the specified key is a regular input key or a special key that requires preprocessing.
        /// </summary>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys" /> values.</param>
        /// <returns>
        ///     true if the specified key is a regular input key; otherwise, falSE.
        /// </returns>
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
        ///     Raises the <see cref="DropDown" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected virtual void OnDropDown(EventArgs e)
        {
            EventHandler eventHandler = this.DropDown;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="DropDownButtonClick" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected virtual void OnDropDownButtonClick(EventArgs e)
        {
            EventHandler eventHandler = this.DropDownButtonClick;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="DropDownClosed" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected virtual void OnDropDownClosed(EventArgs e)
        {
            EventHandler eventHandler = this.DropDownClosed;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.Control.FontChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            SetHeight();
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.Control.GotFocus" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (ShowFocusCues) Invalidate();
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.Control.LostFocus" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (ShowFocusCues) Invalidate();
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.Control.MouseClick" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (_DropDownButtonBounds.Contains(e.Location)) OnDropDownButtonClick(e);
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.Control.MouseDown" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            Invalidate(_DropDownButtonBounds);
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.Control.MouseEnter" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            Invalidate();
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Invalidate();
        }

        /// <summary>
        ///     Repaints the control when the mouse is moved over the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Invalidate(_DropDownButtonBounds);
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.Control.MouseUp" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Invalidate(_DropDownButtonBounds);
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.Control.Paint" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_DrawWithVisualStyles && ComboBoxRenderer.IsSupported)
            {
                // draw using the visual style renderer
                ComboBoxRenderer.DrawTextBox(e.Graphics, ClientRectangle, GetTextBoxState());
                ComboBoxRenderer.DrawDropDownButton(e.Graphics, _DropDownButtonBounds, GetDropDownButtonState());
            }
            else
            {
                // draw using the legacy technique
                Rectangle borderRect = ClientRectangle;
                borderRect.Height++;
                e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
                ControlPaint.DrawBorder3D(e.Graphics, borderRect);
                ControlPaint.DrawComboButton(e.Graphics, _DropDownButtonBounds, GetPlainButtonState());
            }

            OnPaintContent(new DropDownPaintEventArgs(e.Graphics, e.ClipRectangle, GetTextBoxBounds()));
        }

        /// <summary>
        ///     Raises the <see cref="PaintContent" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Forms.Controls.DropDownPaintEventArgs" /> instance containing the event data.</param>
        protected virtual void OnPaintContent(DropDownPaintEventArgs e)
        {
            EventHandler<DropDownPaintEventArgs> eventHandler = this.PaintContent;
            if (eventHandler != null)
                eventHandler(this, e);
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            _DropDownButtonBounds = new Rectangle(ClientSize.Width - DROPDOWNBUTTON_WIDTH, 0, DROPDOWNBUTTON_WIDTH, ClientSize.Height);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Determines the state in which to render the dropdown button portion of the control (when using visual styles).
        /// </summary>
        /// <returns></returns>
        private ComboBoxState GetDropDownButtonState()
        {
            if (!Enabled)
                return ComboBoxState.Disabled;
            if (_DroppedDown || _DropDownButtonBounds.Contains(PointToClient(Cursor.Position)))
                return (_DroppedDown || ((MouseButtons & MouseButtons.Left) == MouseButtons.Left)) ? ComboBoxState.Pressed : ComboBoxState.Hot;
            return ComboBoxState.Normal;
        }

        /// <summary>
        ///     Determines the state in which to render the dropdown button portion of the control (when not using visual styles).
        /// </summary>
        /// <returns></returns>
        private ButtonState GetPlainButtonState()
        {
            if (!Enabled)
                return ButtonState.Inactive;
            if (_DroppedDown || (_DropDownButtonBounds.Contains(PointToClient(Cursor.Position)) && ((MouseButtons & MouseButtons.Left) == MouseButtons.Left)))
                return ButtonState.Pushed;
            return ButtonState.Normal;
        }

        /// <summary>
        ///     Gets the bounds of the textbox portion of the control by subtracting the dropdown button bounds from the client
        ///     rectangle.
        /// </summary>
        /// <returns></returns>
        private Rectangle GetTextBoxBounds()
        {
            return new Rectangle(0, 0, _DropDownButtonBounds.Left, ClientRectangle.Height);
        }

        /// <summary>
        ///     Determines the state in which to render the textbox portion of the control (when using visual styles).
        /// </summary>
        /// <returns></returns>
        private ComboBoxState GetTextBoxState()
        {
            if (!Enabled)
                return ComboBoxState.Disabled;
            if (Focused || ClientRectangle.Contains(PointToClient(Cursor.Position)))
                return ComboBoxState.Hot;
            return ComboBoxState.Normal;
        }

        /// <summary>
        ///     Sets the fixed height of the control, based on the font size.
        /// </summary>
        private void SetHeight()
        {
            Height = CONTROL_HEIGHT + Font.Height;
        }

        #endregion
    }

    /// <summary>
    ///     Designer for DropDownControlBase
    /// </summary>
    public class DropDownControlDesigner : ControlDesigner
    {
        #region Public Properties

        /// <summary>
        ///     Ensures that this control can only be sized horizontally.
        /// </summary>
        public override SelectionRules SelectionRules
        {
            get { return base.SelectionRules & ~SelectionRules.BottomSizeable & ~SelectionRules.TopSizeable; }
        }

        /// <summary>
        ///     Gets a list containing the four main alignment points plus the baseline for the text.
        /// </summary>
        public override IList SnapLines
        {
            get
            {
                IList snapLines = base.SnapLines;
                if (snapLines != null)
                {
                    snapLines.Add(new SnapLine(SnapLineType.Baseline, Control.Height/2 - (int) Control.Font.Size/2 + (int) Control.Font.Size));
                }
                return snapLines;
            }
        }

        #endregion
    }

    /// <summary>
    ///     EventArgs class for the
    /// </summary>
    public class DropDownPaintEventArgs : PaintEventArgs
    {
        #region Constructors

        /// <summary>
        ///     Creates a new instance of the DropDownPaintEventArgs class.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="clipRect"></param>
        /// <param name="bounds"></param>
        public DropDownPaintEventArgs(Graphics graphics, Rectangle clipRect, Rectangle bounds)
            : base(graphics, clipRect)
        {
            Bounds = bounds;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the display rectangle for the editable portion of the control.
        /// </summary>
        public Rectangle Bounds { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Draws a focus rectangle on the editable portion of the control.
        /// </summary>
        public void DrawFocusRectangle()
        {
            Rectangle focus = Bounds;
            focus.Inflate(-2, -2);
            focus.Width++;
            ControlPaint.DrawFocusRectangle(Graphics, focus);
        }

        #endregion
    }
}