using System;

namespace ESRI.ArcGIS.Framework.Internal
{
    /// <summary>
    ///     An internal class used to control the animation actions of the progress bar on the status bar.
    /// </summary>
    internal class ProgressBarAnimation : ProgressGlobeAnimation, IProgressBarAnimation
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressBarAnimation" /> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public ProgressBarAnimation(IApplication application)
            : base(application)
        {
        }

        #endregion

        #region IProgressBarAnimation Members

        /// <summary>
        ///     Gets or sets the percent complete.
        /// </summary>
        /// <value>The percent complete.</value>
        public float Percentage
        {
            get { return base.Application.StatusBar.ProgressBar.Position/(float) base.Application.StatusBar.ProgressBar.MaxRange*100; }
        }

        /// <summary>
        ///     Increments the progress bar and updates the status bar with specified status message.
        /// </summary>
        /// <param name="statusMessage">The status message.</param>
        public void Step(string statusMessage)
        {
            base.Application.StatusBar.ProgressBar.Step();
            base.Application.StatusBar.ProgressBar.Show();

            base.Play(MouseCursorImage.Wait, statusMessage);
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
        public void Initialize(int min, int max, int position, int step = 1)
        {
            if (min > max) throw new ArgumentOutOfRangeException("min", "The minimum cannot be greater than the maximum.");
            if (max < min) throw new ArgumentOutOfRangeException("max", "The maximum cannot be less than the minimum.");
            if (position < 0) throw new ArgumentOutOfRangeException("position", "The value position be less than 0.");
            if (step < 1) throw new ArgumentOutOfRangeException("step", "The step cannot be less than 1.");
            if (step > max) throw new ArgumentOutOfRangeException("step", "The step cannot be greater than the maximum.");

            base.Application.StatusBar.ProgressBar.Position = position;
            base.Application.StatusBar.ProgressBar.MaxRange = max;
            base.Application.StatusBar.ProgressBar.MinRange = min;
            base.Application.StatusBar.ProgressBar.StepValue = step;
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
                base.Application.StatusBar.ProgressBar.Hide();
            }
        }

        #endregion
    }

    /// <summary>
    ///     Provides access to the progress bar animation on the status bar.
    /// </summary>
    public interface IProgressBarAnimation : IDisposable
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the percent complete.
        /// </summary>
        /// <value>The percent complete.</value>
        float Percentage { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Increments the progress bar and updates the status bar with specified status message.
        /// </summary>
        /// <param name="statusMessage">The status message.</param>
        void Step(string statusMessage);

        #endregion
    }
}