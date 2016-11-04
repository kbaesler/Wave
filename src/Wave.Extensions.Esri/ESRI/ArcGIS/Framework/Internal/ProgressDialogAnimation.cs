using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Framework.Internal
{
    /// <summary>
    ///     An internal class used to control the animation actions of the progress bar on the dialog window.
    /// </summary>
    internal class ProgressDialogAnimation : ProgressBarAnimation, IProgressDialogAnimation
    {
        #region Fields

        private readonly IProgressDialog2 _Dialog;
        private readonly ITrackCancel _TrackCancel;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressBarAnimation" /> class.
        /// </summary>
        /// <param name="animationType">Type of the animation.</param>
        /// <param name="application">The application.</param>
        /// <param name="trackCancel">The track cancel.</param>
        public ProgressDialogAnimation(IApplication application, esriProgressAnimationTypes animationType, ITrackCancel trackCancel)
            : base(application)
        {
            _Dialog = (IProgressDialog2) (new ProgressDialogFactoryClass()).Create(trackCancel, application.hWnd);
            _TrackCancel = trackCancel;

            this.AnimationType = animationType;
            this.ProgressBar = (IStepProgressor) _Dialog;
        }

        #endregion

        #region IProgressDialogAnimation Members

        /// <summary>
        ///     Gets or sets the type of the animation.
        /// </summary>
        /// <value>
        ///     The type of the animation.
        /// </value>
        public esriProgressAnimationTypes AnimationType
        {
            get { return _Dialog.Animation; }
            set { _Dialog.Animation = value; }
        }

        /// <summary>
        ///     Increments the progress bar and updates the status bar with specified status message.
        /// </summary>
        /// <param name="statusMessage">The status message.</param>
        public override bool Step(string statusMessage)
        {
            if (_TrackCancel != null && !_TrackCancel.Continue())
                return base.Step(statusMessage);

            return false;
        }
       
        /// <summary>
        ///     Starts the global spinning in ArcMap and updates the message on the status bar.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="statusMessage">The status message.</param>
        public void Play(MouseCursorImage cursor, string title, string description, string statusMessage)
        {
            _Dialog.CancelEnabled = true;
            _Dialog.Description = description;
            _Dialog.Title = title;

            this.Play(cursor, statusMessage);
        }

        #endregion
    }
}