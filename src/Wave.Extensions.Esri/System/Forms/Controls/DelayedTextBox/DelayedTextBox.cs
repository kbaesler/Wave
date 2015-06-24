using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

using Timer = System.Threading.Timer;

namespace System.Forms.Controls
{
    /// <summary>
    ///     A TextBox whose Text will get updated after a specified interval when the user stops entering text
    /// </summary>
    public partial class DelayedTextBox : TextBox
    {
        #region Fields

        private Timer _Timer;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DelayedTextBox" /> class.
        /// </summary>
        public DelayedTextBox()
        {
            InitializeComponent();

            this.DelayTime = 700;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the delay time.
        /// </summary>
        /// <value>
        ///     The delay time.
        /// </value>
        [DefaultValue(700)]
        [Description("The amount of time (in miliseconds) to wait after the text has changed before TextChanged event is triggered.")]
        [Category("Behaviors")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public int DelayTime { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Raises the Text is changed.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnTextChanged(EventArgs e)
        {
            // Get rid of the timer if it exists
            if (_Timer != null)
            {
                // Dispose of the timer so that it wont get called again
                _Timer.Dispose();
            }

            // Recreate the timer everytime the text changes
            _Timer = new Timer(o =>
            {
                // Invoke the delegate to update the binding source on the main (ui) thread
                this.Invoke((MethodInvoker) (() => base.OnTextChanged(e)), new object[] {}
                    );

                // Dispose of the timer so that it wont get called again
                _Timer.Dispose();
            }, null, this.DelayTime, Timeout.Infinite);
        }

        #endregion
    }
}