namespace System.Collections
{
    /// <summary>
    ///     A sealed class used to interupt a method in the middle of its execution and then safely called again
    ///     ("re-entered") before its previous invocations complete execution.
    ///     The interruption could be caused by an internal action such as a jump or call, or by an external action such as a
    ///     hardware interrupt or signal. Once the reentered invocation completes, the previous invocations will resume correct
    ///     execution.
    /// </summary>
    public sealed class ReentrancyMonitor : IDisposable
    {
        #region Fields

        private int _BusyCount;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get { return _BusyCount > 0; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            --_BusyCount;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Increments the busy counter for the monitor.
        /// </summary>
        public void Set()
        {
            ++_BusyCount;
        }

        #endregion
    }
}