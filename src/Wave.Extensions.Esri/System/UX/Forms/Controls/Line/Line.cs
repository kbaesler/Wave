using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace System.Forms
{

    #region Enumerations

    /// <summary>
    ///     The Horizontal Alignment Enumeration
    /// </summary>
    public enum HorizontalAlignment
    {
        /// <summary>
        ///     Alignment Left
        /// </summary>
        Left,

        /// <summary>
        ///     Alignment Center
        /// </summary>
        Center,

        /// <summary>
        ///     Alignment Right
        /// </summary>
        Right
    }

    /// <summary>
    ///     The Verical Alignment Enumeration
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        ///     Alignment Top
        /// </summary>
        Top,

        /// <summary>
        ///     Alignment Middle
        /// </summary>
        Middle,

        /// <summary>
        ///     Alignment Bottom
        /// </summary>
        Bottom
    }

    #endregion

    /// <summary>
    ///     A control that renders a graphical line as a control.
    /// </summary>
    public partial class Line : Control
    {
        #region Fields

        private VerticalAlignment _LineAlignment = VerticalAlignment.Middle;
        private int _MarginSpace = 2;
        private int _Padding = 2;
        private HorizontalAlignment _TextAlignment = HorizontalAlignment.Left;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line" /> class.
        /// </summary>
        public Line()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            BackColor = Color.Transparent;
            TabStop = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The vertical alignement of the line within the space of the control
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(VerticalAlignment.Middle)]
        [Description("The vertical alignment of the line within the space of the control")]
        public VerticalAlignment LineAlignment
        {
            get { return _LineAlignment; }
            set
            {
                _LineAlignment = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Tell where the text caption is aligned in the control
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(HorizontalAlignment.Left)]
        [Description("Tell where the text caption is aligned in the control")]
        public HorizontalAlignment TextAlignment
        {
            get { return _TextAlignment; }
            set
            {
                _TextAlignment = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     The distance in pixels form the control margin to caption text
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(16)]
        [Description("The distance in pixels form the control margin to caption text")]
        public int TextMarginSpace
        {
            get { return _MarginSpace; }
            set
            {
                _MarginSpace = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     The space in pixels arrownd text caption
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(2)]
        [Description("The space in pixels arrownd text caption")]
        public int TextPadding
        {
            get { return _Padding; }
            set
            {
                _Padding = value;
                Invalidate();
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Raises the <see cref="System.Windows.Forms.Control.FontChanged"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnFontChanged(EventArgs e)
        {
            this.OnResize(e);
            base.OnFontChanged(e);
        }

        /// <summary>
        ///     Raises the <see cref="System.Windows.Forms.Control.Paint"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int ym;
            switch (LineAlignment)
            {
                case VerticalAlignment.Top:
                    ym = 0;
                    break;

                case VerticalAlignment.Middle:
                    ym = Convert.ToInt32(Math.Ceiling((double) (Size.Height/2))) - 1;
                    break;

                case VerticalAlignment.Bottom:
                    ym = Size.Height - 2;
                    break;

                default:
                    ym = 0;
                    break;
            }

            SizeF captionSizeF = e.Graphics.MeasureString(this.Text, this.Font, this.Width - _MarginSpace*2, StringFormat.GenericDefault);
            int captionLength = Convert.ToInt32(captionSizeF.Width);

            int beforeCaption;
            int afterCaption;

            if (string.IsNullOrEmpty(this.Text))
            {
                beforeCaption = _MarginSpace;
                afterCaption = _MarginSpace;
            }
            else
            {
                switch (TextAlignment)
                {
                    case HorizontalAlignment.Left:
                        beforeCaption = _MarginSpace;
                        afterCaption = _MarginSpace + _Padding*2 + captionLength;
                        break;

                    case HorizontalAlignment.Center:
                        beforeCaption = (Width - captionLength)/2 - _Padding;
                        afterCaption = (Width - captionLength)/2 + captionLength + _Padding;
                        break;

                    case HorizontalAlignment.Right:
                        beforeCaption = Width - _MarginSpace*2 - captionLength;
                        afterCaption = Width - _MarginSpace;
                        break;

                    default:
                        beforeCaption = _MarginSpace;
                        afterCaption = _MarginSpace;
                        break;
                }
            }

            // -------
            // |      ...caption...
            e.Graphics.DrawLines(new Pen(Color.DimGray, 1),
                new[]
                {
                    new Point(0, ym + 1),
                    new Point(0, ym),
                    new Point(beforeCaption, ym)
                }
                );

            //                  -------
            //	      ...caption...
            e.Graphics.DrawLines(new Pen(Color.DimGray, 1),
                new[]
                {
                    new Point(afterCaption, ym),
                    new Point(this.Width, ym)
                }
                );

            //        ...caption...
            // -------
            e.Graphics.DrawLines(new Pen(Color.White, 1),
                new[]
                {
                    new Point(0, ym + 1),
                    new Point(beforeCaption, ym + 1)
                }
                );

            //        ...caption...       |
            //                  -------
            e.Graphics.DrawLines(new Pen(Color.White, 1),
                new[]
                {
                    new Point(afterCaption, ym + 1),
                    new Point(this.Width, ym + 1),
                    new Point(this.Width, ym)
                }
                );

            //        ...caption...
            if (!string.IsNullOrEmpty(this.Text))
            {
                e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), beforeCaption + _Padding, 1);
            }
        }

        /// <summary>
        ///     Raises the <see cref="Control.Resize" /> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Height = this.Font.Height + 2;
            this.Invalidate();
        }

        #endregion
    }
}