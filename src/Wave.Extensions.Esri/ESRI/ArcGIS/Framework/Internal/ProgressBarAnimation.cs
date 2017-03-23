using System;

using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Framework.Internal
{
    /// <summary>
    ///     An internal class used to control the animation actions of the progress bar on the status bar.
    /// </summary>
    internal class ProgressBarAnimation : ProgressGlobeAnimation, IProgressBarAnimation
    {
        #region Fields

        private IStepProgressor _ProgressBar;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressBarAnimation" /> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public ProgressBarAnimation(IApplication application)
            : this(application, application.StatusBar.ProgressBar)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressBarAnimation" /> class.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="progressBar">The progress bar.</param>
        public ProgressBarAnimation(IApplication application, IStepProgressor progressBar)
            : base(application)
        {
            _ProgressBar = progressBar;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets or sets the progress bar.
        /// </summary>
        /// <value>
        ///     The progress bar.
        /// </value>
        protected IStepProgressor ProgressBar
        {
            get { return _ProgressBar; }
            set { _ProgressBar = value; }
        }

        #endregion

        #region IProgressBarAnimation Members

        /// <summary>
        ///     Gets or sets the percent complete.
        /// </summary>
        /// <value>The percent complete.</value>
        public float Percentage
        {
            get { return _ProgressBar.Position/(float) _ProgressBar.MaxRange*100; }
        }

        /// <summary>
        ///     Increments the progress bar and updates the status bar with specified status message.
        /// </summary>
        /// <param name="statusMessage">The status message.</param>
        public virtual bool Step(string statusMessage)
        {
            _ProgressBar.Step();
            _ProgressBar.Show();
            _ProgressBar.Message = statusMessage;

            return true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Initializes the progress bar with the specified values.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="position">The value.</param>
        /// <param name="step">The step.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     min;The minimum cannot be greater than the maximum.
        ///     or
        ///     max;The maximum cannot be less than the minimum.
        ///     or
        ///     position;The value position be less than 0.
        ///     or
        ///     step;The step cannot be less than 1.
        ///     or
        ///     step;The step cannot be greater than the maximum.
        /// </exception>
        public virtual void Initialize(int min, int max, int position, int step = 1)
        {
            if (min > max) throw new ArgumentOutOfRangeException("min", "The minimum cannot be greater than the maximum.");
            if (position < 0) throw new ArgumentOutOfRangeException("position", "The value position be less than 0.");
            if (step < 1) throw new ArgumentOutOfRangeException("step", "The step cannot be less than 1.");
            if (step > max) throw new ArgumentOutOfRangeException("step", "The step cannot be greater than the maximum.");

            _ProgressBar.Position = position;
            _ProgressBar.MaxRange = max;
            _ProgressBar.MinRange = min;
            _ProgressBar.StepValue = step;

            base.Play(MouseCursorImage.Wait, "");
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _ProgressBar.Hide();
            }
        }

        #endregion
    }
}