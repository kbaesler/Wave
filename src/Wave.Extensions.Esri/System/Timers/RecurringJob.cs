namespace System.Timers
{
    /// <summary>
    ///     A schedule task that executes an action every time the timer elapses.
    /// </summary>
    /// <seealso cref="T:System.IDisposable" />
    public class RecurringJob : BackgroundJob
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="RecurringJob" /> class.
        /// </summary>
        /// <param name="id">The unique identifier for the job.</param>
        /// <param name="method">The delegate function that is executed.</param>
        public RecurringJob(string id, Action method)
            : base(id, method)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RecurringJob" /> class.
        /// </summary>
        /// <param name="method">The delgate function that is executed.</param>
        public RecurringJob(Action method)
            : base(method)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this instance is scheduled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is scheduled; otherwise, <c>false</c>.
        /// </value>
        public bool IsScheduled => (Timer.Interval > 0 && Timer.Enabled);

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the timer.
        /// </summary>
        /// <value>
        ///     The timer.
        /// </value>
        protected Timer Timer { get; } = new Timer();

        #endregion

        #region Public Methods

        /// <summary>
        ///     Schedules the job to be executed based on the a regular time interval.
        /// </summary>
        /// <param name="interval">The interval in milliseconds of when the job should be executed.</param>
        public override void Schedule(double interval)
        {
            Timer.Interval = interval;
            Timer.Start();
            Timer.Elapsed += (sender, args) =>
            {
                // The method can be executed simultaneously on two thread pool threads if the timer interval is 
                // less than the time required to execute the method.
                Timer ts = (Timer) sender;
                ts.Stop();

                Run();

                // Restart the timer to execute within the next miliseconds.                
                ts.Start();
            };
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
                Timer?.Stop();
                Timer?.Dispose();
            }
        }

        #endregion
    }
}