using System;

using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Framework.Internal
{
    /// <summary>
    ///     An internal class used to handle starting and stopping the ArcMap progress animation.
    /// </summary>
    internal class ProgressGlobeAnimation : IProgressGlobeAnimation
    {
        #region Fields

        private readonly IApplication _Application;
        private MouseCursorReverter _MouseCursorReverter;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressGlobeAnimation" /> class.
        /// </summary>
        /// <param name="application">The application.</param>
        public ProgressGlobeAnimation(IApplication application)
        {
            _Application = application;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the application.
        /// </summary>
        /// <value>
        ///     The application.
        /// </value>
        protected IApplication Application
        {
            get { return _Application; }
        }

        #endregion

        #region IProgressGlobeAnimation Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Starts the global spinning in ArcMap and updates the message on the status bar.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        /// <param name="statusMessage">The status message.</param>
        public void Play(MouseCursorImage cursor, string statusMessage)
        {
            if (_MouseCursorReverter != null)
                _MouseCursorReverter.Dispose();

            _MouseCursorReverter = new MouseCursorReverter(cursor);

            IAnimationProgressor animation = _Application.StatusBar.ProgressAnimation;
            animation.Show();

            _Application.StatusBar.PlayProgressAnimation(true);
            _Application.StatusBar.Message[0] = statusMessage;
        }

        /// <summary>
        ///     Sets the message on the status bar
        /// </summary>
        /// <value>
        ///     The message on the status bar.
        /// </value>
        public string Message
        {
            set { _Application.StatusBar.Message[0] = value; }
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
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_MouseCursorReverter != null)
                {
                    _MouseCursorReverter.Dispose();
                }

                _Application.StatusBar.PlayProgressAnimation(false);
                _Application.StatusBar.Message[0] = null;

                IAnimationProgressor animation = _Application.StatusBar.ProgressAnimation;
                animation.Hide();
            }
        }

        #endregion
    }
}