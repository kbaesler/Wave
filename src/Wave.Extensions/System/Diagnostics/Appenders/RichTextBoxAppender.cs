using System.Drawing;
using System.Windows.Forms;

using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util;

namespace System.Diagnostics.Appenders
{
    /// <summary>
    ///     Appends logging events to a <see cref="System.Windows.Forms.RichTextBox" />
    /// </summary>
    /// <para>
    ///     RichTextBoxAppender appends log events to a specified RichTextBox control.
    ///     It also allows the color, font and style of a specific type of message to be set.
    /// </para>
    /// <para>
    ///     When configuring the rich text box appender, mapping should be
    ///     specified to map a logging level to a text style. For example:
    /// </para>
    /// <code lang="XML" escaped="true">
    ///  <mapping>
    ///         <level value="DEBUG" />
    ///         <textColorName value="DarkGreen" />
    ///     </mapping>
    ///  <mapping>
    ///         <level value="INFO" />
    ///         <textColorName value="ControlText" />
    ///     </mapping>
    ///  <mapping>
    ///         <level value="WARN" />
    ///         <textColorName value="Blue" />
    ///     </mapping>
    ///  <mapping>
    ///         <level value="ERROR" />
    ///         <textColorName value="Red" />
    ///         <bold value="true" />
    ///         <pointSize value="10" />
    ///     </mapping>
    ///  <mapping>
    ///         <level value="FATAL" />
    ///         <textColorName value="Black" />
    ///         <backColorName value="Red" />
    ///         <bold value="true" />
    ///         <pointSize value="12" />
    ///         <fontFamilyName value="Lucida Console" />
    ///     </mapping>
    /// </code>
    /// <para>
    ///     The Level is the standard log4net logging level. TextColorName and BackColorName should match
    ///     a value of the System.Drawing.KnownColor enumeration. Bold and/or Italic may be specified, using
    ///     <code>true</code> or <code>false</code>. FontFamilyName should match a font available on the client,
    ///     but if it's not found, the control's font will be used.
    /// </para>
    public sealed class RichTextBoxAppender : AppenderSkeleton
    {
        #region Delegates

        /// <summary>
        ///     A delegate for handling passing the logging event data to the text box.
        /// </summary>
        /// <param name="loggingEvent">The logging event data/</param>
        private delegate void UpdateControlDelegate(LoggingEvent loggingEvent);

        #endregion

        #region Fields

        private readonly LevelMapping _LevelMapping = new LevelMapping();

        private RichTextBox _RichTextBox;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RichTextBoxAppender" /> class.
        /// </summary>
        public RichTextBoxAppender()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RichTextBoxAppender" /> class.
        /// </summary>
        /// <param name="control">The rich text box.</param>
        public RichTextBoxAppender(RichTextBox control)
            : this(control, Level.All, new PatternLayout("%date{dd-MM-yyyy HH:mm:ss.fff} %5level %message %n"))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RichTextBoxAppender" /> class.
        /// </summary>
        /// <param name="control">The rich text box.</param>
        /// <param name="level">The level.</param>
        /// <param name="layout">The layout.</param>
        public RichTextBoxAppender(RichTextBox control, Level level, PatternLayout layout)
        {
            this.Control = control;
            this.Threshold = level;
            this.Layout = layout;
            this.MaxTextLength = 100000;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the control.
        /// </summary>
        /// <value>
        ///     The control.
        /// </value>
        public RichTextBox Control
        {
            get { return _RichTextBox; }
            set
            {
                _RichTextBox = value;
                _RichTextBox.ReadOnly = true;
                _RichTextBox.HideSelection = true;
            }
        }

        /// <summary>
        ///     Gets or sets the length of the max text.
        /// </summary>
        /// <value>
        ///     The length of the max text.
        /// </value>
        public int MaxTextLength { get; set; }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Indicates that the layout pattern must be specified.
        /// </summary>
        protected override bool RequiresLayout
        {
            get { return true; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Activates the options.
        /// </summary>
        public override void ActivateOptions()
        {
            base.ActivateOptions();

            _LevelMapping.ActivateOptions();
        }

        /// <summary>
        ///     Adds the text style level mapping used to render the text.
        /// </summary>
        /// <param name="textStyleLevel">The text style level mapping.</param>
        public void AddLevelStyle(TextStyleLevel textStyleLevel)
        {
            _LevelMapping.Add(textStyleLevel);
        }

        /// <summary>
        ///     Assigns the <paramref name="control" /> to the appender with the specified <paramref name="appenderName" />
        /// </summary>
        /// <param name="control">The <see cref="System.Windows.Forms.RichTextBox" /> control that will display logging events</param>
        /// <param name="appenderName">Name of RichTextBoxAppender (case-sensitive)</param>
        /// <returns>
        ///     Returns a <see cref="bool" /> representing <c>true</c> if the appender named <code>appenderName</code> was found;
        ///     otherwise <c>false</c>.
        /// </returns>
        /// <example>
        ///     <code lang="C#">
        ///  private void MainForm_Load(object sender, EventArgs e)
        ///  {
        ///  System.Diagnostics.Appenders.RichTextBoxAppender.Assign(logRichTextBox, "MainFormRichTextBoxAppender");
        ///  }
        ///    </code>
        /// </example>
        public static bool Assign(RichTextBox control, string appenderName)
        {
            if (control == null)
                return false;

            foreach (RichTextBoxAppender appender in Log.GetAppenders<RichTextBoxAppender>(o => o.Name.Equals(appenderName)))
            {
                appender.Control = control;
                return true;
            }

            return false;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Appends the specified logging event.
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (this.Control == null)
                return;

            if (this.Control.InvokeRequired)
            {
                this.Control.Invoke(
                    new UpdateControlDelegate(UpdateControl),
                    new object[] {loggingEvent});
            }
            else
            {
                UpdateControl(loggingEvent);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Add logging event to configured control
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        private void UpdateControl(LoggingEvent loggingEvent)
        {
            // There's a performance impacts if the text get's to long.
            if (this.Control.TextLength > this.MaxTextLength)
            {
                this.Control.Clear();
                this.Control.AppendText(string.Format("(Cleared Log Length Max: {0})\n", this.MaxTextLength));
            }

            // Look for a style mapping
            TextStyleLevel style = _LevelMapping.Lookup(loggingEvent.Level) as TextStyleLevel;
            if (style != null)
            {
                // Set the colors of the text about to be appended
                this.Control.SelectionBackColor = style.BackColor;
                this.Control.SelectionColor = style.TextColor;

                // Alter selection font as much as necessary
                // Missing settings are replaced by the font settings on the control
                if (style.Font != null)
                {
                    // Set Font Family, size and styles
                    this.Control.SelectionFont = style.Font;
                }
                else if (style.SizeInPoints > 0 && this.Control.Font.SizeInPoints != style.SizeInPoints)
                {
                    // Use control's font family, set size and styles
                    float size = style.SizeInPoints > 0.0f ? style.SizeInPoints : this.Control.Font.SizeInPoints;
                    this.Control.SelectionFont = new Font(this.Control.Font.FontFamily.Name, size, style.FontStyle);
                }
                else if (this.Control.Font.Style != style.FontStyle)
                {
                    // Use control's font family and size, set styles
                    this.Control.SelectionFont = new Font(this.Control.Font, style.FontStyle);
                }
            }

            this.Control.AppendText(this.RenderLoggingEvent(loggingEvent));
        }

        #endregion
    }
}