using log4net.Appender;

namespace System.Diagnostics
{
    /// <summary>
    ///     Use to control the scope of an appender that is used with the Log class.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class LogScope : IDisposable
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogScope" /> class.
        /// </summary>
        /// <param name="appender">The appender.</param>
        public LogScope(IAppender appender)
        {
            this.Name = appender.Name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; private set; }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                LogScopeManager.RemoveScope(this);
            }
        }

        #endregion
    }
}